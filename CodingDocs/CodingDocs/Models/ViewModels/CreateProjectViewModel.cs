using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.ViewModels
{
    public class CreateProjectViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string OwnerID { get; set; }
    }
}