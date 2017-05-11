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
        private readonly IAppDataContext _db;
     
        // Initialize _db to the real database or a mock database.
        public ProjectService(IAppDataContext context)
        {
            _db = context ?? new ApplicationDbContext();
        }

        #region Projects

        // Returns a list of ProjectViewModel with the projects which the ownerID of the projects is the userId that gets sent in
        public List<ProjectViewModel> GetIndividualProjects(string userId)
        {
            var projects = (from proj in _db.Projects
                            where proj.OwnerID == userId
                            select proj)
                            .Select(x => new ProjectViewModel { ID = x.ID, Name = x.Name, Type = x.Type })
                            .ToList();

            return projects;
        }

        // Returns a list of ProjectViewModel with the projects the user with the userId has access to 
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

        // Returns a ViewProjectViewModel for the project that has the projectID that gets sent in
        // This function finds the users in project and project owner
        public ViewProjectViewModel GetProject(int projectId)
        {
            var prod = (from proj in _db.Projects
                           where proj.ID == projectId
                           select proj)
                           .SingleOrDefault();

            var files = (from file in _db.Files
                         where file.ProjectID == projectId
                         select file)
                         .Select(x => new ProjectFileViewModel { ID = x.ID, Name = x.Name, Type = x.Type })
                         .ToList();

            var usersInProjectID = (from users in _db.UsersInProjects
                                    where users.ProjectID == projectId
                                    select users.UserID)
                                    .ToList();

            var userID = prod.OwnerID;

            var ownerName = (from owner in _db.Users
                             where owner.Id == userID
                             select owner.UserName)
                             .SingleOrDefault();

            List<string> usersNames = new List<string>();
            foreach (var item in usersInProjectID)
            {
                var holder = (from name in _db.Users
                              where name.Id == item
                              select name.UserName)
                              .SingleOrDefault();

                usersNames.Add(holder);
            }

            var project = new ViewProjectViewModel
            {
                ID = prod.ID,
                Name = prod.Name,
                Type = prod.Type,
                Files = files,
                UserName = usersNames,
                OwnerName = ownerName
            };

            return project;
        }

        // Returns the projectID of the project that has the file with the fileId that gets sent in 
        public int GetProjectByFile(int fileId)
        {
            var file = (from f in _db.Files
                        where f.ID == fileId
                        select f).SingleOrDefault();

            return file.ProjectID;
        }

        // Creates a project with the attributes from the CreateProjectViewModel that gets sent in
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

        // Shares the project with the user in the ShareProjectViewModel
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

        // Deletes the project with the projectID that gets sent in 
        public void DeleteProject(int projectId)
        {
            var files = (from file in _db.Files
                         where file.ProjectID == projectId
                         select file).ToList();

            foreach (var file in files)
            {
                _db.Files.Remove(file);
            }
            _db.SaveChanges();

            var usersInProject = (from uip in _db.UsersInProjects
                                  where uip.ProjectID == projectId
                                  select uip).ToList();

            foreach (var uip in usersInProject)
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

        // Removes the user access to a project, the userID is in the RemoveProjectViewModel
        public void RemoveSharedProject(RemoveProjectViewModel projectVM)
        {
            var userInProject = (from uip in _db.UsersInProjects
                                 where uip.ProjectID == projectVM.ProjectID
                                 && uip.UserID == projectVM.UserID
                                 select uip).SingleOrDefault();

            _db.UsersInProjects.Remove(userInProject);
            _db.SaveChanges();
        }

        // Returns true if the user is the project owner of the project or if he has access to the project
        public bool AuthorizeProject(string userId, int projectId)
        {
            if (IsOwner(userId, projectId)) return true;
            if (HasSharedAccess(userId, projectId)) return true;

            return false;
        }

        // Returns true if the the user is the owner of the project
        public bool IsOwner(string userId, int projectId)
        {
            var project = (from proj in _db.Projects
                           where proj.ID == projectId
                           select proj).SingleOrDefault();

            if (project == null) return false;
            if (project.OwnerID == userId) return true;

            return false;
        }

        // Returns true if the user has access to the project
        public bool HasSharedAccess(string userId, int projectId)
        {
            var sharedProject = (from uip in _db.UsersInProjects
                                 where uip.ProjectID == projectId
                                 && uip.UserID == userId
                                 select uip).SingleOrDefault();

            if (sharedProject != null) return true;

            return false;
        }
        #endregion

        #region Files

        // Creates a file with the attributes from the CreateFileViewModel
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

        // Deletes the file with the fileID that gets sent in
        public void DeleteFile(int fileId)
        {
            var file = (from f in _db.Files
                        where f.ID == fileId
                        select f).SingleOrDefault();

            _db.Files.Remove(file);
            _db.SaveChanges();
        }

        // Overrides the content of a file with the fileID from the SaveFileViewModel with the content from the SaveFileViewModel
        public void SaveFile(SaveFileViewModel fileVM)
        {
            var file = (from f in _db.Files
                        where f.ID == fileVM.ID
                        select f).SingleOrDefault();

            file.Content = fileVM.Content;
            _db.SaveChanges();
        }

        // Returns true if the file is in the project that gets sent in by the CreateFileViewModel
        public bool FileExistsInProject(CreateFileViewModel newFile)
        {
            var file = (from f in _db.Files
                        where f.Name == newFile.Name
                        && f.ProjectID == newFile.ProjectID
                        select f).SingleOrDefault();

            return (file != null);
        }

        // Returns a FileViewModel with the attributes from the file with the fileID that gets sent in 
        public FileViewModel GetFile(int fileId)
        {
            var file = (from f in _db.Files
                        where f.ID == fileId
                        select f).SingleOrDefault();

            var project = (from proj in _db.Projects
                           where proj.ID == file.ProjectID
                           select proj)
                           .Select(x => new ProjectViewModel { ID = x.ID, Name = x.Name, Type = x.Type })
                           .SingleOrDefault();

            var fileVM = new FileViewModel
            {
                ID = file.ID,
                Name = file.Name,
                Type = file.Type,
                Content = file.Content,
                Project = project
            };

            return fileVM;
        }
        #endregion

        #region Users

        // Returns true if the userName that gets sent in exists in our database
        public bool UserExists(string userName)
        {
            var user = (from usr in _db.Users
                        where usr.UserName == userName
                        select usr)
                        .SingleOrDefault();

            return (user != null) ;
        }

        // Returns the userID of the user with the username that gets sent in
        public string GetUserId(string userName)
        {
            var user = (from usr in _db.Users
                        where usr.UserName == userName
                        select usr).SingleOrDefault();

            return user.Id;
        }
        #endregion
    }
}