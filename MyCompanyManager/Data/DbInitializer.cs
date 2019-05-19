using Microsoft.AspNetCore.Identity;
using MyCompanyManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MyCompanyManager.Data
{
    public class DbInitializer
    {
        public static void Initialize(MyCompanyContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if(context.Members.Any())
            {
                return;
            }

            CreateRoles(roleManager);
            context.SaveChanges();

            var equipes = new Equipe[]
            {
                new Equipe{Label = "Infrastructure"},
                new Equipe{Label = "Developpement"}
            };
            for(int i = 0; i < equipes.Length; i++)
            {
                context.Equipes.Add(equipes[i]);
            }
            context.SaveChanges();

            var specialites = new Specialite[]
            {
                new Specialite{Label = "OS", EquipeID = equipes.Single( e => e.Label == "Infrastructure").EquipeID},
                new Specialite{Label = "Reseau", EquipeID = equipes.Single( e => e.Label == "Infrastructure").EquipeID},
                new Specialite{Label = "Securite", EquipeID = equipes.Single( e => e.Label == "Infrastructure").EquipeID},
                new Specialite{Label = "Web", EquipeID = equipes.Single( e => e.Label == "Developpement").EquipeID},
                new Specialite{Label = "Mainframe", EquipeID = equipes.Single( e => e.Label == "Developpement").EquipeID},
                new Specialite{Label = "Client lourd", EquipeID = equipes.Single( e => e.Label == "Developpement").EquipeID}
            };
            foreach(Specialite s in specialites)
            {
                context.Specialites.Add(s);
            }
            context.SaveChanges();

            CreateDirecteur(userManager);
            context.SaveChanges();

            CreateResponsables(userManager, equipes);
            context.SaveChanges();
            
            CreateCollaborateur(userManager, equipes, specialites);
            context.SaveChanges();

            var congesTypes = new CongeType[]
            {
                new CongeType{Label = Types.Absence},
                new CongeType{Label = Types.Conge},
                new CongeType{Label = Types.Maladie}
            };
            foreach(CongeType t in congesTypes)
            {
                context.CongeTypes.Add(t);
            }
            context.SaveChanges();

            var statuts = new Statut[]
            {
                new Statut{Label = Statuts.Accepte},
                new Statut{Label = Statuts.Attente},
                new Statut{Label = Statuts.Refuse},
                new Statut{Label = Statuts.Suppression}
            };
            foreach(Statut s in statuts)
            {
                context.Statuts.Add(s);
            }
            context.SaveChanges();

            var colla1 = userManager.FindByNameAsync("c.infrastructure_os1@mycompany.com").Result;
            var colla2 = userManager.FindByNameAsync("c.infrastructure_reseau1@mycompany.com").Result;
            var colla3 = userManager.FindByNameAsync("c.developpement_clientlourd3@mycompany.com").Result;
            var colla4 = userManager.FindByNameAsync("c.infrastructure_securite3@mycompany.com").Result;
            var colla5 = userManager.FindByNameAsync("c.developpement_mainframe2@mycompany.com").Result;
            var colla6 = userManager.FindByNameAsync("c.infrastructure_securite2@mycompany.com").Result;
            var colla7 = userManager.FindByNameAsync("c.developpement_web3@mycompany.com").Result;
            var colla8 = userManager.FindByNameAsync("c.infrastructure_reseau2@mycompany.com").Result;
            var colla9 = userManager.FindByNameAsync("c.infrastructure_reseau3@mycompany.com").Result;

            var conges = new Conge[]
            {
                // CONGES PORTANT SUR UN JOUR
                new Conge{EnvoiDate = DateTime.Parse("15/05/2019"), DateDebut = DateTime.Parse("26/07/2019"), DateFin = DateTime.Parse("26/07/2019"), CollaborateurID = colla1.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("20/04/2019"), DateDebut = DateTime.Parse("26/07/2019"), DateFin = DateTime.Parse("26/07/2019"), CollaborateurID = colla2.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("03/05/2019"), DateDebut = DateTime.Parse("26/07/2019"), DateFin = DateTime.Parse("26/07/2019"), CollaborateurID = colla3.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                // DEMANDES DE CONGES REFUSEES
                new Conge{EnvoiDate = DateTime.Parse("17/05/2019"), DateDebut = DateTime.Parse("15/07/2019"), DateFin = DateTime.Parse("20/07/2019"), CollaborateurID = colla4.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Refuse).StatutID, Commentaire = "Wow, tu prends un peu trop de congé à mon gout, tu vas te calmer tout de suite mon p'tit gars !"},
                new Conge{EnvoiDate = DateTime.Parse("12/05/2019"), DateDebut = DateTime.Parse("21/11/2019"), DateFin = DateTime.Parse("02/12/2019"), CollaborateurID = colla2.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Refuse).StatutID, Commentaire = "Trop d'absences pendant cette période."},
                new Conge{EnvoiDate = DateTime.Parse("10/05/2019"), DateDebut = DateTime.Parse("04/10/2019"), DateFin = DateTime.Parse("14/10/2019"), CollaborateurID = colla5.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Refuse).StatutID, Commentaire = "Pas d'inspiration, ton congé est refusé !"},
                // DEMANDES DE CONGES EN ATTENTE
                new Conge{EnvoiDate = DateTime.Parse("20/05/2019"), DateDebut = DateTime.Parse("12/09/2019"), DateFin = DateTime.Parse("16/09/2019"), CollaborateurID = colla4.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Attente).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("18/05/2019"), DateDebut = DateTime.Parse("15/09/2019"), DateFin = DateTime.Parse("20/09/2019"), CollaborateurID = colla6.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Attente).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("26/04/2019"), DateDebut = DateTime.Parse("03/08/2019"), DateFin = DateTime.Parse("13/08/2019"), CollaborateurID = colla7.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Attente).StatutID},
                // DEMANDES DE CONGES PORTANT SUR 15 JOURS
                new Conge{EnvoiDate = DateTime.Parse("11/05/2019"), DateDebut = DateTime.Parse("15/10/2019"), DateFin = DateTime.Parse("29/10/2019"), CollaborateurID = colla5.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("09/05/2019"), DateDebut = DateTime.Parse("01/09/2019"), DateFin = DateTime.Parse("15/09/2019"), CollaborateurID = colla1.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Attente).StatutID},
                // ABSENCES MALADIE
                new Conge{EnvoiDate = DateTime.Parse("20/05/2019"), DateDebut = DateTime.Parse("27/06/2019"), DateFin = DateTime.Parse("28/06/2019"), CollaborateurID = colla7.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Maladie).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Attente).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("21/05/2019"), DateDebut = DateTime.Parse("01/07/2019"), DateFin = DateTime.Parse("01/07/2019"), CollaborateurID = colla1.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Maladie).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Attente).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("22/05/2019"), DateDebut = DateTime.Parse("01/07/2019"), DateFin = DateTime.Parse("05/07/2019"), CollaborateurID = colla3.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Maladie).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("23/05/2019"), DateDebut = DateTime.Parse("02/07/2019"), DateFin = DateTime.Parse("03/07/2019"), CollaborateurID = colla6.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Maladie).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                // CONGES ACCEPTES ET PASSES
                new Conge{EnvoiDate = DateTime.Parse("20/03/2019"), DateDebut = DateTime.Parse("02/04/2019"), DateFin = DateTime.Parse("10/04/2019"), CollaborateurID = colla1.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("14/02/2019"), DateDebut = DateTime.Parse("20/03/2019"), DateFin = DateTime.Parse("25/03/2019"), CollaborateurID = colla7.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Maladie).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                // CREATION D'UN CONFLIT
                new Conge{EnvoiDate = DateTime.Parse("20/05/2019"), DateDebut = DateTime.Parse("02/01/2020"), DateFin = DateTime.Parse("20/01/2020"), CollaborateurID = colla2.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Attente).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("20/05/2019"), DateDebut = DateTime.Parse("15/01/2020"), DateFin = DateTime.Parse("20/01/2020"), CollaborateurID = colla8.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Conge).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID},
                new Conge{EnvoiDate = DateTime.Parse("20/05/2019"), DateDebut = DateTime.Parse("25/12/2019"), DateFin = DateTime.Parse("15/01/2020"), CollaborateurID = colla9.Id, CongeTypeID = congesTypes.Single( t => t.Label == Types.Maladie).CongeTypeID, StatutID = statuts.Single( s => s.Label == Statuts.Accepte).StatutID}
            };
            foreach(Conge c in conges)
            {
                context.Conges.Add(c);
            }
            context.SaveChanges();
        }

        private static void CreateDirecteur(UserManager<User> userManager)
        {
            User directeur = new User
            {
                UserName = "directeur@mycompany.com",
                Email = "directeur@mycompany.com",
                FullName = "Sam Pique",
                IsResponsable = false
            };

            IdentityResult resultInfra = userManager.CreateAsync(directeur, "D.directeur1").Result;

            if (resultInfra.Succeeded)
            {
                userManager.AddToRoleAsync(directeur, "Directeur").Wait();
            }
        }

        private static void CreateResponsables(UserManager<User> userManager, Equipe[] equipes)
        {
            // RESPONSABLE INFRASTRUCTURE
            User responsableTechnique = new User
            {
                UserName = "r.infrastructure@mycompany.com",
                Email = "r.infrastructure@mycompany.com",
                EquipeID = equipes.Single(s => s.Label == "Infrastructure").EquipeID,
                FullName = "Georges Delajungle",
                IsResponsable = true
            };

            IdentityResult resultInfra = userManager.CreateAsync(responsableTechnique, "R.infrastructure1").Result;

            if (resultInfra.Succeeded)
            {
                userManager.AddToRoleAsync(responsableTechnique, "Responsable").Wait();
            }

            // RESPONSABLE DEVELOPPEMENT
            User responsableDeveloppement = new User
            {
                UserName = "r.developpement@mycompany.com",
                Email = "r.developpement@mycompany.com",
                EquipeID = equipes.Single(s => s.Label == "Developpement").EquipeID,
                FullName = "John Doe",
                IsResponsable = true
            };

            IdentityResult resultDev = userManager.CreateAsync(responsableDeveloppement, "R.developpement1").Result;

            if (resultDev.Succeeded)
            {
                userManager.AddToRoleAsync(responsableDeveloppement, "Responsable").Wait();
            }
        }

        private static void CreateCollaborateur(UserManager<User> userManager, Equipe[] equipes, Specialite[] specialites)
        {
            // INFRASTRUCTURE : OS
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_os1@mycompany.com", "C.technique.os1", "OS", "Jean Nemare");
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_os2@mycompany.com", "C.technique.os2", "OS", "Pierre Durand");
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_os3@mycompany.com", "C.technique.os3", "OS", "Barrack Afritt");
            // INFRASTRUCTURE : RESEAU
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_reseau1@mycompany.com", "C.technique.reseau1", "Reseau", "Elie Coptere");
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_reseau2@mycompany.com", "C.technique.reseau2", "Reseau", "Ella Lapeche");
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_reseau3@mycompany.com", "C.technique.reseau3", "Reseau", "Geoffroy Denledo");
            // INFRASTRUCTURE : SECURITE
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_securite1@mycompany.com", "C.technique.securite1", "Securite", "Guy Yiotine");
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_securite2@mycompany.com", "C.technique.securite2", "Securite", "Henri Cochet");
            MakeCollaborateur(userManager, equipes, specialites, "c.infrastructure_securite3@mycompany.com", "C.technique.securite3", "Securite", "Jacques Celert");
            // DEVELOPPEMENT : WEB
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_web1@mycompany.com", "C.developpement.web1", "Web", "Jerry Kan");
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_web2@mycompany.com", "C.developpement.web2", "Web", "Lara Masse");
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_web3@mycompany.com", "C.developpement.web3", "Web", "Larry Golade");
            // DEVELOPPEMENT : MAINFRAME
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_mainframe1@mycompany.com", "C.developpement.mainframe1", "Mainframe", "Laurent Houtan");
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_mainframe2@mycompany.com", "C.developpement.mainframe2", "Mainframe", "Line Ottension");
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_mainframe3@mycompany.com", "C.developpement.mainframe3", "Mainframe", "Mick Emmaus");
            // DEVELOPPEMENT : CLIENT LOURD
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_clientlourd1@mycompany.com", "C.developpement.clientlourd1", "Client lourd", "Pierre Kiroul");
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_clientlourd2@mycompany.com", "C.developpement.clientlourd2", "Client lourd", "Sacha Touille");
            MakeCollaborateur(userManager, equipes, specialites, "c.developpement_clientlourd3@mycompany.com", "C.developpement.clientlourd3", "Client lourd", "Otto Graf");
        }

        private static void MakeCollaborateur(UserManager<User> userManager, Equipe[] equipes, Specialite[] specialites, string email, string mdp, string specialite, string name)
        {
            User collaborateur = new User
            {
                UserName = email,
                Email = email,
                SpecialiteID = specialites.Single( s => s.Label == specialite).SpecialiteID,
                EquipeID = specialites.Single( s => s.Label == specialite).EquipeID,
                FullName = name,
                IsResponsable = false
            };

            IdentityResult result = userManager.CreateAsync(collaborateur, mdp).Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(collaborateur, "Collaborateur").Wait();
            }
        }

        private static void CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            IdentityRole collaborateur = new IdentityRole
            {
                Name = "Collaborateur"
            };
            roleManager.CreateAsync(collaborateur).Wait();

            IdentityRole manager = new IdentityRole
            {
                Name = "Responsable"
            };
            roleManager.CreateAsync(manager).Wait();

            IdentityRole ceo = new IdentityRole
            {
                Name = "Directeur"
            };
            roleManager.CreateAsync(ceo).Wait();
        }
    }
}
