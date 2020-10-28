﻿/*
 * Copyright (C)
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *
 * See COPYING.LIB for the License of this software
 * 
 * Sample code for :
 * Function :
 *      virConnectOpen
 *      virConnectNumOfStoragePools
 *      virConnectListStoragePools
 */

using System;
using System.Windows.Forms;
using IDNT.AppBasics.Virtualization.Libvirt;
using IDNT.AppBasics.Virtualization.Libvirt.Native;

namespace virConnectOpen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            lbStoragePool.Items.Clear();

            IntPtr conn = NativeVirConnect.Open(tbURI.Text);
            if (conn != IntPtr.Zero)
            {
                int numOfStoragePools = NativeVirConnect.NumOfStoragePools(conn);
                if (numOfStoragePools == -1)
                {
                    ShowError("Unable to get the number of storage pools");
                    goto cleanup;
                }
                string[] storagePoolsNames = new string[numOfStoragePools];
                int listStoragePools = NativeVirConnect.ListStoragePools(conn, ref storagePoolsNames, numOfStoragePools);
                if (listStoragePools == -1)
                {
                    ShowError("Unable to list storage pools");
                    goto cleanup;
                }
                foreach (string storagePoolName in storagePoolsNames)
                    lbStoragePool.Items.Add(storagePoolName);
            cleanup:
                NativeVirConnect.Close(conn);
            }
            else
            {
                ShowError("Unable to connect");
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
