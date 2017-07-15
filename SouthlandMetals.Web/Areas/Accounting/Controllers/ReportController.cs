using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Reporting;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Helpers;
using SouthlandMetals.Web.Models;
using System;
using System.IO;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Accounting.Controllers
{
    public class ReportController : ApplicationBaseController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICountryRepository _countryRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;

        public ReportController()
        {
            _countryRepository = new CountryRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
        }

        public ReportController(ICountryRepository countryRepository,
                                ICustomerDynamicsRepository customerDynamicsRepository,
                                IFoundryDynamicsRepository foundryDynamicsRepository)
        {
            _countryRepository = countryRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
        }

        /// <summary>
        /// GET: Accounting/Report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting")]
        public ActionResult Index()
        {
            var model = new ReportViewModel();

            model.StartDateStr = DateTime.Now.ToShortDateString();
            model.EndDateStr = DateTime.Now.ToShortDateString();

            return View(model);
        }

        /// <summary>
        /// GET: Accounting/Accounting/FoundryInvoicesReport
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="unscheduled"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting")]
        public ActionResult FoundryInvoicesReport(DateTime startDate, DateTime endDate, bool unscheduled)
        {
            var operationResult = new OperationResult();
            MemoryStream ms = new MemoryStream();

            try
            {
                ms = ReportingManager.CreateFoundryInvoicesReport(startDate, endDate, unscheduled);
            }
            catch(Exception ex)
            {
                operationResult.Message = "Error occured printing Foundry Invoices Report";
                logger.ErrorFormat("Error occured printing Foundry Invoices Report: {0} ", ex.ToString());
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }

            return new FileStreamResult(ms, "application/pdf");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}