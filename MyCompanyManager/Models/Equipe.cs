using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public class Equipe
    {
        public int EquipeID { get; set; }
        [Required]
        [StringLength(50)]
        public string Label { get; set; }

        public ICollection<User> Members { get; set; }
        public ICollection<Specialite> Specialites { get; set; }
    }
}
