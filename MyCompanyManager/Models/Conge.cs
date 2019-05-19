using MyCompanyManager.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public class Conge
    {
        public int CongeID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EnvoiDate { get; set; }
        [Required]
        [AnteriorToToday]
        [DataType(DataType.Date)]
        [Display(Name = "Date de début")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateDebut { get; set; }
        [Required]
        [EndAnteriorToStart]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date de fin")]
        public DateTime DateFin { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(200, MinimumLength = 10)]
        [DisplayFormat(NullDisplayText = "/")]
        public string Commentaire { get; set; }

        public string CollaborateurID { get; set; }
        [Required]
        [Display(Name = "Type")]
        public int CongeTypeID { get; set; }
        public int StatutID { get; set; }

        public User Collaborateur { get; set; }
        public CongeType CongeType { get; set; }
        public Statut Statut { get; set; }
    }
}
