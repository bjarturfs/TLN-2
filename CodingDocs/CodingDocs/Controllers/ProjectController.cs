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
        public ActionResult CreateProject(ProjectViewModel projectVM)
        {
            string userId = User.Identity.GetUserId();

            if(userId == null)
            {
                //TODO: error handling
            }

            Project project = new Project
            {
                Name = projectVM.Name,
                Type = projectVM.Type,
                OwnerID = userId
            };

            // Check ModelState?
            pservice.CreateProject(project);
            return RedirectToAction("MyProjects");
        }
    }
}