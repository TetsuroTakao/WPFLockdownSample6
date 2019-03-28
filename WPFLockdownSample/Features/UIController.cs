using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFLockdownSample.Features
{
    public class UIController
    {
        public bool CollapseDesktopIcon()
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies", true);
            regkey.CreateSubKey("Explorer");
            regkey.Close();
            regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", true);
            regkey.SetValue("NoDesktop", 1, RegistryValueKind.DWord);
            regkey.Close();
            return true;
        }
    }
}
