﻿/*
 * Copyright (C)
 *
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *   Marcus Zoller <marcus.zoller@idnt.net>
 *
 * and the Libvirt-CSharp contributors.
 * 
 * Licensed under the GNU Lesser General Public Library, Version 2.1 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a 
 * copy of the License at
 *
 * https://www.gnu.org/licenses/lgpl-2.1.en.html
 * 
 * or see COPYING.LIB for a copy of the license terms. Unless required by applicable 
 * law or agreed to in writing, software distributed under the License is distributed 
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express 
 * or implied. See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// Structure to handle domain informations
    /// <see cref="https://libvirt.org/html/libvirt-libvirt-domain.html#virDomainInfo"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirDomainInfo
    {
        /// <summary>
        /// The running state, one of virDomainState.
        /// </summary>
        private byte state;
        /// <summary>
        /// The maximum memory in KBytes allowed.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public UIntPtr MaxMem;
        /// <summary>
        /// The memory in KBytes used by the domain.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public UIntPtr Memory;
        /// <summary>
        /// The number of virtual CPUs for the domain.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort NrVirtCpu;
        /// <summary>
        /// The CPU time used in nanoseconds.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public UIntPtr CpuTime;
        /// <summary>
        /// The running state, one of virDomainState.
        /// </summary>
        public VirDomainState State { get { return (VirDomainState)state; } }
    }
}
