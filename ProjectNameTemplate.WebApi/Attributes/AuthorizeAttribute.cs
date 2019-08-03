using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectNameTemplate.WebApi.Filters
{
    /// <summary>
    /// 权限验证特性
    /// </summary>    
    public class AuthorizeAttribute : Attribute
    {
        /// <summary>
        /// 权限集合
        /// </summary>
        public string[] Permissions;

        public AuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }
    }
}
