﻿/*
 * Copyright (C)
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *
 * See COPYING.LIB for the License of this software
 *
 * Sample code for :
 * Function :
 *      Connect.DomainEventRegister
 *      Connect.OpenAuth
 *      Event.RegisterImpl
 *
 *
 * Types :
 *      EventHandleType
 *      EventHandleCallback
 *      FreeCallback
 *      EventTimeoutCallback
 *      DomainEventType
 *      DomainEventDefinedDetailType
 *      DomainEventUndefinedDetailType
 *      DomainEventResumedDetailType
 *      DomainEventStartedDetailType
 *      DomainEventStoppedDetailType
 *      DomainEventSuspendedDetailType
 *
 */

using System;
using System.Threading;
using System.Windows.Forms;
using Libvirt;

namespace virEventRegisterImpl
{
    delegate void AddEventDelegate(string message);

    public partial class Form1 : Form
    {
        private int _fd;
        private VirEventHandleType _event;
        private EventHandleCallback _cb;
        private VirFreeCallback _ff;
        private IntPtr _opaque;
        private int _active;
        private int _timeout;
        private EventTimeoutCallback _tcb;
        private IntPtr _conn;
        private readonly System.Threading.Timer _callbackTimer;

        public Form1()
        {
            InitializeComponent();
            _callbackTimer = new System.Threading.Timer(TimerCallback, null, Timeout.Infinite, 50);
        }

        private void TimerCallback(object state)
        {
            if (_tcb != null && _active == 1)
                _tcb(_timeout, _opaque);

            if (_cb != null)
            {
                _cb(0,
                     _fd,
                     (int)_event,
                     _opaque);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NativeVirEvent.RegisterImpl(AddHandleFunc, UpdateHandleFunc, RemoveHandleFunc, AddTimeoutFunc, UpdateTimeoutFunc,
                               RemoveTimeoutFunc);
        }

        private void FreeFunc(IntPtr opaque)
        {
        }

        private int RemoveTimeoutFunc(int timer)
        {
            _active = 0;
            if (_ff != null)
                _ff(_opaque);
            return 0;
        }

        private void UpdateTimeoutFunc(int timer, int timeout)
        {
            _timeout = timeout;
        }

        private int AddTimeoutFunc(int timeout, EventTimeoutCallback cb, IntPtr opaque, VirFreeCallback ff)
        {
            _active = 1;
            _timeout = timeout;
            _tcb = cb;
            _ff = ff;
            _opaque = opaque;
            return 0;
        }

        private int RemoveHandleFunc(int watch)
        {
            _fd = 0;
            if (_ff != null)
                _ff(_opaque);
            return 0;
        }
        private void UpdateHandleFunc(int watch, int events)
        {
            _event = (VirEventHandleType)events;
        }

        private int AddHandleFunc(int fd, int events, EventHandleCallback cb, IntPtr opaque, VirFreeCallback ff)
        {
            _fd = fd;
            _event = (VirEventHandleType)events;
            _cb = cb;
            _ff = ff;
            _opaque = opaque;
            return 0;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _conn = NativeVirConnect.Open(tbURI.Text);
            int cbInstall = NativeVirConnect.DomainEventRegister(_conn, DomainEventCallback, IntPtr.Zero, FreeFunc);
            if (cbInstall == 0)
            {
                tbEvents.Text = "Connection Done\r\n";
                _callbackTimer.Change(0, 50);
            }
            else
            {
                MessageBox.Show("DomainEventRegister failed", "DomainEventRegister failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DomainEventCallback(IntPtr conn, IntPtr dom, VirDomainEventType evt, int detail, IntPtr opaque)
        {
            switch (evt)
            {
                case VirDomainEventType.VIR_DOMAIN_EVENT_DEFINED:
                    BeginInvoke(new AddEventDelegate(AddEvent), new object[] {"Domain defined :"});
                    switch ((VirDomainEventDefinedDetailType)detail)
                    {
                        case VirDomainEventDefinedDetailType.VIR_DOMAIN_EVENT_DEFINED_ADDED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain added\r\n" });
                            break;
                        case VirDomainEventDefinedDetailType.VIR_DOMAIN_EVENT_DEFINED_UPDATED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain updated\r\n" });
                            break;
                    }
                    break;
                case VirDomainEventType.VIR_DOMAIN_EVENT_UNDEFINED:
                    BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain undefined :" });
                    switch ((VirDomainEventUndefinedDetailType)detail)
                    {
                        case VirDomainEventUndefinedDetailType.VIR_DOMAIN_EVENT_UNDEFINED_REMOVED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain deleted\r\n" });
                            break;
                    }
                    break;
                case VirDomainEventType.VIR_DOMAIN_EVENT_RESUMED:
                    BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain resumed :" });
                    switch ((VirDomainEventResumedDetailType)detail)
                    {
                        case VirDomainEventResumedDetailType.VIR_DOMAIN_EVENT_RESUMED_MIGRATED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain migrated\r\n" });
                            break;
                        case VirDomainEventResumedDetailType.VIR_DOMAIN_EVENT_RESUMED_UNPAUSED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain unpaused\r\n" });
                            break;
                    }
                    break;
                case VirDomainEventType.VIR_DOMAIN_EVENT_STARTED:
                    BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain started :" });
                    switch ((VirDomainEventStartedDetailType)detail)
                    {
                        case VirDomainEventStartedDetailType.VIR_DOMAIN_EVENT_STARTED_BOOTED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain booted\r\n" });
                            break;
                        case VirDomainEventStartedDetailType.VIR_DOMAIN_EVENT_STARTED_MIGRATED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain migrated\r\n" });
                            break;
                        case VirDomainEventStartedDetailType.VIR_DOMAIN_EVENT_STARTED_RESTORED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain restored\r\n" });
                            break;
                    }
                    break;
                case VirDomainEventType.VIR_DOMAIN_EVENT_STOPPED:
                    BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain stopped :" });
                    switch ((VirDomainEventStoppedDetailType)detail)
                    {
                        case VirDomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_CRASHED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain crashed\r\n" });
                            break;
                        case VirDomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_DESTROYED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain destroyed\r\n" });
                            break;
                        case VirDomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_FAILED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain failed\r\n" });
                            break;
                        case VirDomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_MIGRATED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain migrated\r\n" });
                            break;
                        case VirDomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_SAVED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain saved\r\n" });
                            break;
                        case VirDomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_SHUTDOWN:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain shutdown\r\n" });
                            break;
                    }
                    break;
                case VirDomainEventType.VIR_DOMAIN_EVENT_SUSPENDED:
                    BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain suspended :" });
                    switch ((VirDomainEventSuspendedDetailType)detail)
                    {
                        case VirDomainEventSuspendedDetailType.VIR_DOMAIN_EVENT_SUSPENDED_MIGRATED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain migrated\r\n" });
                            break;
                        case VirDomainEventSuspendedDetailType.VIR_DOMAIN_EVENT_SUSPENDED_PAUSED:
                            BeginInvoke(new AddEventDelegate(AddEvent), new object[] { " domain paused\r\n" });
                            break;
                    }
                    break;
            }
        }

        private void AddEvent(string eventMessage)
        {
            tbEvents.Text += eventMessage;
        }
    }
}
