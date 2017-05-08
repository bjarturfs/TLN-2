using CodingDocs.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.ViewModels
{
    public class ViewProjectViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<FileViewModel> Files { get; set; }
        public List<string> UserName { get; set; }
    }
}