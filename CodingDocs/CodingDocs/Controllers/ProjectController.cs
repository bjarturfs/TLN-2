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
        private ProjectService pservice = new ProjectService();

        #region Projects
        public ActionResult MyProjects()
        {
            string userId = User.Identity.GetUserId();

            var viewModel = pservice.GetIndividualProjects(userId);
            return View(viewModel);
        }

        public ActionResult SharedProjects()
        {
            string userId = User.Identity.GetUserId();

            var viewModel = pservice.GetSharedProjects(userId);
            return View(viewModel);
        }

        public ActionResult ViewProject(int id)
        {
            string userId = User.Identity.GetUserId();

            if (pservice.AuthorizeProject(userId, id))
            {
                ViewBag.CurrentFile = "HalloFile";
                var viewModel = pservice.GetProject(id);
                return View(viewModel);
            }

            return View("ErrorViewProject");
        }

        public ActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateProject(CreateProjectViewModel project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            string userId = User.Identity.GetUserId();
            project.OwnerID = userId;

            pservice.CreateProject(project);
            return RedirectToAction("MyProjects");
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

            if (!pservice.UserExists(model.UserName))
            {
                ModelState.AddModelError("UserName", "User does not exist.");
            }

            else
            {
                string newUserId = pservice.GetUserId(model.UserName);

                if (pservice.HasSharedAccess(newUserId, model.ProjectID))
                {
                    ModelState.AddModelError("UserName", "User already has access to this project.");
                }

                if (newUserId == userId)
                {
                    ModelState.AddModelError("UserName", "You cannot invite yourself to this project.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            pservice.ShareProject(model);
            return RedirectToAction("ViewProject", new { id = model.ProjectID });
        }

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
            if (pservice.FileExistsInProject(file))
            {
                ModelState.AddModelError("Name", "There is already a file by that name.");
            }

            if (!ModelState.IsValid)
            {
                return View(file);
            }

            file.Type = pservice.GetProject(file.ProjectID).Type;
            pservice.CreateFile(file);
            return RedirectToAction("ViewProject", new { id = file.ProjectID });
        }

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

        [HttpPost]
        public ActionResult SaveFile(SaveFileViewModel file)
        {
            string userId = User.Identity.GetUserId();
            int projectId = pservice.GetProjectByFile(file.ID);

            if (pservice.AuthorizeProject(userId, projectId))
            {
                pservice.SaveFile(file);
                return RedirectToAction("ViewProject", new { id = projectId });
            }

            return View("Error");
        }

        public JsonResult GetContent(int id)
        {
            string content = pservice.GetContent(id);

            return Json(content, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}