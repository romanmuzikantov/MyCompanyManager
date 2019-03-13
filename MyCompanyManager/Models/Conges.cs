using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models
{
    public class Conges
    {
        [DataType(DataType.Date)]
        [DisplayName("Date de début")]
        public DateTime DateDebut { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Date de fin")]
        public DateTime DateFin { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Commentaire")]
        public String Comment { get; set; }
    }
}
