using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public class Absences
    {
        [DisplayName("A partir du")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Jusqu'au")]
        public DateTime DateFin { get; set; }
        [DisplayName("Employé")]
        public User Employe { get; set; }
        public String Raison { get; set; }
        [DataType(DataType.MultilineText)]
        public String Commentaire { get; set; }
    }
}
