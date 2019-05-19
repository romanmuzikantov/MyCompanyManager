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
    public class AbsencesController : Controller
    {
        private readonly MyCompanyContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AbsencesController(MyCompanyContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Show(int? id, string nom, int? type, string debut, string fin, string time)
        {
            var currentUser = _userManager.GetUserAsync(HttpContext.User).Result;
            var currentUserRole = _userManager.GetRolesAsync(currentUser).Result;
            var viewModel = new CongeOverallData();

            ViewData["ID"] = id;
            ViewData["Collaborateur"] = nom;
            ViewData["Debut"] = debut;
            ViewData["Fin"] = fin;
            ViewData["Type"] = type;
            ViewData["Time"] = time;

            var absences = from c in _context.Conges select c;
            
            if((time == "before" || time == null) && !(debut != null || fin != null))
            {
                absences = absences.Where(c => c.DateDebut <= DateTime.Now);
            }
            else if(time == "today" && !(debut != null || fin != null))
            {
                absences = absences.Where(c => c.DateDebut <= DateTime.Now && c.DateFin >= DateTime.Now);
            }
            else if(time == "incoming" && !(debut != null || fin != null))
            {
                absences = absences.Where(c => c.DateDebut > DateTime.Now);
            }
            
            absences = absences.Include(c => c.Collaborateur);
            if (currentUserRole.Contains("Collaborateur"))
            {
                absences = absences.Where(c => c.CollaborateurID == currentUser.Id);
            }
            if (currentUserRole.Contains("Responsable"))
            {
                absences = absences.Where(c => c.Collaborateur.EquipeID == currentUser.EquipeID);
            }

            absences = absences.Include(s => s.Statut)
                .Where(s => s.Statut.Label == Statuts.Accepte);

            if(debut != null)
            {
                DateTime debutDate = DateTime.Parse(debut);
                absences = absences.Where(c => c.DateDebut >= debutDate);
            }
            if(id != null)
            {
                absences = absences.Where(c => c.CongeID == id);
            }
            if(fin != null)
            {
                DateTime finDate = DateTime.Parse(fin);
                absences = absences.Where(c => c.DateFin <= finDate);
            }
            if(nom != null)
            {
                absences = absences.Where(c => c.Collaborateur.FullName.Contains(nom));
            }
            absences = absences.Include(t => t.CongeType);
            if(type != null)
            {
                absences = absences.Where(t => t.CongeType.CongeTypeID == type);
            }

            if (currentUserRole.Contains("Collaborateur"))
            {
                viewModel.CurrentUserRole = "Collaborateur";
            }
            if (currentUserRole.Contains("Responsable"))
            {
                viewModel.CurrentUserRole = "Responsable";
                absences = absences.Include(c => c.Collaborateur.Specialite);
            }
            if (currentUserRole.Contains("Directeur"))
            {
                viewModel.CurrentUserRole = "Directeur";
                absences = absences.Include(c => c.Collaborateur.Specialite)
                    .Include(c => c.Collaborateur.Equipe);
            }

            viewModel.Absences = await absences
                .OrderByDescending(i => i.EnvoiDate)
                .AsNoTracking()
                .ToListAsync();

            viewModel.CongeTypes = await _context.CongeTypes
                .AsNoTracking()
                .ToListAsync();
            
            return View(viewModel);
        }

        public ActionResult Filtrate()
        {
            return View();
        }

        public ActionResult Assign()
        {
            return View();
        }
    }
}