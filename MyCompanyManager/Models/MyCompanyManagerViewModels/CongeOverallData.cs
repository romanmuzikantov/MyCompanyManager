using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompanyManager.Models.MyCompanyManagerViewModels
{
    public class CongeOverallData
    {
        public string CurrentUserRole { get; set; }
        public IEnumerable<CongeType> CongeTypes { get; set; }
        public IEnumerable<Statut> Statuts { get; set; }

        // COLLABORATEUR
        public IEnumerable<Conge> Conges { get; set; }
        public IEnumerable<Conge> Absences { get; set; }

        public int QuotaConges { get; set; }
        public int QuotaMaladies { get; set; }

        // RESPONSABLE
        public IList<CongesConflits> CongeEnAttente { get; set; }
        public IList<CongesConflits> CongeModel { get; set; }

        public int NbrCongeAttente { get; set; }
        public int NbrSuppressionAttente { get; set; }

        // DIRECTEUR
        public IList<Conge> TodayAbsences { get; set;}
        public IList<Conge> IncomingAbsences { get; set; }

        public int NbrCollaborateurAbsent { get; set; }
    }
}
