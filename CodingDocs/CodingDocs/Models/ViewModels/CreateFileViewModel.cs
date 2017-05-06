using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.ViewModels
{
    public class CreateFileViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int ProjectID { get; set; }
    }
}