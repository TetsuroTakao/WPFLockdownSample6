using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLockdownSample.Models;

namespace WPFLockdownSample.Features
{
    public partial class DataAccessLayer
    {
        public static DataAccessLayer Currentfacade { get; private set; }
        public FileInfo LogFile { get; private set; }
        const string appFilesPath = @"c:\WFPLockdownSample";
        DirectoryInfo logFolder = new DirectoryInfo(appFilesPath);
        public DataAccessLayer()
        {
            LogFile = new FileInfo(
                    appFilesPath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "logs.log"
                );
            Currentfacade = this;
        }
        public void AppendWriteLogs(List<Log> logs, bool reverse = true)
        {
            if(reverse) logs.Reverse();
            string jsonstring = string.Empty;
            logs.AddRange(ReadLogs());
            using (StreamWriter file = File.CreateText(LogFile.FullName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, logs);
            }
        }
        public List<Log> ReadLogs()
        {
            List<Log> logs = new List<Log>();
            string jsonstring = string.Empty;
            if (LogFile.Exists) jsonstring = File.ReadAllText(LogFile.FullName);
            if (!string.IsNullOrEmpty(jsonstring))
            {
                logs = JsonConvert.DeserializeObject<List<Log>>(jsonstring);
            }
            return logs;
        }
        public void Logging(string message)
        {
            List<Log> logs = new List<Log>();
            Log log = new Log() { LogType = LogType.Information, Message = message, OccurredTime = DateTime.Now, OperatorName = GetType().Name };
            logs.Add(log);
            AppendWriteLogs(logs);
        }
    }
}
