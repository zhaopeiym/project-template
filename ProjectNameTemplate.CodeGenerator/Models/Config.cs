using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectNameTemplate.CodeGenerator.Models
{
    /// <summary>
    /// 配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public string SqlConnection { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string SqlType { get; set; }
    }
}
