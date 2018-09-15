using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectNameTemplate.CodeGenerator.Models
{
    /// <summary>
    /// 数据库字段信息
    /// </summary>
    public class ColumnInfo
    {       
        public string Name { get; set; }
        public string Type { get; set; }
        public string Length { get; set; }
        public string IsNull { get; set; }
        public string Annotation { get; set; }
    }
}
