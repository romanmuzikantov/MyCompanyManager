using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCompanyManager.Data;
using MyCompanyManager.Models;
using MyCompanyManager.Models.MyCompanyManagerViewModels;

namespace MyCompanyManager.Controllers
{
    [Authorize]
    public class CongesController : Controller
    {
        private readonly MyCompanyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public CongesController(MyCompanyContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var query = from s in _context.Conges select s;
            var quotaConges = 0;
            var quotaMaladies = 0;
            var currentUser = _userManager.GetUserAsync(HttpContext.User).Result;
            var currentUserRole = _userManager.GetRolesAsync(currentUser).Result;
            var viewModel = new CongeOverallData();

            if (currentUserRole.Contains("Collaborateur"))
            {
                var conges = await query
                .Where(c => c.CollaborateurID == currentUser.Id && (c.DateDebut.Year == DateTime.Now.Year || c.DateFin.Year == DateTime.Now.Year))
                .Include(s => s.Statut)
                .Where(s => s.Statut.Label == Statuts.Accepte)
                .Include(t => t.CongeType)
                .Where(t => t.CongeType.Label == Types.Conge)
                .AsNoTracking()
                .ToListAsync();

                var maladies = await query
                    .Where(c => c.CollaborateurID == currentUser.Id && (c.DateDebut.Year == DateTime.Now.Year || c.DateFin.Year == DateTime.Now.Year))
                    .Include(t => t.CongeType)
                    .Where(t => t.CongeType.Label == Types.Maladie)
                    .AsNoTracking()
                    .ToListAsync();

                quotaConges = CountQuota(conges);
                quotaMaladies = CountQuota(maladies);

                viewModel.QuotaConges = quotaConges;
                viewModel.QuotaMaladies = quotaMaladies;
                viewModel.Conges = await query
                    .Where(c => c.CollaborateurID == currentUser.Id)
                    .Include(s => s.Statut)
                    .Include(s => s.CongeType)
                    .Where(s => s.CongeType.Label != Types.Absence)
                    .Include(c => c.Collaborateur)
                    .OrderByDescending(i => i.EnvoiDate)
                    .Take(3)
                    .ToListAsync();
                viewModel.Absences = await query
                    .Where(c => c.CollaborateurID == currentUser.Id && c.DateDebut <= DateTime.Now)
                    .Include(s => s.CongeType)
                    .Include(c => c.Collaborateur)
                    .Include(s => s.Statut)
                    .Where(s => s.Statut.Label == Statuts.Accepte)
                    .AsNoTracking()
                    .OrderByDescending(i => i.EnvoiDate)
                    .Take(3)
                    .ToListAsync();

                viewModel.CurrentUserRole = "Collaborateur";
            }

            if (currentUserRole.Contains("Directeur"))
            {
                var todayAbsences = await query
                    .Where(c => c.DateDebut <= DateTime.Now && c.DateFin >= DateTime.Now)
                    .Include(s => s.Statut)
                    .Where(s => s.Statut.Label == Statuts.Accepte)
                    .Include(t => t.CongeType)
                    .Include(c => c.Collaborateur)
                    .Include(c => c.Collaborateur.Equipe)
                    .Include(c => c.Collaborateur.Specialite)
                    .AsNoTracking()
                    .ToListAsync();

                var incomingAbsences = await query
                    .Where(c => c.DateDebut > DateTime.Now)
                    .Include(s => s.Statut)
                    .Where(s => s.Statut.Label == Statuts.Accepte)
                    .Include(t => t.CongeType)
                    .Include(c => c.Collaborateur)
                    .Include(c => c.Collaborateur.Equipe)
                    .Include(c => c.Collaborateur.Specialite)
                    .AsNoTracking()
                    .ToListAsync();

                viewModel.TodayAbsences = todayAbsences;
                viewModel.IncomingAbsences = incomingAbsences;

                var temp = await _context.Members
                    .Include(c => c.Conges)
                    .Where(c => c.Conges.Any(s => s.DateDebut >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && s.DateFin <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) && s.StatutID == 1 ))
                    .AsNoTracking()
                    .ToListAsync();

                viewModel.NbrCollaborateurAbsent = temp.Count;

                ViewBag.StartMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString();
                ViewBag.EndMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString();

                viewModel.CurrentUserRole = "Directeur";
            }

            if (currentUserRole.Contains("Responsable"))
            {
                var congeEnAttente = await query
                    .Include(s => s.CongeType)
                    .Include(c => c.Collaborateur)
                    .Include(c => c.Collaborateur.Specialite)
                    .Where(c => c.Collaborateur.EquipeID == currentUser.EquipeID)
                    .Include(s => s.Statut)
                    .Where(s => s.Statut.Label == Statuts.Attente)
                    .OrderByDescending(i => i.EnvoiDate)
                    .Take(3)
                    .ToListAsync();

                var congeEnSuppression = await query
                    .Include(s => s.CongeType)
                    .Include(c => c.Collaborateur)
                    .Include(c => c.Collaborateur.Specialite)
                    .Where(c => c.Collaborateur.EquipeID == currentUser.EquipeID)
                    .Include(s => s.Statut)
                    .Where(s => s.Statut.Label == Statuts.Suppression)
                    .OrderByDescending(i => i.EnvoiDate)
                    .Take(3)
                    .ToListAsync();

                viewModel.CongeEnAttente = new List<CongesConflits>();
                viewModel.CongeModel = new List<CongesConflits>();

                foreach(var conge in congeEnAttente)
                {
                    var conflits = await query
                        .Where(c => c.DateDebut <= conge.DateFin && c.DateFin >= conge.DateDebut)
                        .Include(s => s.CongeType)
                        .Include(c => c.Collaborateur)
                        .Where(c => c.CollaborateurID != conge.CollaborateurID)
                        .Where(c => c.Collaborateur.SpecialiteID == conge.Collaborateur.SpecialiteID)
                        .Include(s => s.Statut)
                        .Where(s => s.Statut.Label == Statuts.Accepte)
                        .AsNoTracking()
                        .ToListAsync();
                    
                    var specialite = await _context.Specialites
                        .Include(s => s.Collaborateurs)
                        .SingleAsync(s => s.SpecialiteID == conge.Collaborateur.SpecialiteID);
                    var max = 0;

                    foreach(var conflit in conflits)
                    {
                        var count = 0;
                        foreach(var item in conflits)
                        {
                            if(item.DateDebut <= conflit.DateFin && item.DateFin >= conflit.DateDebut)
                            {
                                count++;
                            }
                        }
                        if(count > max)
                        {
                            max = count;
                        }
                    }
                    
                    viewModel.CongeEnAttente.Add(new CongesConflits
                    {
                        Conge = conge,

                        Conflits = conflits,

                        CanAccept = max < (specialite.Collaborateurs.Count() - 1) ? true : false,

                        ConflitsCount = conflits.Count
                    });
                }

                foreach(var conge in congeEnSuppression)
                {
                    viewModel.CongeModel.Add(new CongesConflits
                    {
                        Conge = conge
                    });
                }

                viewModel.NbrCongeAttente = await query
                    .Include(s => s.Statut)
                    .Where(s => s.Statut.Label == Statuts.Attente)
                    .Include(c => c.Collaborateur)
                    .Where(c => c.Collaborateur.EquipeID == currentUser.EquipeID)
                    .CountAsync();
                viewModel.NbrSuppressionAttente = await query
                    .Include(s => s.Statut)
                    .Where(s => s.Statut.Label == Statuts.Suppression)
                    .Include(c => c.Collaborateur)
                    .Where(c => c.Collaborateur.EquipeID == currentUser.EquipeID)
                    .CountAsync();

                viewModel.CurrentUserRole = "Responsable";
            }

            return View(viewModel);
        }

        public int CountQuota(IEnumerable<Conge> toCount)
        {
            int quota = 0;

            foreach(var c in toCount)
            {
                for(DateTime date = c.DateDebut; date <= c.DateFin; date = date.AddDays(1))
                {
                    if((date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) && date.Year == DateTime.Now.Year)
                    {
                        quota++;
                    }
                }
            }

            return quota;
        }

        [Authorize(Roles = "Collaborateur")]
        public async Task<IActionResult> Apply()
        {
            var viewModel = new CongeFormulaire
            {
                CongeTypes = await _context.CongeTypes
                .Where(t => t.Label != Types.Absence)
                .AsNoTracking()
                .ToListAsync()
            };
            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Collaborateur")]
        public async Task<IActionResult> Apply([Bind("DateDebut, DateFin, CongeTypeID")] Conge conge)
        {
            var viewModel = new CongeFormulaire
            {
                CongeTypes = await _context.CongeTypes
                .Where(t => t.Label != Types.Absence)
                .AsNoTracking()
                .ToListAsync(),
                
                Conge = conge
            };

            var currentUserId = _userManager.GetUserId(HttpContext.User);

            Dictionary<int, int> yearQuota = new Dictionary<int, int>();
            yearQuota.Add(conge.DateDebut.Year, 0);
            if(conge.DateDebut.Year != conge.DateFin.Year)
            {
                yearQuota.Add(conge.DateFin.Year, 0);
            }

            var conges = await _context.Conges
                .Where(c => c.CollaborateurID == currentUserId)
                .Where(c => (c.DateDebut.Year == conge.DateDebut.Year || c.DateDebut.Year == conge.DateFin.Year) || (c.DateFin.Year == conge.DateDebut.Year || c.DateFin.Year == conge.DateFin.Year))
                .Include(s => s.Statut)
                .Where(s => s.Statut.Label == Statuts.Accepte || s.Statut.Label == Statuts.Attente)
                .Include(t => t.CongeType)
                .Where(t => t.CongeType.Label == Types.Conge)
                .AsNoTracking()
                .ToListAsync();

            var congesOwner = await _context.Conges
                .Where(c => c.CollaborateurID == currentUserId)
                .Include(s => s.Statut)
                .Where(s => s.Statut.Label == Statuts.Accepte || s.Statut.Label == Statuts.Attente)
                .AsNoTracking()
                .ToListAsync();

            conges.Add(conge);

            foreach(var c in congesOwner)
            {
                if(c.DateDebut <= conge.DateFin && c.DateFin >= conge.DateDebut)
                {
                    ViewData["Error"] = "Vous avez déjà une demande de congé durant cette période !";
                    return View(viewModel);
                }
            }

            var typeConge = await _context.CongeTypes.SingleAsync(t => t.Label == Types.Conge);
            
            foreach(var c in conges)
            {
                for(DateTime date = c.DateDebut; date <= c.DateFin; date = date.AddDays(1))
                {
                    if((date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) && (yearQuota.ContainsKey(date.Year)))
                    {
                        yearQuota[date.Year] = yearQuota[date.Year] + 1;
                        if(yearQuota[date.Year] > 22 && c.CongeTypeID == typeConge.CongeTypeID)
                        {
                            ViewData["Error"] = "Vous dépassez votre quota pour l'année " + date.Year + " !";
                            return View(viewModel);
                        }
                    }
                }
            }

            try
            {
                conge.CollaborateurID = currentUserId;

                conge.StatutID = _context.Statuts
                    .Single(s => s.Label == Statuts.Attente)
                    .StatutID;

                conge.EnvoiDate = DateTime.Now;
                    
                if (ModelState.IsValid)
                {
                    _context.Add(conge);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Show");
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Responsable")]
        public async Task<IActionResult> Handle(int? id, Types type, string canAccept, string commentaire, string btnDeny, string btnAccept, string btnConfirmDelete)
        {
            if(btnConfirmDelete == null)
            {
                if(canAccept == null && type == Types.Conge && btnAccept != null)
                {
                    return RedirectToAction("Handle");
                }
            }
            
            var conge = _context.Conges
                .Include(s => s.Statut)
                .AsNoTracking()
                .Single(c => c.CongeID == id);

            if(btnAccept != null)
            {
                var accept = await _context.Statuts.AsNoTracking().SingleAsync(s => s.Label == Statuts.Accepte);
                conge.Statut = accept;
                if(commentaire != null)
                {
                    conge.Commentaire = commentaire;
                }
                _context.Conges.Update(conge);
            }
            else if(btnDeny != null && commentaire != null)
            {
                var deny = await _context.Statuts.AsNoTracking().SingleAsync(s => s.Label == Statuts.Refuse);
                conge.Statut = deny;
                conge.Commentaire = commentaire;
                _context.Conges.Update(conge);
            }
            else if(btnConfirmDelete != null)
            {
                _context.Conges.Remove(conge);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Handle");
        }

        [HttpGet]
        [Authorize(Roles = "Responsable")]
        public async Task<IActionResult> Handle(int? id, string nom, int? statut, int? type, string debut, string fin, int?[] ids)
        {
            var query = from s in _context.Conges select s;
            
            var currentUser = _userManager.GetUserAsync(HttpContext.User).Result;
            var viewModel = new CongeOverallData();
            viewModel.CongeModel = new List<CongesConflits>();
            viewModel.CongeEnAttente = new List<CongesConflits>();

            var conges = from s in _context.Conges select s;

            ViewData["ID"] = id;
            ViewData["Collaborateur"] = nom;
            ViewData["Debut"] = debut;
            ViewData["Fin"] = fin;
            TempData["ConflictMode"] = false;
            if(ids.Length > 0)
            {
                TempData["ConflictMode"] = true;
            }
            
            if(debut != null)
            {
                DateTime debutDate = DateTime.Parse(debut);
                conges = conges.Where(c => c.DateDebut >= debutDate);
            }
            if(id != null)
            {
                conges = conges.Where(c => c.CongeID == id);
            }
            else if(ids.Length > 0)
            {
                conges = conges.Where(c => ids.Contains(c.CongeID));
            }
            if(fin != null)
            {
                DateTime finDate = DateTime.Parse(fin);
                conges = conges.Where(c => c.DateFin <= finDate);
            }
            conges = conges.Include(c => c.Collaborateur);
            conges = conges.Where(c => c.Collaborateur.EquipeID == currentUser.EquipeID);
            conges = conges.Include(c => c.Collaborateur.Specialite);
            if(nom != null)
            {
                conges = conges.Where(c => c.Collaborateur.FullName.Contains(nom));
            }
            conges = conges.Include(c => c.Collaborateur.Equipe);
            conges = conges.Include(s => s.Statut);
            if(statut != null)
            {
                conges = conges.Where(s => s.Statut.StatutID == statut);
            }
            else if(ids.Length == 0)
            {
                conges = conges.Where(s => s.Statut.Label == Statuts.Attente || s.Statut.Label == Statuts.Suppression);
            }
            conges = conges.Include(t => t.CongeType).Where(t => t.CongeType.Label != Types.Absence);
            if(type != null)
            {
                conges = conges.Where(t => t.CongeType.CongeTypeID == type);
            }

            var congesList = new List<Conge>();

            if(ids.Length == 0)
            {
                congesList = await conges.OrderByDescending(i => i.EnvoiDate).ToListAsync();
            }
            else
            {
                congesList = await conges.ToListAsync();
            }
            

            foreach(var conge in congesList)
            {
                if(conge.Statut.Label != Statuts.Attente)
                {
                    viewModel.CongeModel.Add(new CongesConflits
                    {
                        Conge = conge
                    });
                }
                else
                {
                    var conflits = await query
                        .Where(c => c.DateDebut <= conge.DateFin && c.DateFin >= conge.DateDebut)
                        .Include(s => s.CongeType)
                        .Include(c => c.Collaborateur)
                        .Where(c => c.CollaborateurID != conge.CollaborateurID)
                        .Where(c => c.Collaborateur.SpecialiteID == conge.Collaborateur.SpecialiteID)
                        .Include(s => s.Statut)
                        .Where(s => s.Statut.Label == Statuts.Accepte)
                        .AsNoTracking()
                        .ToListAsync();
                    
                    var specialite = await _context.Specialites
                        .Include(s => s.Collaborateurs)
                        .SingleAsync(s => s.SpecialiteID == conge.Collaborateur.SpecialiteID);
                    var max = 0;

                    foreach(var conflit in conflits)
                    {
                        var count = 0;
                        foreach(var item in conflits)
                        {
                            if(item.DateDebut <= conflit.DateFin && item.DateFin >= conflit.DateDebut)
                            {
                                count++;
                            }
                        }
                        if(count > max)
                        {
                            max = count;
                        }
                    }
                    
                    viewModel.CongeEnAttente.Add(new CongesConflits
                    {
                        Conge = conge,

                        Conflits = conflits,

                        CanAccept = max < (specialite.Collaborateurs.Count() - 1) ? true : false,

                        ConflitsCount = conflits.Count
                    });
                }
            }

            viewModel.Statuts = await _context.Statuts
                .AsNoTracking()
                .ToListAsync();

            viewModel.CongeTypes = await _context.CongeTypes
                .Where(t => t.Label != Types.Absence)
                .AsNoTracking()
                .ToListAsync();

            viewModel.CurrentUserRole = "Responsable";

            return View(viewModel);
        }

        [Authorize(Roles = "Collaborateur, Directeur")]
        public async Task<IActionResult> Show(int? id, string nom, int? statut, int? type, string debut, string fin)
        {
            var currentUser = _userManager.GetUserAsync(HttpContext.User).Result;
            var currentUserRole = _userManager.GetRolesAsync(currentUser).Result;
            var viewModel = new CongeOverallData();

            ViewData["ID"] = id;
            ViewData["Collaborateur"] = nom;
            ViewData["Debut"] = debut;
            ViewData["Fin"] = fin;

            var conges = from s in _context.Conges select s;
            conges = conges.Include(c => c.Collaborateur);

            if (currentUserRole.Contains("Collaborateur"))
            {
                conges = conges.Where(c => c.CollaborateurID == currentUser.Id);
                viewModel.CurrentUserRole = "Collaborateur";
            }

            if(currentUserRole.Contains("Directeur"))
            {
                conges = conges.Include(c => c.Collaborateur.Specialite)
                    .Include(c => c.Collaborateur.Equipe);

                viewModel.CurrentUserRole = "Directeur";
            }

            if(debut != null)
            {
                DateTime debutDate = DateTime.Parse(debut);
                conges = conges.Where(c => c.DateDebut >= debutDate);
            }
            if(id != null)
            {
                conges = conges.Where(c => c.CongeID == id);
            }
            if(fin != null)
            {
                DateTime finDate = DateTime.Parse(fin);
                conges = conges.Where(c => c.DateFin <= finDate);
            }
            if(nom != null)
            {
                conges = conges.Where(c => c.Collaborateur.FullName.Contains(nom));
            }
            conges = conges.Include(s => s.Statut);
            if(statut != null)
            {
                conges = conges.Where(s => s.Statut.StatutID == statut);
            }
            conges = conges.Include(t => t.CongeType).Where(t => t.CongeType.Label != Types.Absence);
            if(type != null)
            {
                conges = conges.Where(t => t.CongeType.CongeTypeID == type);
            }

            viewModel.Conges = await conges.OrderByDescending(i => i.EnvoiDate).ToListAsync();

            viewModel.Statuts = await _context.Statuts
                .AsNoTracking()
                .ToListAsync();

            viewModel.CongeTypes = await _context.CongeTypes
                .Where(t => t.Label != Types.Absence)
                .AsNoTracking()
                .ToListAsync();

            return View(viewModel);
        }

        [Authorize(Roles = "Collaborateur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conge = await _context.Conges
                .Include(c => c.Collaborateur)
                .Include(c => c.CongeType)
                .Include(c => c.Statut)
                .AsNoTracking()
                .SingleAsync(c => c.CongeID == id);

            if(conge == null || (conge.DateDebut <= DateTime.Now && conge.Statut.Label == Statuts.Accepte) || conge.Statut.Label == Statuts.Refuse || conge.Statut.Label == Statuts.Suppression)
            {
                return NotFound();
            }

            return View(conge);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Collaborateur")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conge = await _context.Conges
                .Include(s => s.Statut)
                .AsNoTracking()
                .SingleAsync(c => c.CongeID == id);

            if(conge == null || (conge.DateDebut <= DateTime.Now && conge.Statut.Label == Statuts.Accepte) || conge.Statut.Label == Statuts.Refuse || conge.Statut.Label == Statuts.Suppression)
            {
                return RedirectToAction("Show");
            }

            try
            {
                if(conge.Statut.Label != Statuts.Accepte)
                {
                    _context.Conges.Remove(conge);
                }
                else
                {
                    var suppression = await _context.Statuts.AsNoTracking().SingleAsync(s => s.Label == Statuts.Suppression);
                    conge.Statut = suppression;
                    _context.Conges.Update(conge);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Show");
            }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { id, saveChangesError = true });
            }
        }
    }
}