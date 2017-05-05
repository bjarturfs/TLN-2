using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.Entities
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int OwnerID { get; set; }
    }
}