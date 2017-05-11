using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodingDocs.Services;
using CodingDocs.Models;
using CodingDocs.Models.Entities;
using CodingDocs.Models.ViewModels;

namespace CodingDocs.Tests.Services
{
    [TestClass]
    public class ProjectServiceTest
    {
        private ProjectService pservice;

        [TestInitialize]
        public void Initialize()
        {
            var mockDb = new MockDatabase();
            var user1 = new ApplicationUser
            {
                Id = "1",
                UserName = "user1",
                Email = "example@gmail.com"
            };
            mockDb.Users.Add(user1);

            var user2 = new ApplicationUser
            {
                Id = "2",
                UserName = "user2",
                Email = "example@ru.is"
            };
            mockDb.Users.Add(user2);

            var project1 = new Project
            {
                 ID = 1,
                 Name = "project1",
                 OwnerID = "1",
                 Type = "js"
            };
            mockDb.Projects.Add(project1);

            var project2 = new Project
            {
                ID = 2,
                Name = "project2",
                OwnerID = "2",
                Type = "cs"
            };
            mockDb.Projects.Add(project2);

            var project3 = new Project
            {
                ID = 3,
                Name = "project3",
                OwnerID = "1",
                Type = "html"
            };
            mockDb.Projects.Add(project3);

            var uip1 = new UsersInProject
            {
                ID = 1,
                ProjectID = 1,
                UserID = "2"
            };
            mockDb.UsersInProjects.Add(uip1);

            var uip2 = new UsersInProject
            {
                ID = 2,
                ProjectID = 2,
                UserID = "1"
            };
            mockDb.UsersInProjects.Add(uip2);

            var file1 = new File
            {
                ID = 1,
                Name = "index",
                Type = "js",
                ProjectID = 1,
                Content = "index.js in project1"
            };
            mockDb.Files.Add(file1);

            var file2 = new File
            {
                ID = 2,
                Name = "index",
                Type = "cs",
                ProjectID = 2,
                Content = "index.cs in project2"
            };
            mockDb.Files.Add(file2);

            var file3 = new File
            {
                ID = 3,
                Name = "index",
                Type = "html",
                ProjectID = 3,
                Content = "index.html in project3"
            };
            mockDb.Files.Add(file3);

            var file4 = new File
            {
                ID = 4,
                Name = "file4",
                Type = "js",
                ProjectID = 1,
                Content = "file4.js in project1"
            };
            mockDb.Files.Add(file4);

            var file5 = new File
            {
                ID = 5,
                Name = "file5",
                Type = "html",
                ProjectID = 3,
                Content = "file5.html in project3"
            };
            mockDb.Files.Add(file5);

            pservice = new ProjectService(mockDb);
        }

        #region Projects
        [TestMethod]
        public void TestGetIndividualProjects()
        {
            // Arrange
            string user1 = "1";
            string user2 = "2";

            // Act
            var result1 = pservice.GetIndividualProjects(user1);
            var result2 = pservice.GetIndividualProjects(user2);

            // Assert
            Assert.AreEqual(2, result1.Count);
            Assert.AreEqual(1, result2.Count);
        }

        [TestMethod]
        public void TestGetSharedProjects()
        {
            string user1 = "1";
            string user2 = "2";

            var result1 = pservice.GetSharedProjects(user1);
            var result2 = pservice.GetSharedProjects(user2);

            Assert.AreEqual(1, result1.Count);
            Assert.AreEqual(1, result2.Count);
        }

        [TestMethod]
        public void TestGetProject()
        {
            int project1 = 1;
            int project2 = 2;
            int project3 = 3;

            var result1 = pservice.GetProject(project1);
            var result2 = pservice.GetProject(project2);
            var result3 = pservice.GetProject(project3);

            Assert.AreEqual("project1", result1.Name);
            Assert.AreEqual("project2", result2.Name);
            Assert.AreEqual("project3", result3.Name);
        }

        [TestMethod]
        public void TestGetProjectByFile()
        {
            int file1 = 1;
            int file2 = 3;
            int file3 = 5;

            var result1 = pservice.GetProjectByFile(file1);
            var result2 = pservice.GetProjectByFile(file2);
            var result3 = pservice.GetProjectByFile(file3);

            Assert.AreEqual(1, result1);
            Assert.AreEqual(3, result2);
            Assert.AreEqual(3, result3);
        }

        [TestMethod]
        public void TestCreateProject()
        {
            string userId = "1";
            var projectVM = new CreateProjectViewModel
            {
                Name = "project4",
                Type = "cpp",
                OwnerID = userId
            };

            pservice.CreateProject(projectVM);

            var result = pservice.GetIndividualProjects(userId);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void TestShareProject()
        {
            string userId = "2";
            var projectVM = new ShareProjectViewModel
            {
                ProjectID = 3,
                UserName = "user2"
            };

            pservice.ShareProject(projectVM);

            var result = pservice.GetSharedProjects(userId);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestDeleteProject()
        {
            int projectId = 1;
            string userId = "1";

            pservice.DeleteProject(projectId);

            var result = pservice.GetIndividualProjects(userId);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestRemoveSharedProject()
        {
            int projectId = 1;
            string userId = "2";
            var projectVM = new RemoveProjectViewModel
            {
                 ProjectID = projectId,
                 UserID = userId
            };

            pservice.RemoveSharedProject(projectVM);

            var result = pservice.GetSharedProjects(userId);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestAuthorizeProject()
        {
            string user = "2";
            int project1 = 1;
            int project2 = 2;
            int project3 = 3;

            var result1 = pservice.AuthorizeProject(user, project1);
            var result2 = pservice.AuthorizeProject(user, project2);
            var result3 = pservice.AuthorizeProject(user, project3);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);
        }

        [TestMethod]
        public void TestIsOwner()
        {
            string user = "1";
            int project1 = 1;
            int project2 = 2;
            int project3 = 3;

            var result1 = pservice.IsOwner(user, project1);
            var result2 = pservice.IsOwner(user, project2);
            var result3 = pservice.IsOwner(user, project3);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
            Assert.IsTrue(result3);
        }

        [TestMethod]
        public void TestHasSharedAccess()
        {
            string user1 = "1";
            string user2 = "2";
            int project1 = 1;
            int project2 = 2;
            int project3 = 3;

            var result1 = pservice.HasSharedAccess(user1, project2);
            var result2 = pservice.HasSharedAccess(user2, project1);
            var result3 = pservice.HasSharedAccess(user2, project3);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);
        }
        #endregion

        #region Files
        [TestMethod]
        public void TestCreateFile()
        {
            int projectId = 1;
            var fileVM = new CreateFileViewModel
            {
                Name = "newfile",
                Type = "js",
                ProjectID = projectId
            };

            pservice.CreateFile(fileVM);

            var result = pservice.GetProject(projectId).Files;
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void TestDeleteFile()
        {
            int fileId = 1;
            int projectId = 1;

            pservice.DeleteFile(fileId);

            var result = pservice.GetProject(projectId).Files;
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestSaveFile()
        {
            int fileId = 1;
            var fileVM = new SaveFileViewModel
            {
                ID = fileId,
                Content = "new content"
            };

            pservice.SaveFile(fileVM);

            var result = pservice.GetFile(fileId);
            Assert.AreEqual("new content", result.Content);
        }

        [TestMethod]
        public void TestFileExistsInProject()
        {
            var fileVM1 = new CreateFileViewModel
            {
                Name = "file4",
                ProjectID = 1
            };
            var fileVM2 = new CreateFileViewModel
            {
                Name = "file4",
                ProjectID = 2
            };
            var fileVM3 = new CreateFileViewModel
            {
                Name = "index",
                ProjectID = 2
            };
            
            var result1 = pservice.FileExistsInProject(fileVM1);
            var result2 = pservice.FileExistsInProject(fileVM2);
            var result3 = pservice.FileExistsInProject(fileVM3);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
            Assert.IsTrue(result3);
        }

        [TestMethod]
        public void TestGetFile()
        {
            int fileId = 5;

            var result = pservice.GetFile(fileId);

            Assert.AreEqual("file5.html in project3", result.Content);
        }
        #endregion

        #region Users
        [TestMethod]
        public void TestUserExists()
        {
            string userName1 = "user1";
            string userName2 = "user123";

            var result1 = pservice.UserExists(userName1);
            var result2 = pservice.UserExists(userName2);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void TestGetUserId()
        {
            string userName1 = "user1";
            string userName2 = "user2";

            var result1 = pservice.GetUserId(userName1);
            var result2 = pservice.GetUserId(userName2);

            Assert.AreEqual("1", result1);
            Assert.AreEqual("2", result2);
        }
        #endregion
    }
}
