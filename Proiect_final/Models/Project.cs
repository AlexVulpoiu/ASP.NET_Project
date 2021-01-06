using Proiect_final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        [Required(ErrorMessage ="Numele proiectului este obligatoriu")]
        [MaxLength(100, ErrorMessage = "Numele proiectului nu poate avea mai mult de 100 de caractere")]
        public string ProjectName { get; set; }
        [Required (ErrorMessage ="Descrierea este obligatorie")]
        public string ProjectDescription { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int TeamId { get; set; }
        public virtual Team Team { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
        public IEnumerable<SelectListItem> Echipe { get; set; }
    }
}