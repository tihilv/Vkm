using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service.Calendar;
using Vkm.Library.Interfaces.Service.Mail;
using Exception = System.Exception;

namespace Vkm.Library.Service.Mail
{
    class OutlookService: IMailService, ICalendarService
    {
        static AppointmentInfo[] _emptyAppointments = new AppointmentInfo[0];
        
        private Application _application;
        private Items _mailItems;
        private MAPIFolder _calendarFolder;

        public Identifier Id => new Identifier("Vkm.OutlookService");
        public string Name => "Outlook Mail Service";

        public void Activate()
        {
            var proc = GetOutlookProc();
            if (proc == null)
            {
                Process.Start("outlook.exe");
            }
            else
            {
                Win32.SwitchToThisWindow(proc.MainWindowHandle, true);
            }
        }

        Process GetOutlookProc()
        {
            return Process.GetProcessesByName("OUTLOOK").Where(Win32.ProcessAccessibleForCurrentUser).FirstOrDefault();
        }

        public int? GetUnreadMessageCount()
        {
            try
            {
                if (EnsureOutlookConnected())
                {
                    return _mailItems.Count;
                }
            }
            catch (Exception)
            {
                _application = null;
                _mailItems = null;
            }
            
            return null;
        }

        AppointmentInfo GetAppointmentInfo(AppointmentItem item)
        {
            var start = item.Start;
            var end = item.End;
            var location = item.Location;
            var subject = item.Subject;
            var organiser = item.Organizer;

            return new AppointmentInfo(start, end, location, organiser, subject);

        }
        
        public AppointmentInfo[] GetCalendar(DateTime from, DateTime to)
        {
            if (EnsureOutlookConnected())
            {
                try
                {
                    List<AppointmentInfo> result = new List<AppointmentInfo>();

                    var app = GetAppointmentsInRange(_calendarFolder, from, to);
                    foreach (AppointmentItem item in app)
                    {
                        result.Add(GetAppointmentInfo(item));
                    }

                    return result.ToArray();
                }
                catch (Exception ex)
                {
                    _application = null;
                    _mailItems = null;
                }
            }

            return _emptyAppointments;
        }

        public bool TryShowAppointment(AppointmentInfo appointmentInfo)
        {
            var app = GetAppointmentsInRange(_calendarFolder, appointmentInfo.FromDateTime.AddMinutes(-5), appointmentInfo.FromDateTime.AddMinutes(5));
            foreach (AppointmentItem item in app)
            {
                if (GetAppointmentInfo(item) == appointmentInfo)
                {
                    item.Display(false);
                    return true;
                }
            }

            return false;
        }

        private Items GetAppointmentsInRange(MAPIFolder folder, DateTime startTime, DateTime endTime)
        {
            var filter = $"[Start] >= '{startTime:g}' and [Start] <= '{endTime:g}'";

            try
            {
                Items calItems = folder.Items;
                calItems.IncludeRecurrences = true;
                calItems.Sort("[Start]", Type.Missing);
                Items restrictItems = calItems.Restrict(filter);
                if (restrictItems.Count > 0)
                {
                    return restrictItems;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null; 
            }
        }
        
        bool IsOutlookRunning()
        {
            return GetOutlookProc() != null;
        }

        private object _outlookConnectLock = new object();

        bool EnsureOutlookConnected()
        {
            lock (_outlookConnectLock)
            {
                if (_application == null && IsOutlookRunning())
                {
                    _application = new Application();
                    var outlookNameSpace = _application.GetNamespace("MAPI");
                    var inbox = outlookNameSpace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
                    _mailItems = inbox.Items.Restrict("[Unread] = true");

                    _calendarFolder = outlookNameSpace.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);
                    return true;
                }
            }

            return _application != null;
        }
    }
}
