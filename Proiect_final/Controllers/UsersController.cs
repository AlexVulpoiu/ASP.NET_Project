using Microsoft.AspNet.Identity;
using Proiect_final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_final.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            List<ApplicationUser> list = db.Users.OrderBy(m => m.UserName).ToList();
            ViewBag.Users = list;
            var currentUser = User.Identity.GetUserId();
            ViewBag.AdminName = db.Users.Find(currentUser).UserName;
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(string Id)
        {

            if (User.IsInRole("Admin"))
            {
                var listOfProjects = from proj in db.Projects
                                     where proj.UserId == Id
                                     select proj;
                foreach (var elem in listOfProjects)
                {
                    db.Projects.Remove(elem);
                }

                var listOfTeams = from team in db.Teams
                                  where team.UserId == Id
                                  select team;
                foreach (var elem in listOfTeams)
                {
                    db.Teams.Remove(elem);
                }

                var listOfTasks = from task in db.Tasks
                                  where task.WorkerId == Id
                                  select task;
                foreach (var elem in listOfTasks)
                {
                    elem.WorkerId = null;
                }

                var memberOfATeam = from team in db.TeamUsers
                                    where team.Id == Id
                                    select team;
                foreach (var elem in memberOfATeam)
                {
                    db.TeamUsers.Remove(elem);
                }

                var listOfComments = from comment in db.Comments
                                     where comment.UserId == Id
                                     select comment;
                foreach (var elem in listOfComments)
                {
                    db.Comments.Remove(elem);
                }
                var user = db.Users.Find(Id);
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["message"] = "Utilizatorul a fost sters!";
                TempData["status"] = "warning";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest user ";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }

    }
}
