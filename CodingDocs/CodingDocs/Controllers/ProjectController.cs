using CodingDocs.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodingDocs.Controllers
{
    public class ProjectController : Controller
    {
        private ProjectService pservice = new ProjectService();

        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetIndividualProjects()
        {
            string userId = User.Identity.GetUserId();
            var viewModel = pservice.GetIndividualProjects(userId);

            return View(viewModel);
        }
    }
}