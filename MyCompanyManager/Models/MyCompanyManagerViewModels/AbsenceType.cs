using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.MyCompanyManagerViewModels
{
    public class AbsenceType
    {
        public IEnumerable<Conge> Absences { get; set; }
        public IEnumerable<CongeType> Types { get; set; }
    }
}
