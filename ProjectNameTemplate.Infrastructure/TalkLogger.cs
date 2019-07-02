using Newtonsoft.Json;
using ProjectNameTemplate.Common.Helper;
using ProjectNameTemplate.Constant;
using ProjectNameTemplate.Core;
using ProjectNameTemplate.Infrastructure.Entitys;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectNameTemplate.Infrastructure
{
    public class TalkLogger : ITalkLogger
    {
        private ITalkSession session;
        public TalkLogger(ITalkSession talkSession)
        {
            session = talkSession;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        /// <param name="executionTime"></param>
        /// <param name="tag"></param>
        /// <param name="sendLogMail">是否发送日志邮件</param>
        public void Debug(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            // https://github.com/serilog/serilog-sinks-elasticsearch/issues/182         
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Debug("{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail("日志邮件【Debug】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", LogEventLevel.Debug);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        /// <param name="executionTime"></param>
        /// <param name="tag"></param>
        /// <param name="sendLogMail">是否发送日志邮件</param>
        public void Error(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Error("{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail("日志邮件【Error】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", LogEventLevel.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        /// <param name="executionTime"></param>
        /// <param name="tag"></param>
        /// <param name="sendLogMail">是否发送日志邮件</param>
        public void Error(Exception ex, string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Error(ex, "{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail("日志邮件【Error】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", LogEventLevel.Error);
        }

        public void Fatal(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Fatal("{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail("日志邮件【Fatal】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", LogEventLevel.Fatal);
        }

        public void Information(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Information("{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail("日志邮件【Information】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", LogEventLevel.Information);
        }

        public void Verbose(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Verbose("{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail("日志邮件【Verbose】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", LogEventLevel.Verbose);
        }

        public void Warning(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Warning("{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail("日志邮件【Warning】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", LogEventLevel.Warning);
        }

        public void Write(LogEventLevel level, string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true)
        {
            var model = GetLogModel(message, sql, executionTime, tag);
            Log.Logger.Write(level, "{@item} {@trackid} {@parenttrackid}", model, session.TrackId, session.ParentTrackId);
            if (sendLogMail) SendLogMail($"日志邮件【{level.ToString()}】", $"{JsonConvert.SerializeObject(model)} {session.TrackId}", level);
        }
        private LogModel GetLogModel(string message, string sql, decimal? executionTime, long? tag = null, bool sendLogMail = true)
        {
            return new LogModel()
            {
                Message = message,
                Time = DateTime.Now,
                ExecutionTime = executionTime,
                SQL = sql,
                UserId = session.UserId,
                UserName = session.UserName,
                Tag = tag,
                ControllerName  = session.ControllerName,
                ActionName = session.ActionName,
                RequestUrl = session.RequestUrl,
            };
        }

        /// <summary>
        /// 发送日志邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="level">日志等级</param>
        /// <param name="mandatoryMail">强制发送邮件，就算配置文件没有包含的日志等级</param>
        public void SendLogMail(string title, string content, LogEventLevel level, bool mandatoryMail = false)
        {
            var love = AppSetting.LogMailLevel.Split(",");
            if (love.Contains(level.ToString()) || (mandatoryMail && !love.Contains(level.ToString())))
                Task.Run(() =>
                {
                    if (!string.IsNullOrWhiteSpace(AppSetting.LogMailFrom))
                    {
                        MailHelper.SendMail(new SendMailModel()
                        {
                            Content = content,
                            Title = title,
                            MailInfo = new MailEntity()
                            {
                                MailFrom = AppSetting.LogMailFrom,
                                MailHost = AppSetting.LogMailHost,
                                MailPwd = AppSetting.LogMailPwd,
                                MailTo = AppSetting.LogMailTo
                            }
                        });
                    }
                });
        }

    }
}
