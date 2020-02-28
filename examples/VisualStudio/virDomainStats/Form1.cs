﻿/*
 * Copyright (C)
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *
 * See COPYING.LIB for the License of this software
 *
 * Sample code for :
 * Functions :
 *      Connect.Open
 *      Connect.NumOfDomains
 *      Connect.ListDomains
 *      Domain.BlockStats
 *      Domain.GetInfo
 *      Domain.GetName
 *      Domain.GetXMLDesc
 *      Domain.InterfaceStats
 *      Domain.ListDomains
 *      Domain.LookupByID
 *      Domain.LookupByName
 *      Errors.SetErrorFunc
 *
 * Types :
 *      DomainBlockStatsStruct
 *      DomainInterfaceStatsStruct
 *      DomainInfo
 *      Error
 *
 */

using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Libvirt;

namespace virDomainStats
{
    public partial class Form1 : Form
    {
        private IntPtr _conn = IntPtr.Zero;
        private IntPtr _domainPtr = IntPtr.Zero;

        public Form1()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            _conn = NativeVirConnect.Open(tbURI.Text);
            NativeVirErrors.SetErrorFunc(IntPtr.Zero, ErrorCallback);
            cbDomains.Items.Clear();
            int nbDomains = NativeVirConnect.NumOfDomains(_conn);
            int[] domainIDs = new int[nbDomains];
            NativeVirConnect.ListDomains(_conn, domainIDs, nbDomains);
            foreach (IntPtr domainPtr in domainIDs.Select(domainID => NativeVirDomain.LookupByID(_conn, domainID)))
            {
                cbDomains.Items.Add(NativeVirDomain.GetName(domainPtr));
                NativeVirDomain.Free(domainPtr);
            }
            if (cbDomains.Items.Count > 0)
            {
                cbDomains.SelectedIndex = 0;
            }
        }

        private void cbDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            _domainPtr = NativeVirDomain.LookupByName(_conn, cbDomains.Text);

            UpdateDomainInfo();

            cbBlockDev.Items.Clear();
            foreach (string dev in  GetDomainBlockDevices(NativeVirDomain.GetXMLDesc(_domainPtr, 0)))
            {
                cbBlockDev.Items.Add(dev);
            }
            if (cbBlockDev.Items.Count > 0)
            {
                cbBlockDev.SelectedIndex = 0;
            }

            cbInterfaces.Items.Clear();
            foreach (string iface in GetDomainInterfaces(NativeVirDomain.GetXMLDesc(_domainPtr, 0)))
            {
                cbInterfaces.Items.Add(iface);
            }
            if (cbInterfaces.Items.Count > 0)
            {
                cbInterfaces.SelectedIndex = 0;
            }
        }

        private string[] GetDomainInterfaces(string xmlDomainDescription)
        {
            XmlDocument xmlDescription = new XmlDocument();
            xmlDescription.LoadXml(xmlDomainDescription);
            XmlNodeList iFaceNodeList = xmlDescription.SelectNodes("//domain/devices/interface");

            return (from XmlNode xn in iFaceNodeList select xn.SelectSingleNode("target").Attributes["dev"].Value).ToArray();
        }

        private void UpdateDomainInfo()
        {
            VirDomainInfo domainInfo = new VirDomainInfo();
            NativeVirDomain.GetInfo(_domainPtr, domainInfo);

            tbState.Text = domainInfo.State.ToString();
            tbMaxMem.Text = domainInfo.maxMem.ToString();
            tbMemory.Text = domainInfo.memory.ToString();
            tbNrVirtCpu.Text = domainInfo.nrVirtCpu.ToString();
            tbCpuTime.Text = domainInfo.cpuTime.ToString();
        }

        private string[] GetDomainBlockDevices(string xmlDomainDescription)
        {
            XmlDocument xmlDescription = new XmlDocument();
            xmlDescription.LoadXml(xmlDomainDescription);
            XmlNodeList devNodeList = xmlDescription.SelectNodes("//domain/devices/disk");

            return (from XmlNode xn in devNodeList select xn.SelectSingleNode("target").Attributes["dev"].Value).ToArray();
        }

        private void cbBlockDev_SelectedIndexChanged(object sender, EventArgs e)
        {
            VirDomainBlockStatsStruct blockStat;
            NativeVirDomain.BlockStats(_domainPtr, cbBlockDev.Text, out blockStat);//, Marshal.SizeOf(blockStat));

            tbReadRequest.Text = blockStat.rd_req.ToString();
            tbReadBytes.Text = blockStat.rd_bytes.ToString();
            tbWriteRequests.Text = blockStat.wr_req.ToString();
            tbWriteBytes.Text = blockStat.wr_bytes.ToString();
            tbErrors.Text = blockStat.errs.ToString();
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

        private void cbInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            VirDomainInterfaceStatsStruct interfaceStat;
            NativeVirDomain.InterfaceStats(_domainPtr, cbInterfaces.Text, out interfaceStat);
            tbRxBytes.Text = interfaceStat.rx_bytes.ToString();
            tbRxDrop.Text = interfaceStat.rx_drop.ToString();
            tbRxErrs.Text = interfaceStat.rx_errs.ToString();
            tbRxPackets.Text = interfaceStat.rx_packets.ToString();
            tbTxBytes.Text = interfaceStat.tx_bytes.ToString();
            tbTxDrop.Text = interfaceStat.tx_drop.ToString();
            tbTxErrs.Text = interfaceStat.tx_errs.ToString();
            tbTxPackets.Text = interfaceStat.tx_packets.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            NativeVirDomain.Free(_domainPtr);
            NativeVirConnect.Close(_conn);
        }
    }
}
