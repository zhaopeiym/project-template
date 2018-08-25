using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProjectNameTemplate.Host.Controllers
{
    [Route("api/[controller]/[Action]")]
    public class HomeController : Controller
    {
        public string Index()
        {
            return "api";
        }
    }
}
