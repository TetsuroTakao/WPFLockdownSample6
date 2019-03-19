using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPFLockdownSample
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {

            //[Logging and save the Log file to Microsoft Azure] explains how to send a log file
            //which is described at next step to Azure Blob Storage.
            //And explains creating model of log information for easy logging
            //LogModel message = new LogModel() { message = "check app files ..." };
            //Check existing of log file.
            //Logging logging = new Logging();
            //if (logging.ReadLogs().Count == 0)
            //{
            //    //[Local account creation] explains how to create these four accounts.
            //    //And how to navigate use logon script
            //    //CoreApplication.Exit();
            //}
            //else //if (logFile.Exists) check current user
            //{
            //    //[Desktop UI control] explains how to check current user
            //    //if (UserName == "maintenanceOperator") navigate to MaintenanceWindow
            //    //if (UserName == "appOperator") quiet this app. This account has specific automatic
            //run application when this account sign in.

            //    //if (UserName == "appUser") quiet this app. This account has specific automatic
            //run application when this account sign in.
            //}
        }
    }
}
