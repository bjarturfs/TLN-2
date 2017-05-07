using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.ViewModels
{
    public class CreateFileViewModel
    {
        [Required]
        [MaxLength(30, ErrorMessage = "Name must be at most 30 characters.")]
        public string Name { get; set; }
        public string Type { get; set; }
        public int ProjectID { get; set; }
    }
}