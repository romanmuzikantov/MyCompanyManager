using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.CustomAttribute
{
    public class EndAnteriorToStartAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var conge = (Conge) validationContext.ObjectInstance;

            if(conge.DateDebut > conge.DateFin)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "La date de fin ne peut être antérieure à la date de début !";
        }
    }
}
