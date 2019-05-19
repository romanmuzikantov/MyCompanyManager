using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCompanyManager.Data;
using MyCompanyManager.Models;
using MyCompanyManager.Models.MyCompanyManagerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Controllers
{
    [Authorize(Roles = "Directeur")]
    public class StatistiquesController : Controller
    {
        private readonly MyCompanyContext _context;

        public StatistiquesController(MyCompanyContext context)
        {
            _context = context;
        }

        public IActionResult Index(string year, string month)
        {
            var monthArray = new string[] { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"};
            var conges = from c in _context.Conges select c;
            var statistiquesPerYear = new Dictionary<string, int>()
                {
                    {"Janvier", 0},
                    {"Février", 0},
                    {"Mars", 0},
                    {"Avril", 0},
                    {"Mai", 0},
                    {"Juin", 0},
                    {"Juillet", 0},
                    {"Août", 0},
                    {"Septembre", 0},
                    {"Octobre", 0},
                    {"Novembre", 0},
                    {"Décembre", 0}
                };
            var statistiquesPerMonth = new Dictionary<string, int>()
                {
                    {"Congés", 0},
                    {"Maladies", 0}
                };
            var count = 0;
            var viewModel = new StatistiquesFiltre();
            ViewBag.StatsAim = "globalité";

            if(year != null && year != "/")
            {
                conges = conges.Where(c => c.DateDebut.Year.ToString() == year || c.DateFin.Year.ToString() == year);
                ViewBag.StatsAim = year;
            }

            if(month != null && month != "/")
            {
                conges = conges.Where(c => c.DateDebut.Month.ToString() == month || c.DateFin.Month.ToString() == month).Include(t => t.CongeType);
                ViewBag.StatsAim = monthArray[int.Parse(month) - 1] + " " + ViewBag.StatsAim;
                ViewBag.CurrentChart = "month";
            }

            if(month == null || month == "/")
            {
                count = conges.Count();

                foreach(var conge in conges)
                {
                    if(statistiquesPerYear.ContainsKey(monthArray[conge.DateDebut.Month - 1]) && (year == null || year == "/" || conge.DateDebut.Year.ToString() == year))
                    {
                        statistiquesPerYear[monthArray[conge.DateDebut.Month - 1]]++;
                    }
                    if(conge.DateDebut.Month != conge.DateFin.Month)
                    {
                        if(statistiquesPerYear.ContainsKey(monthArray[conge.DateFin.Month - 1]) && (year == null || year == "/" || conge.DateFin.Year.ToString() == year))
                        {
                            statistiquesPerYear[monthArray[conge.DateFin.Month - 1]]++;
                            count++;
                        }
                    }
                }

                viewModel.Statistiques = statistiquesPerYear;
            }
            else
            {
                count = conges.Count();

                foreach(var conge in conges)
                {
                    if(conge.CongeType.Label == Types.Conge)
                    {
                        statistiquesPerMonth["Congés"]++;
                    }
                    if(conge.CongeType.Label == Types.Maladie)
                    {
                        statistiquesPerMonth["Maladies"]++;
                    }
                }

                viewModel.Statistiques = statistiquesPerMonth;
            }

            foreach(var key in viewModel.Statistiques.Keys.ToList())
            {
                viewModel.Statistiques[key] = Average(count, viewModel.Statistiques[key]);
            }
            
            return View(viewModel);
        }

        private int Average(int count, int startValue)
        {
            if(startValue > 0)
            {
                var result = ((float) startValue / count) * 100;
                return (int) result;
            }
            else
            {
                return 0;
            }
        }
    }
}
