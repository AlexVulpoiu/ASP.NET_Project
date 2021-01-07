using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Proiect.Models;
using Proiect_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects

        [HttpGet]
        [Authorize(Roles = "Admin,Organiser,Member")]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.Projects = db.Projects.OrderBy(m => m.ProjectName);
                return View();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a accesa aceasta pagina!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }

        [NonAction]
        public bool isMember(int teamId, string userId)
        {
            var user = from x in db.TeamUsers
                        where x.TeamId == teamId && x.Id == userId
                        select x;
            foreach(var x in user)
            {
                if (x.Id == userId)
                    return true;
            }
            return false;
        }

        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Show(int id)
        {
            if (User.IsInRole("Admin") || isMember(db.Projects.Find(id).TeamId, User.Identity.GetUserId()))
            {
                Project project = db.Projects.Find(id);
                setAccessRights(project);
                return View(project);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a accesa aceasta pagina!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }
       
        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult New()
        {
            Project project = new Project();
            string currentUser = User.Identity.GetUserId();
            project.Echipe = GetAllTeams(currentUser);
            project.UserId = User.Identity.GetUserId();
            return View(project);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllTeams(string currentUser)
        {
            var selectList = new List<SelectListItem>();
            var teams = from team in db.Teams
                         select team;
            foreach (var team in teams)
            {
                if (team.UserId == currentUser)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = team.TeamId.ToString(),
                        Text = team.TeamName.ToString()
                    });
                }
            }

            return selectList;
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [HttpPost]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult New(Project project)
        {
            string currentUser = User.Identity.GetUserId(); 
            project.Echipe = GetAllTeams(currentUser);
            project.UserId = User.Identity.GetUserId();
            project.TeamId = 0;
            try
            {
                if (ModelState.IsValid)
                { 
                    ApplicationUser user = db.Users.Find(project.UserId);
                    user.AllRoles = GetAllRoles();
                    var userRole = user.Roles.FirstOrDefault();
                    
                    ApplicationDbContext context = new ApplicationDbContext();
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    if (TryUpdateModel(project))
                    {
                        db.Projects.Add(project);
                        if (!User.IsInRole("Organiser") && !User.IsInRole("Admin"))
                        {
                            var roles = from role in db.Roles select role;
                            foreach (var role in roles)
                            {
                                UserManager.RemoveFromRole(project.UserId, role.Name);
                            }
                            var selectedRole = db.Roles.Find("Organiser");

                            UserManager.AddToRole(project.UserId, "Organiser");

                        }
                        db.SaveChanges();

                    }
                    TempData["message"] = "Proiectul a fost adaugat cu succes!";
                    TempData["status"] = "success";
                    return Redirect("/Home/Index");
                }
                else
                    return View(project);
            }
            catch (Exception e)
            {
                return View(project);
            }
        }

        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult AddTeam() 
        {
            string currentUser = User.Identity.GetUserId();
            ViewBag.Echipe = GetAllTeams(currentUser);
            return View();
        }
        [HttpPut]
        [Authorize(Roles = "Organiser,Admin")]
        public ActionResult AddTeam (int TeamId, int ProjectId)
        {
            Project project = db.Projects.Find(ProjectId);
            string currentUser = User.Identity.GetUserId();
            ViewBag.Echipe = GetAllTeams(currentUser);
            ViewBag.ProjectId = ProjectId;
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                try
                {
                    project.TeamId = TeamId;
                    db.SaveChanges();
                    return Redirect("/Projects/Show/" + ProjectId.ToString());
                }
                catch (Exception e)
                {
                    return View();
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a efectua aceasta actiune!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }
        [HttpGet]
        [Authorize(Roles = "Organiser,Admin")]
        public ActionResult Edit(int id)
        {
            Project project = db.Projects.Find(id);
            string currentUser = User.Identity.GetUserId();
            project.Echipe = GetAllTeams(currentUser);
            if (User.Identity.GetUserId() == project.UserId || User.IsInRole("Admin"))
            {
                return View(project);
            }
            else
            {
                TempData["message"] = "Nu ai voie sa modifici proiectul altei perosoane!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Organiser,Admin")]
        public ActionResult Edit(int id, Project requestProject)
        {
            string currentUser = User.Identity.GetUserId();
            requestProject.Echipe = GetAllTeams(currentUser);
            try
            {
                if (ModelState.IsValid)
                {
                    Project project = db.Projects.Find(id);
                    if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    { 
                        if (TryUpdateModel(requestProject))
                        {
                            project.ProjectName = requestProject.ProjectName;
                            project.ProjectDescription = requestProject.ProjectDescription;
                            project.TeamId = requestProject.TeamId;
                            db.SaveChanges();
                            TempData["message"] = "Proiectul a fost editat cu succes!";
                            TempData["status"] = "warning";
                            return Redirect("/Home/Index");
                        }
                        else
                            return View(requestProject);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa modificati proiectul altei persoane!";
                        TempData["status"] = "danger";
                        return Redirect("/Home/Index");
                    }
                }
                else
                    return View(requestProject);
            }
            catch(Exception e)
            {
                return View(requestProject);
            }
        }
        
        [HttpDelete]
        [Authorize(Roles = "Organiser,Admin")]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Projects.Remove(project);
                db.SaveChanges();
                TempData["message"] = "Proiectul a fost sters!";
                TempData["status"] = "warning";
                return Redirect("/Home/Index");
            }
            else
            {
                TempData["message"] = "Nu ai dreptul sa stergi acest proiect!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }
        [NonAction]
        private void setAccessRights(Project project)
        {
            ViewBag.afisareButoane = false;
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.isAdmin = User.IsInRole("Admin");
            ViewBag.currentUser = User.Identity.GetUserId();
        }

    }
}
