using MyCompanyManager.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.MyCompanyManagerViewModels
{
    public class CongeFormulaire
    {
        public Conge Conge { get; set; }
        public IEnumerable<CongeType> CongeTypes { get; set; }
    }
}
