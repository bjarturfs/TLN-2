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
                            .Select(x => new ProjectViewModel { ID = x.ID, Name = x.Name, Type = x.Type })
                            .ToList();

            return projects;
        }

        public List<ProjectViewModel> GetSharedProjects(string userId)
        {
            var projects = (from proj in _db.Projects
                            join uip in _db.UsersInProjects on proj.ID equals uip.ProjectID
                            where uip.UserID == userId
                            select proj)
                            .Select(x => new ProjectViewModel { ID = x.ID, Name = x.Name, Type = x.Type })
                            .ToList();

            return projects;
        }

        // Maybe return bool?
        public void CreateProject(CreateProjectViewModel projectVM)
        {
            Project project = new Project
            {
                Name = projectVM.Name,
                Type = projectVM.Type,
                OwnerID = projectVM.OwnerID,
            };

            _db.Projects.Add(project);
            _db.SaveChanges();

            CreateFileViewModel file = new CreateFileViewModel
            {
                Name = "index",
                Type = project.Type,
                ProjectID = project.ID
            };

            CreateFile(file);
        }

        public void ShareProject(ShareProjectViewModel projectVM)
        {
            var userID = (from usr in _db.Users
                          where usr.UserName == projectVM.UserName
                          select usr.Id)
                          .SingleOrDefault();

            UsersInProject uip = new UsersInProject
            {
                ProjectID = projectVM.ProjectID,
                UserID = userID
            };

            _db.UsersInProjects.Add(uip);
            _db.SaveChanges();
        }

        public bool ValidUserName(string userName)
        {
            var user = (from usr in _db.Users
                        where usr.UserName == userName
                        select usr)
                        .SingleOrDefault();

            if (user == null) return false;

            return true;
        }

        public ViewProjectViewModel GetProject(int projectId)
        {
            var project = (from proj in _db.Projects
                           where proj.ID == projectId
                           select proj)
                           .Select(x => new ViewProjectViewModel { ID = x.ID, Name = x.Name, Type = x.Type })
                           .SingleOrDefault();

            var files = (from file in _db.Files
                         where file.ProjectID == projectId
                         select file)
                         .Select(x => new FileViewModel { ID = x.ID, Name = x.Name, Type = x.Type, Content = x.Content })
                         .ToList();

            project.Files = files;

            return project;
        }

        public bool AuthorizeProject(string userId, int projectId)
        {
            if(IsOwner(userId, projectId)) return true;
            if(HasSharedAccess(userId, projectId)) return true;

            return false;
        }

        public bool IsOwner(string userId, int projectId)
        {
            var project = (from proj in _db.Projects
                           where proj.ID == projectId
                           select proj).SingleOrDefault();

            if(project == null) return false;
            if(project.OwnerID == userId) return true;

            return false;
        }

        public bool HasSharedAccess(string userId, int projectId)
        {
            var sharedProject = (from uip in _db.UsersInProjects
                                 where uip.ProjectID == projectId
                                 && uip.UserID == userId
                                 select uip).SingleOrDefault();

            if(sharedProject != null) return true;

            return false;
        }

        public void CreateFile(CreateFileViewModel fileVM)
        {
            File file = new File
            {
                Name = fileVM.Name,
                Type = fileVM.Type,
                Content = "",
                ProjectID = fileVM.ProjectID
            };

            _db.Files.Add(file);
            _db.SaveChanges();
        }

        public void DeleteProject(int projectId)
        {
            var files = (from file in _db.Files
                         where file.ProjectID == projectId
                         select file).ToList();

            foreach(var file in files)
            {
                _db.Files.Remove(file);
            }
            _db.SaveChanges();

            var usersInProject = (from uip in _db.UsersInProjects
                                 where uip.ProjectID == projectId
                                 select uip).ToList();

            foreach(var uip in usersInProject)
            {
                _db.UsersInProjects.Remove(uip);
            }
            _db.SaveChanges();

            var project = (from proj in _db.Projects
                           where proj.ID == projectId
                           select proj).SingleOrDefault();

            _db.Projects.Remove(project);
            _db.SaveChanges();
        }

        public void RemoveSharedProject(RemoveProjectViewModel projectVM)
        {
            var userInProject = (from uip in _db.UsersInProjects
                                 where uip.ProjectID == projectVM.ProjectID
                                 && uip.UserID == projectVM.UserID
                                 select uip).SingleOrDefault();

            _db.UsersInProjects.Remove(userInProject);
            _db.SaveChanges();
        }

        public int GetProjectByFile(int fileId)
        {
            var file = (from f in _db.Files
                        where f.ID == fileId
                        select f).SingleOrDefault();

            return file.ProjectID;
        }

        public void DeleteFile(int fileId)
        {
            var file = (from f in _db.Files
                        where f.ID == fileId
                        select f).SingleOrDefault();

            _db.Files.Remove(file);
            _db.SaveChanges();
        }

        public string GetUserId(string userName)
        {
            var user = (from usr in _db.Users
                        where usr.UserName == userName
                        select usr).SingleOrDefault();

            // if(user == null) throw exception

            return user.Id;
        }

        public bool HasAccess(ShareProjectViewModel projectVM)
        {
            string userId = GetUserId(projectVM.UserName);

            var userInProject = (from uip in _db.UsersInProjects
                                 where uip.ProjectID == projectVM.ProjectID
                                 && uip.UserID == userId
                                 select uip).SingleOrDefault();

            return (userInProject != null);
        }

        public bool FileExistsInProject(CreateFileViewModel newFile)
        {
            var file = (from f in _db.Files
                        where f.Name == newFile.Name
                        && f.ProjectID == newFile.ProjectID
                        select f).SingleOrDefault();

            return (file != null);
        }
    }
}