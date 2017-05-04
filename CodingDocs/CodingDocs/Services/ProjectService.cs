using CodingDocs.Models;
using CodingDocs.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodingDocs.Services
{
    public class ProjectService
    {
        private ApplicationDbContext _db;

        public ProjectService()
        {
            _db = new ApplicationDbContext();
        }

        public List<ProjectViewModel> GetIndividualProjects(string UserID)
        {
            var projects = (from proj in _db.Projects
                            where proj.OwnerID == UserID
                            select proj)
                            .Select(x => new ProjectViewModel { Name = x.Name, Type = x.Type })
                            .ToList();
            return projects;
        }
    }
}