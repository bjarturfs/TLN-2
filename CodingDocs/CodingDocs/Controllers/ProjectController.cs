using CodingDocs.Models.Entities;
using CodingDocs.Models.ViewModels;
using CodingDocs.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingDocs.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private ProjectService pservice = new ProjectService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyProjects()
        {
            string userId = User.Identity.GetUserId();

            if(userId == null)
            {
                //TODO: error handling
            }

            var viewModel = pservice.GetIndividualProjects(userId);
            return View(viewModel);
        }

        public ActionResult SharedProjects()
        {
            string userId = User.Identity.GetUserId();

            if(userId == null)
            {
                //TODO: error handling
            }

            var viewModel = pservice.GetSharedProjects(userId);
            return View(viewModel);
        }

        public ActionResult CreateProject()
        {
            return View();
        }

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

        public ActionResult ViewProject(int id)
        {
            string userId = User.Identity.GetUserId();

            if(pservice.AuthorizeProject(userId, id))
            {
                var viewModel = pservice.GetProject(id);
                return View(viewModel);
            }

            return View("Error");
        }

        public ActionResult InviteUser(int id)
        {
            var user = new ShareProjectViewModel();
            user.ProjectID = id;
        
            return View(user); 
        }

        [HttpPost]
        public ActionResult InviteUser(ShareProjectViewModel model)
        {
            string userId = User.Identity.GetUserId();

            if(!pservice.ValidUserName(model.UserName))
            {
                ModelState.AddModelError("UserName", "User does not exist.");
            }

            else
            {
                if(pservice.HasAccess(model))
                {
                    ModelState.AddModelError("UserName", "User already has access to this project.");
                }

                if(pservice.GetUserId(model.UserName) == userId)
                {
                    ModelState.AddModelError("UserName", "You cannot invite yourself to this project.");
                }
            }

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            pservice.ShareProject(model);
            return RedirectToAction("ViewProject", new { id = model.ProjectID });
        }

        public ActionResult CreateFile(int id)
        {
            string userId = User.Identity.GetUserId();

            if(pservice.AuthorizeProject(userId, id))
            {
                var viewModel = new CreateFileViewModel();
                viewModel.ProjectID = id;
                return View(viewModel);
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult CreateFile(CreateFileViewModel file)
        {
            if(pservice.FileExistsInProject(file))
            {
                ModelState.AddModelError("Name", "There is already a file by that name.");
            }

            if(!ModelState.IsValid)
            {
                return View(file);
            }

            file.Type = pservice.GetProject(file.ProjectID).Type;
            pservice.CreateFile(file);
            return RedirectToAction("ViewProject", new { id = file.ProjectID });
        }

        public ActionResult DeleteProject(int id)
        {
            string userId = User.Identity.GetUserId();

            if(pservice.IsOwner(userId, id))
            {
                pservice.DeleteProject(id);
                return RedirectToAction("MyProjects");
            }

            return View("Error");
        }

        public ActionResult RemoveSharedProject(int id)
        {
            string userId = User.Identity.GetUserId();

            if(pservice.HasSharedAccess(userId, id))
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

        public ActionResult DeleteFile(int id)
        {
            string userId = User.Identity.GetUserId();
            int projectId = pservice.GetProjectByFile(id);

            if(pservice.AuthorizeProject(userId, projectId))
            {
                pservice.DeleteFile(id);
                return RedirectToAction("ViewProject", new { id = projectId });
            }

            return View("Error");
        }
    }
}