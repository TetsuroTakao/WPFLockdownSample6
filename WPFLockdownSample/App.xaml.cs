using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFLockdownSample.Features;
using WPFLockdownSample.Models;

namespace WPFLockdownSample
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        DataAccessLayer dataAccessLayer = new DataAccessLayer();
        void App_Startup(object sender, StartupEventArgs e)
        {
            List<Log> logs = new List<Log>();
            Log log = new Log() { Message = "checkking app files ...", OccurredTime = DateTime.Now, OperatorName = typeof(App).Name, LogType = LogType.Information };
            logs.Add(log);
            if (dataAccessLayer.ReadLogs().Count == 0)
            {
                log = new Log() { Message = "this exesution is first time.", OccurredTime = DateTime.Now, OperatorName = typeof(App).Name, LogType = LogType.Information };
                logs.Add(log);
                List<string> groups = new List<string>();
                List<Tuple<string, List<string>>> users = new List<Tuple<string, List<string>>>();
                groups.Add("Users");
                groups.Add("Remote Desktop Users");
                groups.Add("Administrators");
                var user = new Tuple<string, List<string>>("superUser", groups);
                users.Add(user);
                groups = new List<string>();
                groups.Add("Users");
                groups.Add("Administrators");
                user = new Tuple<string, List<string>>("maintenanceOperator", groups);
                users.Add(user);
                groups = new List<string>();
                groups.Add("Users");
                groups.Add("Remote Desktop Users");
                user = new Tuple<string, List<string>>("appOperator", groups);
                users.Add(user);
                groups = new List<string>();
                groups.Add("Users");
                user = new Tuple<string, List<string>>("appUser", groups);
                users.Add(user);
                dataAccessLayer.CreateUsers(users);
                //And how to navigate use logon script
                //CoreApplication.Exit();
            }
            else
            {
                log = new Log() { Message = "this execution has run on this device more than twice.", OccurredTime = DateTime.Now, OperatorName = typeof(App).Name, LogType = LogType.Information };
                logs.Add(log);
                //[Desktop UI control] explains how to check current user
                //if (UserName == "maintenanceOperator") navigate to MaintenanceWindow
                //if (UserName == "appOperator") quiet this app. This account has specific automatic
                //run application when this account sign in.
                //if (UserName == "appUser") quiet this app. This account has specific automatic
                //run application when this account sign in.
            }
            log = new Log() { Message = "checkking app files is complete.", OccurredTime = DateTime.Now, OperatorName = typeof(App).Name, LogType = LogType.Information };
            logs.Add(log);
            dataAccessLayer.AppendWriteLogs(logs);
        }
    }
}
