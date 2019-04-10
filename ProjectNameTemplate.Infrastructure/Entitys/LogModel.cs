using System;

namespace ProjectNameTemplate.Infrastructure.Entitys
{
    public class LogModel
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string SQL { get; set; }
        /// <summary>
        /// 执行时长（秒）
        /// </summary>
        public decimal? ExecutionTime { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public long? Tag { get; set; }
    }
}
