using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public enum Statuts
    {
        [Display(Name = "En attente")]
        Attente,
        [Display(Name = "Accepté")]
        Accepte,
        [Display(Name = "Refusé")]
        Refuse,
        [Display(Name = "En suppression")]
        Suppression
    }

    public class Statut
    {
        public int StatutID { get; set; }
        [Display(Name = "Statut")]
        public Statuts Label { get; set; }

        public ICollection<Conge> Conges { get; set; }
    }
}
