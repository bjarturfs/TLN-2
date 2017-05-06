using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.Entities
{
    public class FilesInProject
    {
        public int ID { get; set; }
        public int FileID { get; set; }
        public int ProjectID { get; set; }
    }
}