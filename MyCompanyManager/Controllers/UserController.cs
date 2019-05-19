using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCompanyManager.Models;

namespace MyCompanyManager.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Connect()
        {
            return View();
        }
    }
}