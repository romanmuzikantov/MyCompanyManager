using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.MyCompanyManagerViewModels
{
    public class CongeStatutType
    {
        public IEnumerable<Conge> Conges { get; set; }
        public IEnumerable<Statut> Statuts { get; set; }
        public IEnumerable<CongeType> CongeTypes { get; set; }
    }
}
