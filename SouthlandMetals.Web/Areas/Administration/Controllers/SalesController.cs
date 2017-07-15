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
    public class SalesController : ApplicationBaseController
    {
        private ISalespersonDynamicsRepository _salespersonDynamicsRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;

        public SalesController()
        {
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
        }

        public SalesController(ISalespersonDynamicsRepository salespersonDynamicsRepository,
                               ICustomerDynamicsRepository customerDynamicsRepository)
        {
            _customerDynamicsRepository = customerDynamicsRepository;
            _salespersonDynamicsRepository = salespersonDynamicsRepository;
        }

        /// <summary>
        /// GET: Administration/Sales
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new SalespersonViewModel();

            var salespersons = new List<SalespersonViewModel>();

            var tempSalespersons = _salespersonDynamicsRepository.GetSalespersons().Where(x => x.INACTIVE != 1).ToList();

            if (tempSalespersons != null && tempSalespersons.Count > 0)
            {
                foreach (var tempSalesperson in tempSalespersons)
                {
                    SalespersonViewModel convertedModel = new SalespersonConverter().ConvertToView(tempSalesperson);

                    salespersons.Add(convertedModel);
                }
            }

            model.Salespersons = salespersons.OrderBy(x => x.SalespersonName).ToList();

            return View(model);
        }

        /// <summary>
        /// GET: Administration/Sales/Detail
        /// </summary>
        /// <param name="salespersonId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Detail(string salespersonId)
        {
            var salesperson = _salespersonDynamicsRepository.GetSalesperson(salespersonId);

            SalespersonViewModel model = new SalespersonConverter().ConvertToView(salesperson);

            return View(model);
        }

        /// <summary>
        /// GET: Administration/Sales/Customers
        /// </summary>
        /// <param name="salespersonId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Customers(string salespersonId)
        {
            var model = new SalespersonViewModel();

            var salesperson = _salespersonDynamicsRepository.GetSalesperson(salespersonId);

            var customers = new List<CustomerViewModel>();

            var tempCustomers = _customerDynamicsRepository.GetCustomers().Where(x => x.SLPRSNID.Replace(" ", string.Empty).ToLower() == salespersonId.Replace(" ", string.Empty).ToLower()).OrderBy(y => y.SHRTNAME).ToList();

            if (tempCustomers != null && tempCustomers.Count > 0)
            {
                foreach (var tempCustomer in tempCustomers)
                {
                    CustomerViewModel convertedModel = new CustomerConverter().ConvertToListView(tempCustomer);

                    customers.Add(convertedModel);
                }
            }

            model.Customers = customers;
            model.SalespersonName = salesperson.SLPRSNFN + " " + salesperson.SPRSNSLN;

            return View(model);
        }

        /// <summary>
        /// get active salespersons
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveSalespersons()
        {
            var model = new SalespersonViewModel();

            var salespersons = new List<SalespersonViewModel>();

            var tempSalespersons = _salespersonDynamicsRepository.GetSalespersons().Where(x => x.INACTIVE != 1).ToList();

            if (tempSalespersons != null && tempSalespersons.Count > 0)
            {
                foreach (var tempSalesperson in tempSalespersons)
                {
                    SalespersonViewModel convertedModel = new SalespersonConverter().ConvertToView(tempSalesperson);

                    salespersons.Add(convertedModel);
                }
            }

            model.Salespersons = salespersons.OrderBy(x => x.SalespersonName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive salespersons
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveSalespersons()
        {
            var model = new SalespersonViewModel();

            var salespersons = new List<SalespersonViewModel>();

            var tempSalespersons = _salespersonDynamicsRepository.GetSalespersons().Where(x => x.INACTIVE != 0).ToList();

            if (tempSalespersons != null && tempSalespersons.Count > 0)
            {
                foreach (var tempSalesperson in tempSalespersons)
                {
                    SalespersonViewModel convertedModel = new SalespersonConverter().ConvertToView(tempSalesperson);

                    salespersons.Add(convertedModel);
                }
            }

            model.Salespersons = salespersons.OrderBy(x => x.SalespersonName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_salespersonDynamicsRepository != null)
                {
                    _salespersonDynamicsRepository.Dispose();
                    _salespersonDynamicsRepository = null;
                }

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }  
            }

            base.Dispose(disposing);
        }
    }
}