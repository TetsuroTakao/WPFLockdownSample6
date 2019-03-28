using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLockdownSample.Models;

namespace WPFLockdownSample.Features
{
    public class ProcessController
    {
        public bool KillProcess()
        {
            bool result = false;
            DataAccessLayer.Currentfacade.Logging("kill processes start ...");
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                try
                {
                    if (p.ProcessName == "explore") p.Kill();
                    DataAccessLayer.Currentfacade.Logging("killing process : " + p.ProcessName);
                    result = true;
                }
                catch (Exception ex)
                {
                    DataAccessLayer.Currentfacade.Logging(string.Format("Fault killing process {0} : {1}", p.ProcessName,ex.Message));
                }
            }
            return result;
        }
        public bool SetLogonScript(string path)
        {
            if (string.IsNullOrEmpty(path)) path = Environment.CommandLine;
            string current = Environment.CurrentDirectory;
            DirectoryInfo d = new DirectoryInfo(current);
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            regkey.SetValue("", path);
            regkey.Close();
            return true;
        }
    }
}
