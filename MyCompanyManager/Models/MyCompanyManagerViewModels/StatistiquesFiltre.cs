using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.MyCompanyManagerViewModels
{
    public class StatistiquesFiltre
    {
        public Dictionary<string, int> Statistiques { get; set; }
        public string CurrentUserRole { get; set; }
    }
}
