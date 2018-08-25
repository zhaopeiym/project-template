using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectNameTemplate.Host.Models;

namespace ProjectNameTemplate.Host.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet,HttpPost]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet, HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
