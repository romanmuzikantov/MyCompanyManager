using MyCompanyManager.Models.MyCompanyManagerViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.CustomAttribute
{
    public class AnteriorToTodayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var conge = (Conge) validationContext.ObjectInstance;

            if(conge.DateDebut < DateTime.Now.Date)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "La date de début ne peut être antérieure à celle d'aujourd'hui !";
        }
    }
}
