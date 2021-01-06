using Proiect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect_final.Models
{
    public class TeamUser
    {
        [Key]
        public int TeamUserId  {get; set;}
        public string Id { get; set; } // de la User.Id
        public int TeamId { get; set; }

        public virtual ApplicationUser Users { get; set; }
        public virtual Team Teams { get; set; }
    }
}