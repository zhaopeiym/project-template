using Serilog.Events;
using System;
using Talk;

namespace ProjectNameTemplate.Core
{
    public interface ITalkLogger : IScopedDependency
    {
        /// <summary>
        /// 详细日志
        /// </summary>
        void Verbose(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);
        /// <summary>
        /// 调试跟踪
        /// </summary>
        void Debug(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        /// <param name="executionTime"></param>
        /// <param name="tag"></param>
        void Information(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);
        /// <summary>
        /// 警告
        /// </summary>
        void Warning(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        /// <param name="executionTime"></param>
        /// <param name="tag"></param>
        /// <param name="sendLogMail">是否发送日志邮件</param>
        void Error(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        /// <param name="executionTime"></param>
        /// <param name="tag"></param>
        /// <param name="sendLogMail">是否发送日志邮件</param>
        void Error(Exception ex, string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);
        /// <summary>
        /// 致命
        /// </summary>
        void Fatal(string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);

        void Write(LogEventLevel level, string message, string sql = null, decimal? executionTime = null, long? tag = null, bool sendLogMail = true);
        /// <summary>
        /// 发送日志邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="level">日志等级</param>
        /// <param name="mandatoryMail">强制发送邮件，就算配置文件没有包含的日志等级</param>
        void SendLogMail(string title, string content, LogEventLevel level, bool mandatoryMail = false);

    }
}
