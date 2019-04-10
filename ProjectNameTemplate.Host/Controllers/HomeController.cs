﻿using System;
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
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
