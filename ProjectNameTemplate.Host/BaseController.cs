using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ProjectNameTemplate.Core;

namespace ProjectNameTemplate.Host
{

    /// <summary>
    /// 如果不标记Route 则swagger生成不了
    /// </summary>
    [EnableCors("AllowSameDomain")]
    [Route("[controller]/[Action]")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Session 属性注入
        /// </summary>
        public ITalkSession Session { get; set; }
        /// <summary>
        /// 日记记录器
        /// </summary>
        public ITalkLogger Logger { get; set; }
    }
}
