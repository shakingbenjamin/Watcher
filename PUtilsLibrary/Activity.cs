using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Mime;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Web;
using System.Diagnostics;

namespace WatcherLibrary
{
    public class StartUp
    {
        private string hostname;
        private string dateTimeStmp;
        private static System.Timers.Timer ticker;
        public void GoTimeAppend()
        {
            hostname = System.Environment.MachineName;
            ticker = new System.Timers.Timer(10000);
            dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var aw = new ActiveWindow();
            var jo = new Activity
            {
                host = hostname,
                dt = dateTimeStmp,
                app = "***",
                action = "Starting",
                info = "***"
            };
            aw.ToWebService(jo);
            ticker.Elapsed += new ElapsedEventHandler(aw.OnTimeAppend);
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(aw.SystemEvents_SessionSwitchAppend);
            ticker.Enabled = true;
            ticker.Interval = 2000;
        }
        public void GoTimeWeb()
        {
            ticker = new System.Timers.Timer(10000);
            var aw = new ActiveWindow();
            ticker.Elapsed += new ElapsedEventHandler(aw.OnTimeWeb);
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(aw.SystemEvents_SessionSwitchWeb);
            ticker.Enabled = true;
            ticker.Interval = 2000;
        }
        public void StopTime()
        {
            dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            hostname = System.Environment.MachineName;
            var jo = new Activity
            {
                host = hostname,
                dt = dateTimeStmp,
                app = "***",
                action = "Stopping",
                info = "***"
            };
            ticker.Stop();
        }
    }
    public class ActiveWindow
    {
        private string existingTitle;
        private string newTitle;
        private string appName;
        private string existingUser;
        private string newUser;
        private string dateTimeStmp;
        private string hostname;
        private string fileName = @"C:\GitLocal\TestArea.txt";
        private string url = "somewheretoreceiveJson";

        public void ToWebService(Activity jo)
        {
            string json = new JavaScriptSerializer().Serialize(jo);
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("Content-Type", "application/json");
                json = web.UploadString(url, json);
            }
        }
        public void OnTimeWeb(object source, ElapsedEventArgs e)
        {
            newTitle = GetActiveWindowTitle();
            appName = GetForegroundProcessName();
            newUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            hostname = Environment.MachineName;

            if (newUser != existingUser)
            {
                dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                existingUser = newUser;
                var jo = new Activity
                {
                    host = hostname,
                    dt = dateTimeStmp,
                    app = appName,
                    action = "Login",
                    info = newUser
                };
                ToWebService(jo);
            }

            if (newTitle != existingTitle)
            {
                dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (!string.IsNullOrEmpty(newTitle))
                    newTitle = newTitle.Replace(",", "");
                existingTitle = newTitle;
                var jo = new Activity();

                if (newTitle != "InActive")
                {
                    appName = GetForegroundProcessName();
                    
                    jo.host = hostname;
                    jo.dt = dateTimeStmp;
                    jo.app = appName;
                    jo.action = "Action";
                    jo.info = newTitle;
                }

                if (newTitle == "InActive")
                {
                    appName = GetForegroundProcessName();

                    jo.host = hostname;
                    jo.dt = dateTimeStmp;
                    jo.app = appName;
                    jo.action = "InActive";
                    jo.info = newTitle;
                }
                ToWebService(jo);
            }
        }
        public void OnTimeAppend(object source, ElapsedEventArgs e)
        {
            if (!File.Exists(fileName))
            {
                string headers = string.Join(",", "Hostname", "DateTime", "Type", "Info", Environment.NewLine);
                File.AppendAllText(fileName, headers);
            }

            newTitle = GetActiveWindowTitle();
            appName = GetForegroundProcessName();
            newUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            hostname = System.Environment.MachineName;

            if (newUser != existingUser)
            {
                dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      
                existingUser = newUser;
                if(File.Exists(fileName))
                {
                    
                    string csvLogin = string.Join(",", hostname, dateTimeStmp, appName, "Login", newUser, Environment.NewLine);
                    File.AppendAllText(fileName, csvLogin);
                }
            }

            if (newTitle != existingTitle)
            {
                dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if(!string.IsNullOrEmpty(newTitle))
                    newTitle = newTitle.Replace(",","");
                existingTitle = newTitle;

                if(File.Exists(fileName))
                {
                    if (newTitle != "InActive")
                    {
                        appName = GetForegroundProcessName();
                        string csvAction = string.Join(",", hostname, dateTimeStmp, appName, "Action", newTitle, Environment.NewLine);
                        File.AppendAllText(fileName, csvAction);
                    }
                    if (newTitle == "InActive")
                    {
                        appName = GetForegroundProcessName();
                        string csvAction = string.Join(",", hostname, dateTimeStmp, appName, newTitle, newTitle, Environment.NewLine);
                        File.AppendAllText(fileName, csvAction);
                    }
                }   
            }
        }
        public void SystemEvents_SessionSwitchWeb(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                var jo = new Activity
                {
                    host = hostname,
                    dt = dateTimeStmp,
                    app = appName,
                    action = "Locked",
                    info = newUser
                };

                ToWebService(jo);
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                var jo = new Activity
                {
                    host = hostname,
                    dt = dateTimeStmp,
                    app = appName,
                    action = "Unlocked",
                    info = newUser
                };
                ToWebService(jo);
            }
        }
        public void SystemEvents_SessionSwitchAppend(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            dateTimeStmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                if (File.Exists(fileName))
                {
                    string csvLock = string.Join(",",hostname, dateTimeStmp, appName, "Action", "Locked", Environment.NewLine);
                    File.AppendAllText(fileName, csvLock);
                }  
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                if (File.Exists(fileName))
                {
                    string csvUnlock = string.Join(",",hostname, dateTimeStmp, appName, "Action", "Unlocked", Environment.NewLine);
                    File.AppendAllText(fileName, csvUnlock);
                }  
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            if(handle != null)
            {
                if (GetWindowText(handle, Buff, nChars) > 0)
                {
                    return Buff.ToString();
                }
            }
            
            return "InActive";
        
        }

        // The GetWindowThreadProcessId function retrieves the identifier of the thread
        // that created the specified window and, optionally, the identifier of the
        // process that created the window.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private string GetForegroundProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();

            // The foreground window can be NULL in certain circumstances, 
            // such as when a window is losing activation.
            if (hwnd == null)
                return "Unknown";

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.Id == pid)
                    return p.ProcessName;
            }
            return "Unknown";
        }
    }
    [DataContract]
    public class Activity
    {
        [DataMember]
        public string host { get; set; }
        [DataMember]
        public string dt { get; set; }
        [DataMember]
        public string app { get; set; }
        [DataMember]
        public string action { get; set; }
        [DataMember]
        public string info { get; set; }
    }
}
