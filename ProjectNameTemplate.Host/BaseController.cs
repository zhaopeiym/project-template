using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectNameTemplate.Host
{
    [Route("[controller]/[action]")]
    public class BaseController: Controller
    {
        /// <summary>
        /// 日记记录器
        /// </summary>
        public Serilog.ILogger Logger { get; set; }
    }
}
