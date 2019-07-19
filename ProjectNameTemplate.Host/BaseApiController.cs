using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ProjectNameTemplate.Core;

namespace ProjectNameTemplate.Host
{
    [EnableCors("AllowSameDomain")]
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class BaseApiController : ControllerBase
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
