using CodingDocs.Models;
using CodingDocs.Models.Entities;
using CodingDocs.Models.ViewModels;
using Microsoft.AspNet.Identity;
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

        public List<ProjectViewModel> GetIndividualProjects(string userId)
        {
            var projects = (from proj in _db.Projects
                            where proj.OwnerID == userId
                            select proj)
                            .Select(x => new ProjectViewModel { Name = x.Name, Type = x.Type })
                            .ToList();
            return projects;
        }

        public List<ProjectViewModel> GetSharedProjects(string userId)
        {
            var projects = (from proj in _db.Projects
                            join uip in _db.UsersInProjects on proj.ID equals uip.ProjectID
                            where uip.UserID == userId
                            select proj)
                            .Select(x => new ProjectViewModel { Name = x.Name, Type = x.Type })
                            .ToList();
            return projects;
        }

        // Maybe return bool?
        public void CreateProject(Project project)
        {
            _db.Projects.Add(project);
            _db.SaveChanges();
        }
    }
}