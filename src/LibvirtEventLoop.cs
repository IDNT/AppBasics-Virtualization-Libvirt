﻿/*
 * Libvirt-dotnet
 * 
 * Copyright 2020 IDNT (https://www.idnt.net) and Libvirt-dotnet contributors.
 * 
 * This project incorporates work by the following original authors and contributors
 * to libvirt-csharp:
 *    
 *    Copyright (C) 
 *      Arnaud Champion <arnaud.champion@devatom.fr>
 *      Jaromír Červenka <cervajz@cervajz.com>
 *
 * Licensed under the GNU Lesser General Public Library, Version 2.1 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a 
 * copy of the License at
 *
 * https://www.gnu.org/licenses/lgpl-2.1.en.html
 * 
 * or see LICENSE for a copy of the license terms. Unless required by applicable 
 * law or agreed to in writing, software distributed under the License is distributed 
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express 
 * or implied. See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Libvirt
{
    public class LibvirtEventLoop : IDisposable
    {
        private readonly LibvirtConnection _connection;
        private int _domainEventHandlerRegistrationId = -1;
        private int _storagePoolLifecycleEventHandlerRegistrationId = -1;
        private int _storagePoolRefreshEventHandlerRegistrationId = -1;

        private readonly VirConnectDomainEventCallback _domainEventCallback;
        private readonly VirConnectStoragePoolEventLifecycleCallback _storagePoolLifecycleCallback;
        private readonly VirConnectStoragePoolGenericEventCallback _storagePoolRefreshCallback;

        internal LibvirtEventLoop(LibvirtConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException("connection");

            _connection.ShutdownToken.Register(ShutdownEventLoop);

            _domainEventCallback = new VirConnectDomainEventCallback(DomainEventCallback);
            _storagePoolLifecycleCallback = new VirConnectStoragePoolEventLifecycleCallback(StoragePoolLifecycleEventCallback);
            _storagePoolRefreshCallback = new VirConnectStoragePoolGenericEventCallback(StoragePoolRefreshEventCallback);

            if ((_domainEventHandlerRegistrationId = NativeVirConnect.DomainEventRegisterAny(
                _connection.ConnectionPtr, IntPtr.Zero, VirDomainEventID.VIR_DOMAIN_EVENT_ID_LIFECYCLE,
                _domainEventCallback, IntPtr.Zero, FreeCallbackFunc)) < 0)
                throw new LibvirtException();

            if ((_storagePoolLifecycleEventHandlerRegistrationId = NativeVirConnect.StoragePoolEventRegisterAny(
                _connection.ConnectionPtr, IntPtr.Zero, VirStoragePoolEventID.VIR_STORAGE_POOL_EVENT_ID_LIFECYCLE,
                _storagePoolLifecycleCallback, IntPtr.Zero, FreeCallbackFunc)) < 0)
                throw new LibvirtException();

            if ((_storagePoolRefreshEventHandlerRegistrationId = NativeVirConnect.StoragePoolEventRegisterAny(
                _connection.ConnectionPtr, IntPtr.Zero,
               (int)VirStoragePoolEventID.VIR_STORAGE_POOL_EVENT_ID_REFRESH,
               _storagePoolRefreshCallback, IntPtr.Zero, FreeCallbackFunc)) < 0)
                throw new LibvirtException();

            _lvEventLoopTask = Task.Factory.StartNew(EventLoopTask, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning);
        }

        private Task _lvEventLoopTask = null;

        private void ShutdownEventLoop()
        {
            if (_lvEventLoopTask != null)
            {
                try
                {
                    if (!_lvEventLoopTask.Wait(TimeSpan.FromSeconds(60)))
                        Trace.WriteLine("Event loop did not terminate in time.");
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private void EventLoopTask()
        {
            Trace.WriteLine("Event loop task is running.");

            while (!_connection.ShutdownToken.IsCancellationRequested)
            {
                if (!_connection.IsAlive)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (Libvirt.NativeVirEvent.RunDefaultImpl() < 0)
                    throw new LibvirtException();
            }

            Trace.WriteLine("Event loop task ended.");
        }

        private void FreeCallbackFunc(IntPtr opaque)
        {
        }

        private void DomainEventCallback(IntPtr conn, IntPtr dom, VirDomainEventType evt, int detail, IntPtr opaque)
        {
            Trace.WriteLine($"Received domain event of type {evt.ToString()}.");

            var domUUID = new byte[16];
            Guid domGuid = Guid.Empty;
            if (NativeVirDomain.GetUUID(dom, domUUID) < 0 || Guid.Empty.Equals(domGuid = domUUID.ToGuid()))
            {
                Trace.WriteLine($"Received event for unknown domain.");
                return;
            }
            _connection.DispatchDomainEvent(domGuid, new VirDomainEventArgs { UniqueId = domGuid, EventType = evt, Detail = detail });
        }

        private void StoragePoolLifecycleEventCallback(IntPtr conn, IntPtr pool, VirStoragePoolEventLifecycleType evt, int detail, IntPtr opaque)
        {
            Trace.WriteLine($"Received storage pool event of type {evt.ToString()}.");

            var poolUUID = new byte[16];
            Guid poolGuid = Guid.Empty;
            if (NativeVirStoragePool.GetUUID(pool, poolUUID) < 0 || Guid.Empty.Equals(poolGuid = poolUUID.ToGuid()))
            {
                Trace.WriteLine($"Received event for unknown storage pool.");
                return;
            }
            _connection.DispatchStoragePoolEvent(poolGuid, new VirStoragePoolLifecycleEventArgs { UniqueId = poolGuid, EventType = evt, Detail = detail });
        }

        private void StoragePoolRefreshEventCallback(IntPtr conn, IntPtr pool, IntPtr opaque)
        {
            Trace.WriteLine($"Received storage pool refresh event ptr={pool}.");

            var poolUUID = new byte[16];
            Guid poolGuid = Guid.Empty;
            if (NativeVirStoragePool.GetUUID(pool, poolUUID) < 0 || Guid.Empty.Equals(poolGuid = poolUUID.ToGuid()))
            {
                Trace.WriteLine($"Received event for unknown storage pool.");
                return;
            }
            _connection.DispatchStoragePoolEvent(poolGuid, new VirStoragePoolRefreshEventArgs { UniqueId = poolGuid });
        }

        #region IDisposable implementation
        private Int32 _isDisposing = 0;

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposing, 1, 0) == 1)
                return;

            if (_domainEventHandlerRegistrationId >= 0)
                NativeVirConnect.DomainEventDeregister(_connection.ConnectionPtr, DomainEventCallback);

            if (_storagePoolLifecycleEventHandlerRegistrationId > 0)
                NativeVirConnect.StoragePoolEventDeregisterAny(_connection.ConnectionPtr, _storagePoolLifecycleEventHandlerRegistrationId);

            if (_storagePoolRefreshEventHandlerRegistrationId > 0)
                NativeVirConnect.StoragePoolEventDeregisterAny(_connection.ConnectionPtr, _storagePoolRefreshEventHandlerRegistrationId);
        }
        #endregion
    }
}
