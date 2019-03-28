using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Principal;
using WPFLockdownSample.Models;

namespace WPFLockdownSample.Features
{
    public partial class DataAccessLayer
    {
        public string CurrentUserName
        {
            get { return WindowsIdentity.GetCurrent().Name; }
        }
        PrincipalContext context
        {
            get { return new PrincipalContext(ContextType.Machine); }
        }
        public List<string> GetUserGroupNames(string userName)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(userName)) userName = Environment.UserName;
            if (string.IsNullOrEmpty(userName)) return result;
            UserPrincipal user = UserPrincipal.FindByIdentity(context, Environment.UserName);
            PrincipalSearchResult<Principal> registeredGroups = user.GetGroups();
            foreach (Principal oResult in registeredGroups) result.Add(oResult.Name);
            return result;
        }
        public bool IsAdministrator(string userName)
        {
            if (string.IsNullOrEmpty(userName)) userName = Environment.UserName;
            if (string.IsNullOrEmpty(userName)) return false;
            var principals = GetUserGroupNames(userName);
            return (principals.Where(p => p == "Administrators").Count() > 0);
        }
        public bool IsRemoteDesktopUsers(string userName)
        {
            if (string.IsNullOrEmpty(userName)) userName = Environment.UserName;
            if (string.IsNullOrEmpty(userName)) return false;
            var principals = GetUserGroupNames(userName);
            return (principals.Where(p => p == "Remote Desktop Users").Count() > 0);
        }
        public List<Log> CreateUsers(List<Tuple<string, List<string>>> users)
        {
            List<Log> result = new List<Log>();
            Log log = null;
            UserPrincipal user = null;
            List<string> groups = null;
            try
            {
                foreach (var username in users)
                {
                    user = UserPrincipal.FindByIdentity(context, username.Item1);
                    if (user == null)
                    {
                        user = new UserPrincipal(context, username.Item1, username.Item1,true);
                    }
                    user.SetPassword(string.Empty);
                    user.DisplayName = username.Item1;
                    user.Name = username.Item1;
                    user.Description = string.Empty;
                    user.UserCannotChangePassword = false;
                    user.PasswordNeverExpires = true;
                    user.Save();
                    groups = username.Item2;
                    if (groups == null) groups = new List<string>();
                    if (groups.Where(g => g == "Users").Count() == 0) groups.Add("Users");
                    SetUserGroups(user, groups);
                }
                log = new Log();
                log.LogType = LogType.Information;
                log.Message = string.Format("Users created");
                log.OccurredTime = DateTime.Now;
                log.OperatorName = GetType().Name;
                result.Add(log);
                return result;
            }
            catch (Exception ex)
            {
                log = new Log();
                log.LogType = LogType.Information;
                log.Message = string.Format("Users creation fault reason : {0}", ex.Message);
                log.OccurredTime = DateTime.Now;
                log.OperatorName = GetType().Name;
                result.Add(log);
                return result;
            }
        }
        public List<Log> SetUserGroups(UserPrincipal user, List<string> groups)
        {
            List<Log> result = new List<Log>();
            Log log = new Log();
            try
            {
                foreach (string groupname in groups)
                {
                    log = new Log();
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupname);
                    group.Members.Add(user);
                    group.Save();
                    log.LogType = LogType.Information;
                    log.Message = string.Format("User[{0}] registered Group[{1}]", user.Name, groupname);
                    log.OccurredTime = DateTime.Now;
                    log.OperatorName = GetType().Name;
                    result.Add(log);
                }
                log.LogType = LogType.Information;
                log.Message = string.Format("Groups of user[{0}] registering complete", user.Name);
                log.OccurredTime = DateTime.Now;
                log.OperatorName = GetType().Name;
                result.Add(log);
                return result;
            }
            catch (Exception ex)
            {
                log.LogType = LogType.Information;
                log.Message = string.Format("User[{0}] creation fault reason : {1}", user.Name, ex.Message);
                log.OccurredTime = DateTime.Now;
                log.OperatorName = GetType().Name;
                result.Add(log);
                return result;
            }
        }
        public List<Log> DeleteUsers(List<string> users)
        {
            List<Log> result = new List<Log>();
            GroupPrincipal group = null;
            UserPrincipal user = null;
            Log log = null;
            try
            {
                foreach (string username in users)
                {
                    foreach (string groupname in GetUserGroupNames(username))
                    {
                        group = GroupPrincipal.FindByIdentity(context, groupname);
                        group.Delete();
                        log = new Log();
                        log.Message = string.Format("Group[{1}] of user[{0}] deleting complete", user.Name, groupname);
                        log.OccurredTime = DateTime.Now;
                        log.OperatorName = GetType().Name;
                        result.Add(log);
                    }
                    user = UserPrincipal.FindByIdentity(context, username);
                    user.Delete();
                    log = new Log();
                    log.Message = string.Format("User[{0}] deleting complete", user.Name, username);
                    log.OccurredTime = DateTime.Now;
                    log.OperatorName = GetType().Name;
                    result.Add(log);
                }
            }
            catch(Exception e)
            {
                log = new Log();
                log.Message = string.Format("User[{0}] deleting fault reason : ", e.Message);
                log.OccurredTime = DateTime.Now;
                log.OperatorName = GetType().Name;
                result.Add(log);
            }
            return result;
        }
        public bool RestrictForSpecificUser()
        {
            DataAccessLayer.Currentfacade.Logging(string.Format("User[{0}] is logon : ", CurrentUserName));
            if (CurrentUserName == "appOperator" || CurrentUserName == "appUser")
            {
                new ProcessController().KillProcess();
                DataAccessLayer.Currentfacade.Logging(string.Format("Set logon script for user[{0}] : ", CurrentUserName));
                new ProcessController().SetLogonScript(string.Empty);
                DataAccessLayer.Currentfacade.Logging(string.Format("Clear desktop of user[{0}] : ", CurrentUserName));
                new UIController().CollapseDesktopIcon();
            }
            return true;
        }
    }
}
