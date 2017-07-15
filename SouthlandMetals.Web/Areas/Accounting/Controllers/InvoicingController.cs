using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using SouthlandMetals.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Accounting.Controllers
{
    public class InvoicingController : ApplicationBaseController
    {
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private IFoundryInvoiceRepository _foundryInvoiceRepository;

        public InvoicingController()
        {
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _foundryInvoiceRepository = new FoundryInvoiceRepository();
        }

        public InvoicingController(IFoundryDynamicsRepository foundryDynamicsRepository,
                                   IFoundryInvoiceRepository foundryInvoiceRepository)
        {
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _foundryInvoiceRepository = foundryInvoiceRepository;
        }

        /// <summary>
        /// GET: Accounting/Invoicing/FoundryInvoices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting")]
        public ActionResult FoundryInvoices()
        {
            var model = new FoundryInvoiceViewModel();

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            model.FromDateStr = DateTime.Now.ToShortDateString();
            model.ToDateStr = DateTime.Now.ToShortDateString();

            return View(model);
        }

        /// <summary>
        /// search foundry invoices with filter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting")]
        public JsonResult SearchFoundryInvoices(FoundryInvoiceViewModel model)
        {
            var foundryInvoices = new List<FoundryInvoiceViewModel>();

            var tempInvoices = _foundryInvoiceRepository.GetFoundryInvoices();

            if (tempInvoices != null && tempInvoices.Count > 0)
            {
                foreach (var tempInvoice in tempInvoices)
                {
                    FoundryInvoiceViewModel convertedModel = new FoundryInvoiceConverter().ConvertToView(tempInvoice);
                    foundryInvoices.Add(convertedModel);
                }
            }

            if (model.InvoiceNumber != null && model.InvoiceNumber != string.Empty)
            {
                foundryInvoices = foundryInvoices.Where(x => x.InvoiceNumber.ToLower() == model.InvoiceNumber.ToLower()).ToList();
            }

            if (model.FoundryId != null && model.FoundryId != string.Empty && model.FoundryId != "--Select Foundry--")
            {
                foundryInvoices = foundryInvoices.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.Unscheduled)
            {
                foundryInvoices = foundryInvoices.Where(x => x.ScheduledPaymentDate == null).ToList();
            }
            else
            {
                if (model.FromDate != null && model.ToDate != null)
                {
                    var fromDate = model.FromDate;
                    var toDate = model.ToDate.AddDays(1);

                    foundryInvoices = foundryInvoices.Where(x => x.ScheduledPaymentDate >= fromDate && x.ScheduledPaymentDate <= toDate).ToList();
                }
            }

            model.FoundryInvoices = foundryInvoices.OrderBy(x => x.InvoiceNumber).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Accounting/Invoicing/EditFoundryInvoice
        /// </summary>
        /// <param name="foundryInvoiceId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting")]
        public ActionResult EditFoundryInvoice(Guid foundryInvoiceId)
        {
            var selectedInvoice = _foundryInvoiceRepository.GetFoundryInvoice(foundryInvoiceId);

            FoundryInvoiceViewModel model = new FoundryInvoiceConverter().ConvertToView(selectedInvoice);

            return View(model);
        }

        /// <summary>
        /// update foundry invoice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting")]
        public JsonResult UpdateFoundryInvoice(FoundryInvoiceViewModel model)
        {
            var operationResult = new OperationResult();

            FoundryInvoice foundryInvoice = new FoundryInvoiceConverter().ConvertToDomain(model);

            operationResult = _foundryInvoiceRepository.UpdateFoundryInvoice(foundryInvoice);

            if (!operationResult.Success)
            {
                this.AddNotification(operationResult.Message, NotificationType.ERROR);
            }
            else
            {
                this.AddNotification(operationResult.Message, NotificationType.SUCCESS);
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_foundryInvoiceRepository != null)
                {
                    _foundryInvoiceRepository.Dispose();
                    _foundryInvoiceRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}