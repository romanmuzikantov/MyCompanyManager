﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyCompanyManager.Controllers
{
    public class HomeController : Controller
    {
        public String Index()
        {
            return "Home/Index";
        }
    }
}