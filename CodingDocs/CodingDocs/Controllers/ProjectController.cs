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
            string userId = User.Identity.GetUserId();

            if (userId == null)
            {
                //TODO: error handling
            }

            project.OwnerID = userId;

            // Check ModelState?
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
            if (pservice.ValidUserName(model.UserName))
            {
                pservice.ShareProject(model);
                return RedirectToAction("ViewProject", new { id = model.ProjectID });
            }

            return View("Error");
        }

        public ActionResult CreateFile(int id)
        {
            string userId = User.Identity.GetUserId();

            if (pservice.AuthorizeProject(userId, id))
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
            }
    
            return RedirectToAction("MyProjects");
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
            }

            return RedirectToAction("SharedProjects");
        }
    }
}