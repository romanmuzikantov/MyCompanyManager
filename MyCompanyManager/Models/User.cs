using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public class User : IdentityUser
    {
        public bool IsResponsable { get; set; }
        public string FullName { get; set; }

        public int? EquipeID { get; set; }
        public int? SpecialiteID { get; set; }

        public ICollection<Conge> Conges { get; set; }
        public Equipe Equipe { get; set; }
        public Specialite Specialite { get; set; }
    }
}
