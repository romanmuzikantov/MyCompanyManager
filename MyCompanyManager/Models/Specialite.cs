using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public class Specialite
    {
        public int SpecialiteID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Spécialité")]
        public string Label { get; set; }

        public int EquipeID { get; set; }

        public Equipe Equipe { get; set; }
        public ICollection<User> Collaborateurs { get; set; }
    }
}
