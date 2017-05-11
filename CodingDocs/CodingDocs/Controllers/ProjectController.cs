using CodingDocs.Models.Entities;
using CodingDocs.Models.ViewModels;
using CodingDocs.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CodingDocs.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private ProjectService pservice;

        public ProjectController()
        {
            pservice = new ProjectService(null);

            List<SelectListItem> types = new List<SelectListItem>();

            SelectListItem type1 = new SelectListItem { Text = "C", Value = "c", Selected = true };
            SelectListItem type2 = new SelectListItem { Text = "C#", Value = "cs", Selected = false };
            SelectListItem type3 = new SelectListItem { Text = "C++", Value = "cpp", Selected = false };
            SelectListItem type4 = new SelectListItem { Text = "CSS", Value = "css", Selected = false };
            SelectListItem type5 = new SelectListItem { Text = "HTML", Value = "html", Selected = false };
            SelectListItem type6 = new SelectListItem { Text = "Java", Value = "java", Selected = false };
            SelectListItem type7 = new SelectListItem { Text = "JavaScript", Value = "js", Selected = false };
            SelectListItem type8 = new SelectListItem { Text = "PHP", Value = "php", Selected = false };
            SelectListItem type9 = new SelectListItem { Text = "Python", Value = "py", Selected = false };
            SelectListItem type10 = new SelectListItem { Text = "SQL", Value = "sql", Selected = false };

            types.Add(type1);
            types.Add(type2);
            types.Add(type3);
            types.Add(type4);
            types.Add(type5);
            types.Add(type6);
            types.Add(type7);
            types.Add(type8);
            types.Add(type9);
            types.Add(type10);

            ViewBag.Types = types;
        }

        #region Projects
        // Gets all the projects of the user which he is the owner of 
        public ActionResult MyProjects()
        {
            string userId = User.Identity.GetUserId();

            var viewModel = pservice.GetIndividualProjects(userId);
            return View(viewModel);
        }

        // Gets all the projects the user has been shared in 
        public ActionResult SharedProjects()
        {
            string userId = User.Identity.GetUserId();

            var viewModel = pservice.GetSharedProjects(userId);
            return View(viewModel);
        }

        // Shows the files, owner and users in the project which has the id that gets sent in
        public ActionResult ViewProject(int id)
        {
            string userId = User.Identity.GetUserId();

            if (pservice.AuthorizeProject(userId, id))
            {
                var viewModel = pservice.GetProject(id);
                return View(viewModel);
            }

            return View("ErrorViewProject");
        }

        // Gets a pop up window for create project
        public PartialViewResult CreateProjectPartial()
        {
            return PartialView("CreateProjectPartial", new CreateProjectViewModel());
        }

        // Creates the project which was entered in the partial view
        [HttpPost]
        public ActionResult CreateProject(CreateProjectViewModel project)
        {
            if(!ModelState.IsValid)
            {
                return View(project);
            }

            string userId = User.Identity.GetUserId();
            project.OwnerID = userId;

            pservice.CreateProject(project);
            return RedirectToAction("MyProjects");
        }

        // Gets a pop up view which takes in the project id which we want to share
        public PartialViewResult InviteUserPartial(int id)
        {
            var user = new ShareProjectViewModel();
            user.ProjectID = id;
            
            return PartialView("InviteUserPartial", user);
        }

        // Shares the project with the user that was inserted in the InviteUser view
        // if the user does not exist then we will return error messages and not insert the user to the project
        [HttpPost]
        public ActionResult InviteUser(ShareProjectViewModel model)
        {
            string userId = User.Identity.GetUserId();

            if(!pservice.UserExists(model.UserName))
            {
                ModelState.AddModelError("UserName", "User does not exist.");
            }

            else
            {
                string newUserId = pservice.GetUserId(model.UserName);

                if(pservice.HasSharedAccess(newUserId, model.ProjectID))
                {
                    ModelState.AddModelError("UserName", "User already has access to this project.");
                }

                if(newUserId == userId)
                {
                    ModelState.AddModelError("UserName", "You cannot invite yourself to this project.");
                }

                if(pservice.IsOwner(newUserId, model.ProjectID))
                {
                    ModelState.AddModelError("UserName", "This user is the owner of the project.");
                }
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            pservice.ShareProject(model);
            return RedirectToAction("ViewProject", new { id = model.ProjectID });
        }

        // Deletes the project with the id that gets sent in the view and the user has to be the owner of the project
        public ActionResult DeleteProject(int id)
        {
            string userId = User.Identity.GetUserId();

            if (pservice.IsOwner(userId, id))
            {
                pservice.DeleteProject(id);
                return RedirectToAction("MyProjects");
            }

            return View("Error");
        }

        // Removes the user from the shared Project with the id that gets sent in the view
        // Does not delete the project just the user out of the project 
        public ActionResult RemoveSharedProject(int id)
        {
            string userId = User.Identity.GetUserId();

            if (pservice.HasSharedAccess(userId, id))
            {
                var project = new RemoveProjectViewModel
                {
                    ProjectID = id,
                    UserID = userId
                };
                pservice.RemoveSharedProject(project);
                return RedirectToAction("SharedProjects");
            }

            return View("Error");
        }
        #endregion

        #region Files

        // Creates a pop up window for create file and takes in project id 
        public PartialViewResult CreateFilePartial(int id)
        {
            var viewModel = new CreateFileViewModel();
            viewModel.ProjectID = id;
            return PartialView("CreateFilePartial", viewModel);
        }

        // Creates the file from the partial view in the project with the id from the partal view
        [HttpPost]
        public ActionResult CreateFile(CreateFileViewModel file)
        {
            if (pservice.FileExistsInProject(file))
            {
                ModelState.AddModelError("Name", "There is already a file by that name.");
            }

            if (!ModelState.IsValid)
            {
                return View(file);
            }

            pservice.CreateFile(file);
            return RedirectToAction("ViewProject", new { id = file.ProjectID });
        }

        // Deletes the file with the id that gets sent in 
        public ActionResult DeleteFile(int id)
        {
            string userId = User.Identity.GetUserId();
            int projectId = pservice.GetProjectByFile(id);

            if (pservice.AuthorizeProject(userId, projectId))
            {
                pservice.DeleteFile(id);
                return RedirectToAction("ViewProject", new { id = projectId });
            }

            return View("Error");
        }

        // Saves the file with the id from the SaveFileViewModel with the content from the SaveFileViewModel
        [HttpPost]
        public ActionResult SaveFile(SaveFileViewModel file)
        {
            string userId = User.Identity.GetUserId();
            int projectId = pservice.GetProjectByFile(file.ID);

            if(pservice.AuthorizeProject(userId, projectId))
            {
                if(file.Content == null) file.Content = "";

                pservice.SaveFile(file);
                return RedirectToAction("GetFile", new { id = file.ID });
            }

            return View("Error");
        }

        // Displays the content in the ACE editor of the file with the id that gets sent in 
        public ActionResult GetFile(int id)
        {
            string userId = User.Identity.GetUserId();
            int projectId = pservice.GetProjectByFile(id);

            if(pservice.AuthorizeProject(userId, projectId))
            {
                var file = pservice.GetFile(id);
                return View(file);
            }

            return View("ErrorViewProject");
        }
        #endregion
    }
}