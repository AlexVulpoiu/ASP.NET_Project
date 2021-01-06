using Microsoft.AspNet.Identity;
using Proiect.Models;
using Proiect_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Controllers
{

    public class TeamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Teams
        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Index()
        {
            ViewBag.Teams = db.Teams.OrderBy(m => m.TeamName);
            if ( User.IsInRole("Admin"))
            {
                return View();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa vizualizati aceasta pagina!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult NewTeam()
        {
            Team team = new Team();
            team.UserId = User.Identity.GetUserId();
            return View(team);
        }

        [HttpPost]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult NewTeam(Team team)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = db.Users.Find(User.Identity.GetUserId());
                    TeamUser tu = new TeamUser();
                    tu.Id = user.Id;
                    tu.TeamId = team.TeamId;
                    db.TeamUsers.Add(tu);
                    db.Teams.Add(team);
                    db.SaveChanges();
                    TempData["message"] = "Echipa a fost adaugata!";
                    TempData["status"] = "succes";
                    return Redirect("/Home/Index");
                }
                else
                {
                    return View(team);
                }
            }
            catch (Exception e)
            {
                return View(team);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Show(int id)
        {
            Team team = db.Teams.Find(id);
            setAccessRights(team);
            List<ApplicationUser> listUsers = ListTeamUser(team.TeamId);
            bool ok = false;
            foreach (var elem in listUsers)
            {
                if (elem.Id == User.Identity.GetUserId())
                {
                    ok = true;
                    break;
                }
            }

            if (User.IsInRole("Admin") || ok)
            {
                ViewBag.Manager = db.Users.Find(team.UserId);
                ViewBag.Members = UsersToShow(id);
                return View(team);
            }
            else
            {
                TempData["message"] = "Nu faceti parte din aceasta echipa!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }   
        }

        private List<ApplicationUser> UsersToShow(int id)
        {
            Team team = db.Teams.Find(id);
            string manager = team.UserId;
            var obj = from x in db.TeamUsers
                      where x.TeamId == id && x.Id != manager
                      select x;
            List<ApplicationUser> toShow = new List<ApplicationUser>();
            foreach (var elem in obj)
            {
                ApplicationUser user = db.Users.Find(elem.Id);
                toShow.Add(user);
            }
            return toShow;
        }

        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult New()
        {
            Team team = new Team();
            team.UserId = User.Identity.GetUserId();
            return View(team);
        }

        [HttpPost]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult New(Team team)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = db.Users.Find(User.Identity.GetUserId());
                    TeamUser tu = new TeamUser();
                    tu.Id = user.Id;
                    tu.TeamId = team.TeamId;
                    db.TeamUsers.Add(tu);
                    db.Teams.Add(team);
                    db.SaveChanges();
                    TempData["message"] = "Echipa a fost adaugata!";
                    TempData["status"] = "success";
                    return Redirect("/Home/Index");
                }
                else
                {
                    return View(team);
                }
            }
            catch (Exception e)
            {
                return View(team);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);
            if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(team);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati aceasta echipa!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index"); 
            }

        }

        [HttpPut]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Edit(int id, Team requestTeam)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Team team = db.Teams.Find(id);
                    if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(team))
                        {
                            team.TeamName = requestTeam.TeamName;
                            db.SaveChanges();
                            TempData["message"] = "Editarea echipei a fost efectuata cu succes!";
                            TempData["status"] = "warning";
                            return Redirect("/Teams/Show/" + team.TeamId.ToString());
                        }
                        else
                            return View(requestTeam);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa editati aceasta echipa!";
                        TempData["status"] = "danger";
                        return Redirect("/Home/Index");
                    }
                }
                else
                    return View(requestTeam);
            }
            catch (Exception e)
            {
                return View(requestTeam);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Delete(int id)
        {
            Team team = db.Teams.Find(id);
            if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Teams.Remove(team);
                db.SaveChanges();
                TempData["message"] = "Echipa a fost stearsa!";
                TempData["status"] = "warning";
                return Redirect("/Home/Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti aceasta echipa!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }

        }

        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult NewMember(string TeamId)
        {
            Team team = db.Teams.Find(Convert.ToInt32(TeamId));

            ViewBag.UnusedUser = UnusedUser(team.TeamId);
            if (User.Identity.GetUserId() == team.UserId || User.IsInRole("Admin"))
            {
                return View(team);
            }
            else
            {
                TempData["message"] = "Nu aveti permisiunea sa adaugati un membru!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }

        [NonAction]
        private List<ApplicationUser> UnusedUser(int teamId)
        {
            var query = from x in db.Users
                        select x;
            List<ApplicationUser> listOfUsers = new List<ApplicationUser>();
            foreach (var elem in query)
            {
                listOfUsers.Add(elem);
            }
            foreach (var pair in db.TeamUsers)
            {
                foreach (var elem in query)
                {
                    if (pair.Id == elem.Id && pair.TeamId == teamId)
                        listOfUsers.Remove(elem);
                }
            }
            return listOfUsers;
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllMembers(Team team)
        {
            var selectList = new List<SelectListItem>();
            var members = from member in db.Users
                          select member;

            foreach (var member in members)
            {
                selectList.Add(new SelectListItem
                {
                    Value = member.Id.ToString(),
                    Text = member.UserName.ToString()
                });

            }
            foreach (var elem in db.TeamUsers)
            {
                foreach (var member in selectList)
                {
                    if (elem.Id == member.Value && elem.TeamId == team.TeamId)
                    {
                        selectList.Remove(member);
                        break;
                    }
                }
            }
            return selectList;
        }

        [HttpPost]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult NewMember(string UserId, int TeamId, string RUser)
        {
            var query = from x in db.Users
                        select x;
            ViewBag.query = query;
            Team team = db.Teams.Find(TeamId);
            team.Members = GetAllMembers(team);
            try
            {
                if (User.Identity.GetUserId() == team.UserId || User.IsInRole("Admin"))
                {
                    if (ModelState.IsValid)
                    {
                        TeamUser tu = new TeamUser();
                        tu.Id = RUser;
                        tu.TeamId = team.TeamId;
                        db.TeamUsers.Add(tu);
                        db.SaveChanges();
                        TempData["message"] = "Membrul a fost adaugat!";
                        TempData["status"] = "warning";
                        return RedirectToAction("/Show/" + TeamId.ToString());
                    }
                    else
                    {
                        return View(team);
                    }
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul de a adauga un membru in aceasta echipa!";
                    TempData["status"] = "danger";
                    return Redirect("/Home/Index");
                }
            }
            catch (Exception e)
            {
                return View(team);
            }

        }

        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult DeleteMember(string UserId, string TeamId)
        {

            Team team = db.Teams.Find(Convert.ToInt32(TeamId));

            if (User.Identity.GetUserId() == team.UserId || User.IsInRole("Admin"))
            {
                int aux = Convert.ToInt32(TeamId);
                var temp1 = from x in db.TeamUsers
                            where (x.Id == UserId && x.TeamId == aux)
                            select x;
                var tasks = db.Tasks;
                foreach(var elem in tasks)
                {
                    if (elem.WorkerId == UserId && elem.Project.TeamId == aux)
                        elem.WorkerId = null;
                }
                //TeamUser temp1 = db.TeamUsers.Find(UserId, TeamId);
                foreach (var elem in temp1)
                {
                    db.TeamUsers.Remove(elem);
                }
                db.SaveChanges();
                TempData["message"] = "Membrul a fost sters!";
                TempData["status"] = "warning";
                return Redirect("/Teams/Show/" + TeamId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest user!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }

        [NonAction]
        private void setAccessRights(Team team)
        {
            ViewBag.afisareButoane = false;
            if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.isAdmin = User.IsInRole("Admin");
            ViewBag.currentUser = User.Identity.GetUserId();
        }

        [NonAction]
        private List<ApplicationUser> ListTeamUser(int teamId)
        {
            var query = from x in db.Users
                        select x;
            List<ApplicationUser> listOfUsers = new List<ApplicationUser>();
            foreach (var pair in db.TeamUsers)
            {
                foreach (var elem in query)
                {
                    if ( pair.Id == elem.Id && pair.TeamId == teamId)
                        listOfUsers.Add(elem);
                }
            }
            return listOfUsers;
        }

        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Search(string TeamId)
        {
            var search = "";
            var aux = Convert.ToInt32(TeamId);
            var team = db.Teams.Find(aux);
            ViewBag.UnusedUser = UnusedUser(team.TeamId);
            List<ApplicationUser> UnuserdUsers = UnusedUser(team.TeamId);

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<ApplicationUser> UsersList = new List<ApplicationUser>();
                foreach (var elem in UnuserdUsers)
                {
                    if (elem.UserName.Contains(search))
                    {
                        UsersList.Add(elem);
                    }
                }
                ViewBag.UsersList = UsersList;
            }

            var teamId = db.Teams.Find(aux);
            return View(teamId);

        }
    }
}
