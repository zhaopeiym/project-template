using System;

namespace ProjectNameTemplate.WebApi.Attributes
{
    /// <summary>
    /// 标记了此特性的方法，返回结果不进行json包装
    /// </summary>
    public class NoJsonResultAttribute : Attribute
    {
    }
}
