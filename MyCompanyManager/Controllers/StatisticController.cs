using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyCompanyManager.Controllers
{
    public class StatisticController : Controller
    {
        public String Index()
        {
            return "Statistic/Index";
        }

        public String Filtrate()
        {
            return "Statistic/Filtrate";
        }
    }
}