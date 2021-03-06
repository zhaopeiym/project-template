﻿using Microsoft.AspNetCore.Mvc;
using ProjectNameTemplate.Host.Attributes;

namespace ProjectNameTemplate.Host.Controllers
{
    public class HomeController : BaseController
    {
        //默认路由
        //https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.1
        [HttpGet]
        [Route("/")]
        [Route("/Home")]
        [Route("/Home/Index")]
        [NoJsonResultAttribute]
        public IActionResult Index()
        {
            return View();
        }
    }
}
