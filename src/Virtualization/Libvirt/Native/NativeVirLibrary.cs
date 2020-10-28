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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// The Library class expose all libvirt library related methods
    /// </summary>
    public class NativeVirLibrary
    {
        
        /// <summary>
        /// Provides two information back, @libVer is the version of the library while @typeVer will be the version of the hypervisor
        /// type @type against which the library was compiled. If @type is NULL, "Xen" is assumed,
        /// if @type is unknown or not available, an error code will be returned and @typeVer will be 0.
        /// </summary>
        /// <param name="libVer">
        /// A <see cref="System.UInt64"/>return value for the library version (OUT).
        /// </param>
        /// <param name="type">
        /// A <see cref="System.String"/>the type of connection/driver looked at.
        /// </param>
        /// <param name="typeVer">
        /// A <see cref="System.UInt64"/>return value for the version of the hypervisor (OUT).
        /// </param>
        /// <returns>
        /// -1 in case of failure, 0 otherwise, and values for @libVer and @typeVer have the format major * 1,000,000 + minor * 1,000 + release.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virGetVersion")]
        public static extern int GetVersion([Out] out ulong libVer, [In] string type, [Out] out ulong typeVer);

        /// <summary>
        /// Initialize the library. It's better to call this routine at startup in multithreaded applications to avoid
        /// potential race when initializing the library.
        /// </summary>
        /// <returns>
        /// 0 in case of success, -1 in case of error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virInitialize")]
        internal static extern int InitializeLib();
    }
}
