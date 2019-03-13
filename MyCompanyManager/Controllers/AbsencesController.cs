using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCompanyManager.Models;

namespace MyCompanyManager.Controllers
{
    public class AbsencesController : Controller
    {
        public String Index()
        {
            return "Absences/Index";
        }

        public String Show()
        {
            return "Absences/Show";
        }

        public String Filtrate()
        {
            return "Absences/Filtrate";
        }

        public ActionResult Assign()
        {
            Absences absences = new Absences();
            return View(absences);
        }
    }
}