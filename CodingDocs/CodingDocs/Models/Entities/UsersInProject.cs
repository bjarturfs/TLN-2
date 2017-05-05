using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingDocs.Models.Entities
{
    public class UsersInProject
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public string UserID { get; set; }
    }
}