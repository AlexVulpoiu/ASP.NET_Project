using Proiect_final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        [Required (ErrorMessage ="Numele taskului este obligatoriu")]
        public string TaskName { get; set; }
        public string Status { get; set; }

        [Required (ErrorMessage ="Descrierea taskului este obligatoriu")]
        public string TaskDescription { get; set; }

        [Required(ErrorMessage = "Data de inceput este obligatorie")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Data de sfarsit este obligatorie")]
        public DateTime EndDate { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        [Required(ErrorMessage = "Proiectul este obligatoriu")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } 

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string WorkerId{ get; set; }

        public IEnumerable<SelectListItem> Statusuri { get; set; }
    }
}