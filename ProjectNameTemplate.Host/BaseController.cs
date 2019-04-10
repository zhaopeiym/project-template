using Microsoft.AspNetCore.Mvc;
using ProjectNameTemplate.Core;

namespace ProjectNameTemplate.Host
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 日记记录器
        /// </summary>
        public ITalkLogger Logger { get; set; }
    }
}
