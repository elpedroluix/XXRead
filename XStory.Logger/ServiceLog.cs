using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace XStory.Logger
{
    public class ServiceLog
    {
        public static async void Log(string label, string content, string source, DateTime date, LogType logType)
        {
            RepositoryLog _repoLog = new RepositoryLog();

            Log log = new Log
            {
                Id = Guid.NewGuid(),
                Label = label,
                Content = content,
                Source = source,
                Date = date,
                Type = logType.ToString(),
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
                return null;
            }
            
        }

    }
}
