using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Administration.Controllers
{
    public class SiteController : ApplicationBaseController
    {
        private ISiteDynamicsRepository _siteDynamicsRepository;
        
        public SiteController()
        {
            _siteDynamicsRepository = new SiteDynamicsRepository();
        }

        public SiteController(ISiteDynamicsRepository siteDynamicsRepository)
        {
            _siteDynamicsRepository = siteDynamicsRepository;
        }

        /// <summary>
        /// GET: Administration/Site
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new SiteViewModel();

            var sites = new List<SiteViewModel>();

            var tempSites = _siteDynamicsRepository.GetSites().Where(x => x.INACTIVE != 1).ToList();

            if (tempSites != null && tempSites.Count > 0)
            {
                foreach (var tempSite in tempSites)
                {
                    SiteViewModel convertedModel = new SiteConverter().ConvertToView(tempSite);

                    sites.Add(convertedModel);
                }
            }

            model.Sites = sites.OrderBy(x => x.SiteDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// GET: Administration/Site/Detail
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Detail(string siteId)
        {
            var site = _siteDynamicsRepository.GetSite(siteId);

            SiteViewModel model = new SiteConverter().ConvertToView(site);

            return View(model);
        }

        /// <summary>
        /// GET: get active sites
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveSites()
        {
            var model = new SiteViewModel();

            var sites = new List<SiteViewModel>();

            var tempSites = _siteDynamicsRepository.GetSites().Where(x => x.INACTIVE != 1).ToList();

            if (tempSites != null && tempSites.Count > 0)
            {
                foreach (var tempSite in tempSites)
                {
                    SiteViewModel convertedModel = new SiteConverter().ConvertToView(tempSite);

                    sites.Add(convertedModel);
                }
            }

            model.Sites = sites.OrderBy(x => x.SiteDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive sites
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveSites()
        {
            var model = new SiteViewModel();

            var sites = new List<SiteViewModel>();

            var tempSites = _siteDynamicsRepository.GetSites().Where(x => x.INACTIVE != 0).ToList();

            if (tempSites != null && tempSites.Count > 0)
            {
                foreach (var tempSite in tempSites)
                {
                    SiteViewModel convertedModel = new SiteConverter().ConvertToView(tempSite);

                    sites.Add(convertedModel);
                }
            }

            model.Sites = sites.OrderBy(x => x.SiteDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_siteDynamicsRepository != null)
                {
                    _siteDynamicsRepository.Dispose();
                    _siteDynamicsRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}