using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using SouthlandMetals.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Controllers
{
    public class DashboardController : ApplicationBaseController
    {
        private IProjectRepository _projectRepository;
        private IRfqRepository _rfqRepository;
        private IQuoteRepository _quoteRepository;
        private ICustomerOrderRepository _customerOrderRepository;
        private IFoundryOrderRepository _foundryOrderRepository;
        private IFoundryInvoiceRepository _foundryInvoiceRepository;
        private IPackingListRepository _packingListRepository;

        public DashboardController()
        {
            _projectRepository = new ProjectRepository();
            _rfqRepository = new RfqRepository();
            _quoteRepository = new QuoteRepository();
            _customerOrderRepository = new CustomerOrderRepository();
            _foundryOrderRepository = new FoundryOrderRepository();
            _foundryInvoiceRepository = new FoundryInvoiceRepository();
            _packingListRepository = new PackingListRepository();
        }

        public DashboardController(IProjectRepository projectRepository,
                                   IRfqRepository rfqRepository,
                                   IQuoteRepository quoteRepository,
                                   ICustomerOrderRepository customerOrderRepository,
                                   IFoundryOrderRepository foundryOrderRepository,
                                   IFoundryInvoiceRepository foundryInvoiceRepository,
                                   IPackingListRepository packingListRepository)
        {
            _projectRepository = projectRepository;
            _rfqRepository = rfqRepository;
            _quoteRepository = quoteRepository;
            _customerOrderRepository = customerOrderRepository;
            _foundryOrderRepository = foundryOrderRepository;
            _foundryInvoiceRepository = foundryInvoiceRepository;
            _packingListRepository = packingListRepository;
        }

        /// <summary>
        /// rediect to dashboard depend on role of current user
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Admin", "Dashboard");
                }
                else if (User.IsInRole("Accounting"))
                {
                    return RedirectToAction("Accounting", "Dashboard");
                }
                else if (User.IsInRole("Standard"))
                {
                    return RedirectToAction("Operations", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        /// <summary>
        /// admin dashboard
        /// </summary>
        /// <returns></returns>
        public ActionResult Admin()
        {
            var model = new DashboardViewModel();
            var _db = new SouthlandDb();

            var currentDate = DateTime.Today.AddMonths(-1);

            var projects = new List<ProjectViewModel>();

            var tempProjects = _projectRepository.GetProjects().Where(x => x.IsHold).ToList();

            if(tempProjects != null && tempProjects.Count > 0)
            {
                foreach(var tempProject in tempProjects)
                {
                    ProjectViewModel convertedModel = new ProjectConverter().ConvertToListView(tempProject);

                    projects.Add(convertedModel);
                }
            }

            model.OnHoldProjects = projects;
            
            var rfqs = new List<RfqViewModel>();

            var tempRfqs = _rfqRepository.GetRfqs().Where(x => x.IsHold).ToList();

            if (tempRfqs != null && tempRfqs.Count > 0)
            {
                foreach (var tempRfq in tempRfqs)
                {
                    RfqViewModel convertedModel = new RfqConverter().ConvertToListView(tempRfq);

                    rfqs.Add(convertedModel);
                }
            }

            model.OnHoldRfqs = rfqs;

            var quotes = new List<QuoteViewModel>();

            var tempQuotes = _quoteRepository.GetQuotes().Where(x => x.IsHold).ToList();

            if (tempQuotes != null && tempQuotes.Count > 0)
            {
                foreach (var tempQuote in tempQuotes)
                {
                    QuoteViewModel convertedModel = new QuoteConverter().ConvertToListView(tempQuote);

                    quotes.Add(convertedModel);
                }
            }

            model.OnHoldQuotes = quotes;

            var customerOrders = new List<CustomerOrderViewModel>();

            var tempCustomerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => x.IsHold).ToList();

            if (tempCustomerOrders != null && tempCustomerOrders.Count > 0)
            {
                foreach (var tempCustomerOrder in tempCustomerOrders)
                {
                    CustomerOrderViewModel convertedModel = new CustomerOrderConverter().ConvertToListView(tempCustomerOrder);

                    customerOrders.Add(convertedModel);
                }
            }

            model.OnHoldCustomerOrders = customerOrders;

            var foundryOrders = new List<FoundryOrderViewModel>();

            var tempFoundryOrders = _foundryOrderRepository.GetFoundryOrders().Where(x => x.IsHold).ToList();

            if (tempFoundryOrders != null && tempFoundryOrders.Count > 0)
            {
                foreach (var tempFoundryOrder in tempFoundryOrders)
                {
                    FoundryOrderViewModel convertedModel = new FoundryOrderConverter().ConvertToListView(tempFoundryOrder);

                    foundryOrders.Add(convertedModel);
                }
            }

            model.OnHoldFoundryOrders = foundryOrders;

            var packingLists = new List<PackingListViewModel>();

            var tempPackingLists = _packingListRepository.GetPackingLists().Where(x => !x.IsClosed).ToList();

            if (tempPackingLists != null && tempPackingLists.Count > 0)
            {
                foreach (var tempPackingList in tempPackingLists)
                {
                    PackingListViewModel convertedModel = new PackingListConverter().ConvertToListView(tempPackingList);

                    packingLists.Add(convertedModel);
                }
            }

            model.OpenPackingLists = packingLists;

            return View(model);
        }

        /// <summary>
        /// accounting dashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting")]
        public ActionResult Accounting()
        {
            var model = new DashboardViewModel();
            var foundryInvoices = _foundryInvoiceRepository.GetFoundryInvoices().Where(x=>x.ScheduledPaymentDate.HasValue == false && x.ActualPaymentDate.HasValue == false).ToList();
            if (foundryInvoices != null && foundryInvoices.Count > 0)
            {
                model.NeedScheduledFoundryInvoices = new List<FoundryInvoiceViewModel>();
                foreach (var invoice in foundryInvoices)
                {
                    var convertedInvoice = new FoundryInvoiceConverter().ConvertToView(invoice);
                    model.NeedScheduledFoundryInvoices.Add(convertedInvoice);
                }
            }
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_projectRepository != null)
                {
                    _projectRepository.Dispose();
                    _projectRepository = null;
                }

                if (_rfqRepository != null)
                {
                    _rfqRepository.Dispose();
                    _rfqRepository = null;
                }

                if (_quoteRepository != null)
                {
                    _quoteRepository.Dispose();
                    _quoteRepository = null;
                }

                if (_customerOrderRepository != null)
                {
                    _customerOrderRepository.Dispose();
                    _customerOrderRepository = null;
                }

                if (_foundryOrderRepository != null)
                {
                    _foundryOrderRepository.Dispose();
                    _foundryOrderRepository = null;
                }

                if (_foundryInvoiceRepository != null)
                {
                    _foundryInvoiceRepository.Dispose();
                    _foundryInvoiceRepository = null;
                }

                if (_packingListRepository != null)
                {
                    _packingListRepository.Dispose();
                    _packingListRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}