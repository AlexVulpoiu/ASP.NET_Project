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
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult New(Comment comment)
        {
            try
            {
                comment.UserId = User.Identity.GetUserId();
                comment.Date = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost adaugat!";
                TempData["status"] = "success";
                return Redirect("/Tasks/Show/" + comment.TaskId.ToString());
            }
            catch (Exception e)
            {
                return Redirect("/Tasks/Show/" + comment.TaskId.ToString());
            }
        }
        [HttpGet]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a edita acest comentariu!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Member,Organiser,Admin")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                requestComment.Task = comment.Task;
                if (User.IsInRole("Admin") || User.Identity.GetUserId() == comment.UserId)
                {
                    if (TryUpdateModel(requestComment))
                    {
                        comment.CommentContent = requestComment.CommentContent;
                        comment.Date = requestComment.Date;
                        db.SaveChanges();
                        TempData["message"] = "Comentariul a fost editat!";
                        TempData["status"] = "warning";
                    }
                    return Redirect("/Tasks/Show/" + comment.TaskId.ToString());
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul de a edita acest comentariu!";
                    TempData["status"] = "danger";
                    return Redirect("/Home/Index");
                }
            }
            catch (Exception e)
            {
                return View(requestComment);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Memer,Organiser,Admin")]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            var x = comment.TaskId;
            if (User.IsInRole("Admin") || comment.UserId == User.Identity.GetUserId())
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters!";
                TempData["status"] = "warning";
                return Redirect("/Tasks/Show/" + x.ToString());
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul de a sterge acest comentariu!";
                TempData["status"] = "danger";
                return Redirect("/Home/Index");
            }
        }
    }
}