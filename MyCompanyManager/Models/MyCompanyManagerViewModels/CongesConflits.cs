using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.MyCompanyManagerViewModels
{
    public class CongesConflits
    {
        public Conge Conge { get; set; }
        public IEnumerable<Conge> Conflits { get; set; }
        public int ConflitsCount { get; set; }
        public bool CanAccept { get; set; }
    }
}
