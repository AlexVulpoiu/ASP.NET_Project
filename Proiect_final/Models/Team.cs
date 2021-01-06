using Proiect_final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        [Required(ErrorMessage = "Numele echipei este obligatoriu")]
        public string TeamName { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<TeamUser> TeamUsers { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public IEnumerable<SelectListItem> Members { get; set; }
        public string AuxUser { get; set; }
    }
}