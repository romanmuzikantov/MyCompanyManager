using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public enum Types
    {
        [Display(Name = "Congé")]
        Conge,
        Maladie,
        Absence
    }

    public class CongeType
    {
        public int CongeTypeID { get; set; }
        [Display(Name = "Type")]
        public Types Label { get; set; }

        public ICollection<Conge> Conges { get; set; }
    }
}
