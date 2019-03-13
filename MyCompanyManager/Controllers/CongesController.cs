using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCompanyManager.Models;

namespace MyCompanyManager.Controllers
{
    public class CongesController : Controller
    {
        public String Index()
        {
            return "Conges/Index";
        }

        public ActionResult Apply()
        {
            Conges conges = new Conges();
            return View(conges);
        }

        public ActionResult Handle()
        {
            Conges conges = new Conges();
            return View(conges);
        }

        public String Accept()
        {
            return "Conges/Accept";
        }

        public String Show()
        {
            return "Conges/Show";
        }

        public String Filtrate()
        {
            return "Conges/Filtrate";
        }

        public String Dismiss()
        {
            return "Conges/Dismiss";
        }
    }
}