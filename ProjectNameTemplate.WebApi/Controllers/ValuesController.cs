using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProjectNameTemplate.WebApi.Controllers
{ 
    public class ValuesController : BaseApiController
    {
        // GET api/values
        [HttpGet]
        [Route("/")]
        [Route("/api")]
        [Route("/api/values")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "欢迎使用项目模板!!!" };
        }
    }
}
