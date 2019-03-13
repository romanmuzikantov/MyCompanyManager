using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyCompanyManager.Models
{
    public class User
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Mot de passe")]
        public String Password { get; set; }
        public String Email { get; set; }
    }
}
