using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static SQLite.SQLite3;

namespace XStory.Logger
{
    public class ServiceLog
    {
        public static async void Log(string label, string content, string source = null)
        {
            RepositoryLog _repoLog = new RepositoryLog();

            Log log = new Log()
            {
                Id = Guid.NewGuid(),
                Label = label,
                Content = content,
                Source = source,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.SSS"),
                Type = LogType.Info.ToString(),
            };

            await _repoLog.InsertLog(log);
        }

        public static async void Error(Exception exception)
        {
            RepositoryLog _repoLog = new RepositoryLog();

            Log log = new Log()
            {
                Id = Guid.NewGuid(),
                Content = exception.Message,
                StackTrace = exception.StackTrace,
                Source = exception.Source,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Type = LogType.Error.ToString()
            };

            await _repoLog.InsertLog(log);
        }

        public static async Task<List<Log>> GetLogs()
        {
            RepositoryLog _repoLog = new RepositoryLog();
            try
            {
                return await _repoLog.GetLogs();
            }
            catch (Exception ex)
            {
                Error(ex);
                return null;
            }

        }

    }
}
