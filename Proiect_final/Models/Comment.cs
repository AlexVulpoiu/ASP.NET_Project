using Proiect_final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proiect.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required (ErrorMessage = "Acest camp este obligatoriu") ]
        public string CommentContent { get; set; }
        
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int TaskId { get; set; }
        public virtual Task Task { get; set; } 

        public DateTime Date { get; set; }
    }
}
