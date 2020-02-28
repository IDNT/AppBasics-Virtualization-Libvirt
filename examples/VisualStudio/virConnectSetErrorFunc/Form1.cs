﻿/*
 * Copyright (C)
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *
 * See COPYING.LIB for the License of this software
 *
 * Sample code for :
 * Functions :
 *      Connect.OpenAuth
 *      Domain.GetInfo
 *      Domain.LookupByName
 *      Errors.GetLastError
 *      Errors.SetErrorFunc
 *
 * Types :
 *      DomainInfo
 *      Error
 *
 */

using System;
using System.Windows.Forms;
using Libvirt;

namespace virConnectSetErrorFunc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            IntPtr conn = Connect.Open(tbURI.Text);

            if (conn != IntPtr.Zero)
            {
                Errors.SetErrorFunc(IntPtr.Zero, ErrorCallback);
                IntPtr domain = Domain.LookupByName(conn, tbDomainName.Text);
                VirDomainInfo di = new VirDomainInfo();
                Domain.GetInfo(domain, di);
                textBox1.Text = di.State.ToString();
                textBox2.Text = di.maxMem.ToString();
                textBox3.Text = di.memory.ToString();
                textBox4.Text = di.nrVirtCpu.ToString();
                textBox5.Text = di.cpuTime.ToString();
            }
            else
            {
                VirError error = Errors.GetLastError();
                ShowError(error);
            }
        }

        private void ErrorCallback(IntPtr userData, VirError error)
        {
            ShowError(error);
        }

        private void ShowError(VirError libvirtError)
        {
            string ErrorBoxMessage = string.Format("Error number : {0}. Error message : {1}", libvirtError.code, libvirtError.Message);
            MessageBox.Show(ErrorBoxMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
