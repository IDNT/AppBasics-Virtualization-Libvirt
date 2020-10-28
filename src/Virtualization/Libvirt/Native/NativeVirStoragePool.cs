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
using System.Runtime.InteropServices;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// The StoragePool class expose all libvirt storage pool related functions
    /// </summary>
    public class NativeVirStoragePool
    {
		private const int MaxStringLength = 1024;

        /// <summary>
        /// Build the underlying storage pool.
        /// </summary>
        /// <param name="pool">
        /// A pointer to storage pool.
        /// </param>
        /// <param name="flags">
        /// Future flags, use 0 for now.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 upon failure
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolBuild")]
        public static extern int Build(IntPtr pool, VirStoragePoolBuildFlags flags);

        /// <summary>
        /// Starts an inactive storage pool.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="flags">
        /// Future flags, use 0 for now.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 if it could not be started.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolCreate")]
        public static extern int Create(IntPtr pool, uint flags);

        /// <summary>
        /// Create a new storage based on its XML description. The pool is not persistent,
        /// so its definition will disappear when it is destroyed, or if the host is restarted
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/>pointer to hypervisor connection.
        /// </param>
        /// <param name="xmlDesc">
        /// A <see cref="System.String"/>XML description for new pool.
        /// </param>
        /// <param name="flags">
        /// A <see cref="System.UInt32"/>future flags, use 0 for now.
        /// </param>
        /// <returns>
        /// A virStoragePoolPtr object, or NULL if creation failed.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolCreateXML")]
        public static extern IntPtr CreateXML(IntPtr conn, string xmlDesc, uint flags);

        /// <summary>
        /// Define a new inactive storage pool based on its XML description. The pool is persistent,
        /// until explicitly undefined.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/>pointer to hypervisor connection.
        /// </param>
        /// <param name="xml">
        /// A <see cref="System.String"/>XML description for new pool.
        /// </param>
        /// <param name="flags">
        /// A <see cref="System.UInt32"/>future flags, use 0 for now.
        /// </param>
        /// <returns>
        /// A virStoragePoolPtr object, or NULL if creation failed.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolDefineXML")]
        public static extern IntPtr DefineXML(IntPtr conn, string xml, uint flags);

        /// <summary>
        /// Delete the underlying pool resources. This is a non-recoverable operation. The virStoragePoolPtr object itself is not free'd.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="flags">
        /// A <see cref="VirStoragePoolDeleteFlags"/>flags for obliteration process.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 if it could not be obliterate.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolDelete")]
        public static extern int Delete(IntPtr pool, VirStoragePoolDeleteFlags flags);

        /// <summary>
        /// Destroy an active storage pool. This will deactivate the pool on the host,
        /// but keep any persistent config associated with it.
        /// If it has a persistent config it can later be restarted with virStoragePoolCreate().
        /// This does not free the associated virStoragePoolPtr object.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 if it could not be destroyed.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolDestroy")]
        public static extern int Destroy(IntPtr pool);

        /// <summary>
        /// Free a storage pool object, releasing all memory associated with it. Does not change the state of the pool on the host.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 if it could not be free'd.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolFree")]
        public static extern int Free(IntPtr pool);

        /// <summary>
        /// Fetches the value of the autostart flag, which determines whether the pool is automatically started at boot time.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="autotart"></param>
        /// <returns>
        /// 0 on success, -1 on failure
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolGetAutostart")]
        public static extern int GetAutostart(IntPtr pool, out int autotart);

        /// <summary>
        /// Provides the connection pointer associated with a storage pool. The reference counter on the connection is not increased by this call.
        /// WARNING: When writing libvirt bindings in other languages, do not use this function. Instead, store the connection and the pool object together.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to a pool.
        /// </param>
        /// <returns>
        /// A <see cref="IntPtr"/>the virConnectPtr or NULL in case of failure.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolGetConnect")]
        public static extern IntPtr GetConnect(IntPtr pool);

        /// <summary>
        /// Get volatile information about the storage pool such as free space / usage summary.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="info">
        /// Pointer at which to store info.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 on failure.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolGetInfo")]
        public static extern int GetInfo(IntPtr pool, ref VirStoragePoolInfo info);

        /// <summary>
        /// Fetch the locally unique name of the storage pool.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <returns>
        /// The name of the pool, or NULL on error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolGetName")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringWithoutNativeCleanUpMarshaler))]
        public static extern string GetName(IntPtr pool);

        /// <summary>
        /// Fetch the globally unique ID of the storage pool.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>Pointer to storage pool.
        /// </param>
        /// <param name="uuid">
        /// Buffer of VIR_UUID_BUFLEN bytes in size.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 on error
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolGetUUID", CharSet = CharSet.Ansi)]
        public static extern int GetUUID(IntPtr pool, [Out] byte[] uuid);

        /// <summary>
        /// Fetch the globally unique ID of the storage pool as a string.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="uuid">
        /// A <see cref="IntPtr"/>buffer of VIR_UUID_STRING_BUFLEN bytes in size.
        /// </param>
        /// <returns>
        /// 0 on success, or -1 on error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolGetUUIDString")]
        private static extern int GetUUIDString(IntPtr pool, [Out] char[] uuid);

        ///<summary>
        /// Fetch the globally unique ID of the storage pool as a string.
        ///</summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        ///<returns>the storage pool UUID</returns>
        public static string GetUUIDString(IntPtr pool)
        {
            char[] uuidArray = new char[36];
            GetUUIDString(pool, uuidArray);
            return new string(uuidArray);
        }

        /// <summary>
        /// Fetch the globally unique key of the volume
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to volume.
        /// </param>
        /// <returns>
        /// key or null if not found
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStorageVolGetKey")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringWithoutNativeCleanUpMarshaler))]
        public static extern string GetVolKey(IntPtr volume);

        /// <summary>
        /// Fetch an XML document describing all aspects of the storage pool.
        /// This is suitable for later feeding back into the virStoragePoolCreateXML method.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="flags">
        /// Flags for XML format options (set of virDomainXMLFlags).
        /// </param>
        /// <returns>
        /// A XML document, or NULL on error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolGetXMLDesc")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringWithoutNativeCleanUpMarshaler))]
        public static extern string GetXMLDesc(IntPtr pool, VirDomainXMLFlags flags);

        /// <summary>
        /// Determine if the storage pool is currently running.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to the storage pool object.
        /// </param>
        /// <returns>
        /// 1 if running, 0 if inactive, -1 on error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolIsActive")]
        public static extern int IsActive(IntPtr pool);

        /// <summary>
        /// Determine if the storage pool has a persistent configuration which means it will still exist after shutting down.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to the storage pool object.
        /// </param>
        /// <returns>
        /// 1 if persistent, 0 if transient, -1 on error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolIsPersistent")]
        public static extern int IsPersistent(IntPtr pool);

        /// <summary>
        /// Fetch list of storage volume names, limiting to at most maxnames.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="names">
        /// A <see cref="IntPtr"/>array in which to storage volume names.
        /// </param>
        /// <param name="maxnames">
        /// A <see cref="System.Int32"/>size of names array.
        /// </param>
        /// <returns>
        /// The number of names fetched, or -1 on error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolListVolumes")]
        private static extern int ListVolumes(IntPtr pool, IntPtr names, int maxnames);
        /// <summary>
        /// Fetch list of storage volume names, limiting to at most maxnames.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="names">
        /// A <see cref="IntPtr"/>array in which to storage volume names.
        /// </param>
        /// <param name="maxnames">
        /// A <see cref="System.Int32"/>size of names array.
        /// </param>
        /// <returns>
        /// The number of names fetched, or -1 on error.
        /// </returns>
        public static int ListVolumes(IntPtr pool, ref string[] names, int maxnames)
        {
            IntPtr namesPtr = Marshal.AllocHGlobal(MaxStringLength);
            int count = ListVolumes(pool, namesPtr, maxnames);
            if (count > 0)
                names = MarshalHelper.ptrToStringArray(namesPtr, count);
            Marshal.FreeHGlobal(namesPtr);
            return count;
        }
        /// <summary>
        /// Fetch a storage pool based on its unique name.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/>pointer to hypervisor connection.
        /// </param>
        /// <param name="name">
        /// Name of pool to fetch.
        /// </param>
        /// <returns>
        /// A <see cref="IntPtr"/>virStoragePoolPtr object, or NULL if no matching pool is found.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolLookupByName")]
        public static extern IntPtr LookupByName(IntPtr conn, string name);

        /// <summary>
        /// Fetch a storage pool volume based on its unique name.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/>pointer to storage pool
        /// </param>
        /// <param name="name">
        /// Name of volume to fetch.
        /// </param>
        /// <returns>
        /// A <see cref="IntPtr"/>virStorageVolPtr object, or NULL if no matching volume was found.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStorageVolLookupByName")]
        public static extern IntPtr LookupVolByName(IntPtr pool, string name);

        /// <summary>
        /// Fetch a storage pool based on its globally unique id.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/>pointer to hypervisor connection.
        /// </param>
        /// <param name="uuid">
        /// Globally unique id of pool to fetch.
        /// </param>
        /// <returns>
        /// A virStoragePoolPtr object, or NULL if no matching pool is found
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolLookupByUUID", CharSet = CharSet.Ansi)]
        public static extern IntPtr LookupByUUID(IntPtr conn, byte[] uuid);

        /// <summary>
        /// Fetch a storage pool based on its globally unique id.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/>pointer to hypervisor connection.
        /// </param>
        /// <param name="uuidstr">
        /// A <see cref="System.String"/>globally unique id of pool to fetch.
        /// </param>
        /// <returns>
        /// A <see cref="IntPtr"/>object, or NULL if no matching pool is found.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolLookupByUUIDString")]
        public static extern IntPtr LookupByUUIDString(IntPtr conn, string uuidstr);

        /// <summary>
        /// Fetch a storage pool which contains a particular volume.
        /// </summary>
        /// <param name="vol">
        /// A <see cref="IntPtr"/>pointer to storage volume.
        /// </param>
        /// <returns>
        /// A <see cref="IntPtr"/>object, or NULL if no matching pool is found.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolLookupByVolume")]
        public static extern IntPtr LookupByVolume(IntPtr vol);

        /// <summary>
        /// Fetch the number of storage volumes within a pool.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>the number of storage pools, or -1 on failure.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolNumOfVolumes")]
        public static extern int NumOfVolumes(IntPtr pool);

        /// <summary>
        /// Increment the reference count on the pool. For each additional call to this method,
        /// there shall be a corresponding call to virStoragePoolFree to release the reference count,
        /// once the caller no longer needs the reference to this object.
        /// This method is typically useful for applications where multiple threads are using a connection,
        /// and it is required that the connection remain open until all threads have finished using it. ie,
        /// each new thread using a pool would increment the reference count.
        /// </summary>
        /// <param name="pool">
        /// The pool to hold a reference on.
        /// </param>
        /// <returns>
        /// 0 in case of success, -1 in case of failure.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolRef")]
        public static extern int Ref(IntPtr pool);

        /// <summary>
        /// Request that the pool refresh its list of volumes. This may involve communicating with a remote server,
        /// and/or initializing new devices at the OS layer.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="flags">
        /// A <see cref="System.UInt32"/>flags to control refresh behaviour (currently unused, use 0).
        /// </param>
        /// <returns>
        /// 0 if the volume list was refreshed, -1 on failure.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolRefresh")]
        public static extern int Refresh(IntPtr pool, uint flags);

        /// <summary>
        /// Sets the autostart flag.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <param name="autostart">
        /// A <see cref="System.Int32"/>new flag setting.
        /// </param>
        /// <returns>
        /// 0 on success, -1 on failure.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolSetAutostart")]
        public static extern int SetAutostart(IntPtr pool, int autostart);

        /// <summary>
        /// Undefine an inactive storage pool.
        /// </summary>
        /// <param name="pool">
        /// A <see cref="IntPtr"/>pointer to storage pool.
        /// </param>
        /// <returns>
        /// 0 on success, -1 on failure.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virStoragePoolUndefine")]
        public static extern int Undefine(IntPtr pool);
    }
}
