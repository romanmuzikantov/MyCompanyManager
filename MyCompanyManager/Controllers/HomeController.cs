using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCompanyManager.Models;

namespace MyCompanyManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return LocalRedirect("~/Conges/Index");
            }

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}