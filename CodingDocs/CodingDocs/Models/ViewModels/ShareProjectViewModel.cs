using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.ViewModels
{
    public class ShareProjectViewModel
    {
        public int ProjectID { get; set; }
        [Required]
        [Display(Name = "Username")]
        [MaxLength(30, ErrorMessage = "Name must be at most 30 characters.")]
        public string UserName { get; set; }
    }
}