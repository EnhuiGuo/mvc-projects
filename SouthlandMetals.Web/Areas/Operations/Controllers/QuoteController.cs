using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Controllers
{
    public class QuoteController : ApplicationBaseController
    {
        private IQuoteRepository _quoteRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private ICustomerAddressDynamicsRepository _customerAddressDynamicsRepository;
        private IProjectRepository _projectRepository;
        private IShipmentTermRepository _shipmentTermRepository;
        private IPaymentTermRepository _paymentTermRepository;
        private IMaterialRepository _materialRepository;
        private ICoatingTypeRepository _coatingTypeRepository;
        private IHtsNumberRepository _htsNumberRepository;
        private IRfqRepository _rfqRepository;
        private IPriceSheetRepository _priceSheetRepository;
        private IProjectPartRepository _projectPartRepository;
        private IStateRepository _stateRepository;
        private IPartRepository _partRepository;

        public QuoteController()
        {
            _quoteRepository = new QuoteRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            _projectRepository = new ProjectRepository();
            _shipmentTermRepository = new ShipmentTermRepository();
            _paymentTermRepository = new PaymentTermRepository();
            _materialRepository = new MaterialRepository();
            _coatingTypeRepository = new CoatingTypeRepository();
            _htsNumberRepository = new HtsNumberRepository();
            _rfqRepository = new RfqRepository();
            _priceSheetRepository = new PriceSheetRepository();
            _projectPartRepository = new ProjectPartRepository();
            _stateRepository = new StateRepository();
            _partRepository = new PartRepository();
        }

        public QuoteController(IQuoteRepository quoteRepository,
                                     ICustomerDynamicsRepository customerDynamicsRepository,
                                     ICustomerAddressDynamicsRepository customerAddressDynamicsRepository,
                                     IProjectRepository projectRepository,
                                     IShipmentTermRepository shipmentTermRepository,
                                     IPaymentTermRepository paymentTermRepository,
                                     IMaterialRepository materialRepository,
                                     ICoatingTypeRepository coatingTypeRepository,
                                     IHtsNumberRepository htsNumberRepository,
                                     IRfqRepository rfqRepository,
                                     IPriceSheetRepository priceSheetRepository,
                                     IProjectPartRepository projectPartRepository,
                                     IStateRepository stateRepository,
                                     IPartRepository partRepository)
        {
            _quoteRepository = quoteRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _customerAddressDynamicsRepository = customerAddressDynamicsRepository;
            _projectRepository = projectRepository;
            _shipmentTermRepository = shipmentTermRepository;
            _paymentTermRepository = paymentTermRepository;
            _materialRepository = materialRepository;
            _coatingTypeRepository = coatingTypeRepository;
            _htsNumberRepository = htsNumberRepository;
            _rfqRepository = rfqRepository;
            _priceSheetRepository = priceSheetRepository;
            _projectPartRepository = projectPartRepository;
            _stateRepository = stateRepository;
            _partRepository = partRepository;
        }

        private string QuoteNumber()
        {
            var quoteNumber = _quoteRepository.QuoteNumber();

            return quoteNumber;
        }

        /// <summary>
        /// GET: Operations/Quote
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            QuoteViewModel model = new QuoteViewModel();

            var tempQuotes = _quoteRepository.GetQuotes().Where(x=>x.IsOpen).ToList();

            if (tempQuotes != null && tempQuotes.Count > 0)
            {
                model.Quotes = new List<QuoteViewModel>();

                foreach (var tempQuote in tempQuotes)
                {
                    QuoteViewModel convertedModel = new QuoteConverter().ConvertToListView(tempQuote);
                    model.Quotes.Add(convertedModel);
                }
            }

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Quote/Standard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Standard()
        {
            QuoteViewModel model = new QuoteViewModel();

            model.QuoteNumber = QuoteNumber();

            _quoteRepository.RemoveQuoteNumber(model.QuoteNumber);

            model.Date = DateTime.Now.ToShortDateString();
            model.QuoteDateStr = DateTime.Now.ToShortDateString();
            model.RfqNumber = "N/A";
            model.Machining = "Not Included";

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableProjects = _projectRepository.GetSelectableActiveProjects();

            var defaultProject = new SelectListItem()
            {
                Text = "--Select Project--",
                Value = null
            };

            model.SelectableProjects.Insert(0, defaultProject);

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Shipment Term--",
                Value = null
            };

            model.SelectableShipmentTerms.Insert(0, defaultShipmentTerm);

            model.SelectablePaymentTerms = _paymentTermRepository.GetSelectablePaymentTerms();

            var defaultPaymentTerm = new SelectListItem()
            {
                Text = "--Select Payment Term--",
                Value = null
            };

            model.SelectablePaymentTerms.Insert(0, defaultPaymentTerm);

            model.SelectableMaterial = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            model.SelectableMaterial.Insert(0, defaultMaterial);

            model.SelectableCoatingTypes = _coatingTypeRepository.GetSelectableCoatingTypes();

            var defaultCoatingType = new SelectListItem()
            {
                Text = "--Select Coating Type--",
                Value = null
            };

            model.SelectableCoatingTypes.Insert(0, defaultCoatingType);

            model.SelectableHtsNumbers = _htsNumberRepository.GetSelectableHtsNumbers();

            var defaultHtsNumber = new SelectListItem()
            {
                Text = "--Select HtsNumber--",
                Value = null
            };

            model.SelectableHtsNumbers.Insert(0, defaultHtsNumber);

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Quote/Convert
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Convert(Guid rfqId)
        {
            var currentRfq = _rfqRepository.GetRfq(rfqId);

            QuoteViewModel model = new QuoteConverter().ConvertToView(currentRfq);

            model.QuoteNumber = QuoteNumber();

            _quoteRepository.RemoveQuoteNumber(model.QuoteNumber);

            model.SelectableCustomerAddresses = _customerAddressDynamicsRepository.GetSelectableCustomerAddresses(model.CustomerId);

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Shipment Term--",
                Value = null
            };

            model.SelectableShipmentTerms.Insert(0, defaultShipmentTerm);

            model.SelectablePaymentTerms = _paymentTermRepository.GetSelectablePaymentTerms();

            var defaultPaymentTerm = new SelectListItem()
            {
                Text = "--Select Payment Term--",
                Value = null
            };

            model.SelectablePaymentTerms.Insert(0, defaultPaymentTerm);

            model.SelectableMaterial = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            model.SelectableMaterial.Insert(0, defaultMaterial);

            model.SelectableCoatingTypes = _coatingTypeRepository.GetSelectableCoatingTypes();

            var defaultCoatingType = new SelectListItem()
            {
                Text = "--Select Coating Type--",
                Value = null
            };

            model.SelectableCoatingTypes.Insert(0, defaultCoatingType);

            model.SelectableHtsNumbers = _htsNumberRepository.GetSelectableHtsNumbers();

            var defaultHtsNumber = new SelectListItem()
            {
                Text = "--Select HtsNumber--",
                Value = null
            };

            model.SelectableHtsNumbers.Insert(0, defaultHtsNumber);

            return View(model);
        }

        /// <summary>
        /// Create quote
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Create(QuoteViewModel model)
        {
            var operationResult = new OperationResult();

            Quote newQuote = new QuoteConverter().ConvertToDomain(model);

            operationResult = _quoteRepository.SaveQuote(newQuote);

            if (operationResult.Success)
            {
                var quoteId = operationResult.ReferenceId;

                var rfq = _rfqRepository.GetRfq(model.RfqId);

                if (rfq != null)
                {
                    rfq.IsOpen = false;
                    operationResult = _rfqRepository.UpdateRfq(rfq);
                }

                var rfqId = model.RfqId;
                var priceSheetId = model.PriceSheetId;
                var projectId = model.ProjectId;
                var customerAddressId = model.CustomerAddressId;
                var foundryId = (rfq != null) ? rfq.FoundryId : string.Empty;
                var htsNumberId = model.HtsNumberId;
                var shipmentTermId = model.ShipmentTermId;
                var paymentTermId = model.PaymentTermId;
                var materialId = model.MaterialId;
                var coatingTypeId = model.CoatingTypeId;

                var quotePriceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId ?? Guid.Empty);

                if (model.QuoteParts != null && model.QuoteParts.Count() > 0)
                {
                    foreach (var quotePart in model.QuoteParts)
                    {
                        var projectPart = new ProjectPartConverter().ConvertToDomain(quotePart);

                        projectPart.RfqId = rfqId;
                        projectPart.PriceSheetId = priceSheetId;
                        projectPart.QuoteId = quoteId;
                        projectPart.ProjectId = projectId;
                        projectPart.CustomerAddressId = customerAddressId;
                        projectPart.FoundryId = (!string.IsNullOrEmpty(foundryId)) ? foundryId : projectPart.FoundryId;
                        projectPart.HtsNumberId = htsNumberId;
                        projectPart.ShipmentTermId = shipmentTermId;
                        projectPart.PaymentTermId = paymentTermId;
                        projectPart.MaterialId = materialId;
                        projectPart.CoatingTypeId = coatingTypeId;
                        projectPart.FixtureDate = (quotePriceSheet != null) ? quotePriceSheet.CreatedDate : null;//enter from price sheet
                        projectPart.PatternDate = (quotePriceSheet != null) ? quotePriceSheet.CreatedDate : null;//enter from price sheet

                        var projectPartToUpdate = _projectPartRepository.GetProjectPart(projectPart.ProjectPartId);

                        if (projectPartToUpdate != null)
                        {
                            operationResult = _projectPartRepository.UpdateProjectPart(projectPart);
                        }
                        else
                        {
                            operationResult = _projectPartRepository.SaveProjectPart(projectPart);
                            projectPart.ProjectPartId = operationResult.ReferenceId;

                            if (quotePriceSheet != null)
                            {
                                var newPriceSheetPart = new PriceSheetPartConverter().ConvertToDomain(projectPart);

                                newPriceSheetPart.IsQuote = true;
                                newPriceSheetPart.IsProduction = false;
                                operationResult = _priceSheetRepository.SavePriceSheetPart(newPriceSheetPart);
                            }
                        }
                    }
                }

                operationResult.ReferenceId = quoteId;
            }

           

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Quote/Detail
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Detail(Guid quoteId)
        {
            var quote = _quoteRepository.GetQuote(quoteId);

            QuoteViewModel model = new QuoteConverter().ConvertToView(quote);

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Quote/Edit
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Edit(Guid quoteId)
        {
            QuoteViewModel model = new QuoteViewModel();

            var currentQuote = _quoteRepository.GetQuote(quoteId);

            model = new QuoteConverter().ConvertToView(currentQuote);

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Shipment Term--",
                Value = null
            };

            model.SelectableShipmentTerms.Insert(0, defaultShipmentTerm);

            model.SelectablePaymentTerms = _paymentTermRepository.GetSelectablePaymentTerms();

            var defaultPaymentTerm = new SelectListItem()
            {
                Text = "--Select Payment Term--",
                Value = null
            };

            model.SelectablePaymentTerms.Insert(0, defaultPaymentTerm);

            model.SelectableMaterial = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            model.SelectableMaterial.Insert(0, defaultMaterial);

            model.SelectableCoatingTypes = _coatingTypeRepository.GetSelectableCoatingTypes();

            var defaultCoatingType = new SelectListItem()
            {
                Text = "--Select Coating Type--",
                Value = null
            };

            model.SelectableCoatingTypes.Insert(0, defaultCoatingType);

            var htsNumbers = _htsNumberRepository.GetSelectableHtsNumbers();

            var defaultHtsNumber = new SelectListItem()
            {
                Text = "--Select Hts Number--",
                Value = null
            };

            htsNumbers.Insert(0, defaultHtsNumber);

            model.SelectableHtsNumbers = htsNumbers;

            model.CurrentUser = User.Identity.Name;

            return View(model);
        }

        /// <summary>
        /// edit quote
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Edit(QuoteViewModel model)
        {
            var operationResult = new OperationResult();

            var quoteToUpdate = new QuoteConverter().ConvertToDomain(model);

            operationResult = _quoteRepository.UpdateQuote(quoteToUpdate);

            if (operationResult.Success)
            {
                model.Success = true;
                model.IsHold = quoteToUpdate.IsHold;
                model.IsCanceled = quoteToUpdate.IsCanceled;

                var rfq = _rfqRepository.GetRfq(quoteToUpdate.RfqId);
                var rfqId = quoteToUpdate.RfqId;
                var priceSheetId = quoteToUpdate.PriceSheetId;
                var quoteId = model.QuoteId;
                var projectId = quoteToUpdate.ProjectId;
                var customerAddressId = quoteToUpdate.CustomerAddressId;
                var foundryId = (rfq != null) ? rfq.FoundryId : string.Empty;
                var htsNumberId = model.HtsNumberId;
                var shipmentTermId = model.ShipmentTermId;
                var paymentTermId = model.PaymentTermId;
                var materialId = quoteToUpdate.MaterialId;
                var coatingTypeId = quoteToUpdate.CoatingTypeId;

                var quotePriceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId ?? Guid.Empty);

                if (model.QuoteParts != null && model.QuoteParts.Count() > 0)
                {
                    foreach (var quotePart in model.QuoteParts)
                    {
                        var projectPart = new ProjectPartConverter().ConvertToDomain(quotePart);

                        projectPart.RfqId = rfqId;
                        projectPart.PriceSheetId = priceSheetId;
                        projectPart.QuoteId = quoteId;
                        projectPart.ProjectId = projectId;
                        projectPart.CustomerAddressId = customerAddressId;
                        projectPart.FoundryId = (string.IsNullOrEmpty(foundryId)) ? foundryId : projectPart.FoundryId;
                        projectPart.HtsNumberId = htsNumberId;
                        projectPart.ShipmentTermId = shipmentTermId;
                        projectPart.PaymentTermId = paymentTermId;
                        projectPart.MaterialId = materialId;
                        projectPart.MaterialSpecificationId = materialId;
                        projectPart.CoatingTypeId = coatingTypeId;
                        projectPart.FixtureDate = (quotePriceSheet != null) ? quotePriceSheet.CreatedDate : null;//enter from price sheet
                        projectPart.PatternDate = (quotePriceSheet != null) ? quotePriceSheet.CreatedDate : null;//enter from price sheet

                        var projectPartToUpdate = _projectPartRepository.GetProjectPart(projectPart.ProjectPartId);

                        if (projectPartToUpdate != null)
                        {
                            operationResult = _projectPartRepository.UpdateProjectPart(projectPart);
                        }
                        else
                        {
                            operationResult = _projectPartRepository.SaveProjectPart(projectPart);
                            projectPart.ProjectPartId = operationResult.ReferenceId;

                            if (quotePriceSheet != null)
                            {
                                var newPriceSheetPart = new PriceSheetPartConverter().ConvertToDomain(projectPart);

                                newPriceSheetPart.IsQuote = true;
                                newPriceSheetPart.IsProduction = false;
                                operationResult = _priceSheetRepository.SavePriceSheetPart(newPriceSheetPart);
                            }
                        }
                    }
                }

                var existingParts = _projectPartRepository.GetProjectParts().Where(x => x.QuoteId == quoteId).ToList();

                if (existingParts != null)
                {
                    foreach (var existingPart in existingParts)
                    {
                        var part = model.QuoteParts.FirstOrDefault(x => x.PartNumber == existingPart.Number);

                        if (part == null)
                        {
                            existingPart.QuoteId = null;
                            operationResult = _projectPartRepository.UpdateProjectPart(existingPart);
                        }
                    }
                }

                if (!operationResult.Success)
                {
                    model.Success = false;
                    model.Message = operationResult.Message;
                }
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete quote
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Delete(Guid quoteId)
        {
            var operationResult = new OperationResult();

            var quoteParts = _projectPartRepository.GetProjectParts().Where(x => x.QuoteId == quoteId).ToList();

            if (quoteParts != null && quoteParts.Count() > 0)
            {
                foreach (var quotePart in quoteParts)
                {
                    quotePart.QuoteId = null;

                    _projectPartRepository.UpdateProjectPart(quotePart);
                }
            }

            operationResult = _quoteRepository.DeleteQuote(quoteId);

            var existingQuote = _quoteRepository.GetQuote(quoteId);

            if (existingQuote != null)
            {
                var existingRfq = _rfqRepository.GetRfq(existingQuote.QuoteId);

                if (existingRfq != null)
                {
                    existingRfq.IsOpen = true;
                    existingRfq.IsCanceled = false;
                    existingRfq.IsHold = false;

                    operationResult = _rfqRepository.UpdateRfq(existingRfq);
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add quote part modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddQuotePart()
        {
            return PartialView();
        }

        /// <summary>
        /// edit quote part modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditQuotePart()
        {
            return PartialView();
        }

        /// <summary>
        /// get all quotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAllQuotes()
        {
            QuoteViewModel model = new QuoteViewModel();

            var tempQuotes = _quoteRepository.GetQuotes();

            if (tempQuotes != null && tempQuotes.Count > 0)
            {
                model.Quotes = new List<QuoteViewModel>();

                foreach (var tempQuote in tempQuotes)
                {
                    QuoteViewModel convertedModel = new QuoteConverter().ConvertToListView(tempQuote);
                    model.Quotes.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get open quotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOpenQuotes()
        {
            var model = new QuoteViewModel();

            var tempQuotes = _quoteRepository.GetQuotes().Where(x => x.IsOpen).ToList();

            if (tempQuotes != null && tempQuotes.Count > 0)
            {
                model.Quotes = new List<QuoteViewModel>();

                foreach (var tempQuote in tempQuotes)
                {
                    QuoteViewModel convertedModel = new QuoteConverter().ConvertToListView(tempQuote);
                    model.Quotes.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get hold quotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetHoldQuotes()
        {
            var model = new QuoteViewModel();

            var tempQuotes = _quoteRepository.GetQuotes().Where(x => x.IsHold).ToList();

            if (tempQuotes != null && tempQuotes.Count > 0)
            {
                model.Quotes = new List<QuoteViewModel>();

                foreach (var tempQuote in tempQuotes)
                {
                    QuoteViewModel convertedModel = new QuoteConverter().ConvertToListView(tempQuote);
                    model.Quotes.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get canceled quotes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCanceledQuotes()
        {
            var model = new QuoteViewModel();

            var tempQuotes = _quoteRepository.GetQuotes().Where(x => x.IsCanceled).ToList();

            if (tempQuotes != null && tempQuotes.Count > 0)
            {
                model.Quotes = new List<QuoteViewModel>();

                foreach (var quote in tempQuotes)
                {
                    QuoteViewModel convertedModel = new QuoteConverter().ConvertToListView(quote);
                    model.Quotes.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get quote part by part id
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetQuotePartByPart(Guid partId)
        {
            var part = _partRepository.GetPart(partId);

            QuotePartViewModel model = new QuotePartConverter().ConvertToView(part);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_quoteRepository != null)
                {
                    _quoteRepository.Dispose();
                    _quoteRepository = null;
                }

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
                }

                if (_customerAddressDynamicsRepository != null)
                {
                    _customerAddressDynamicsRepository.Dispose();
                    _customerAddressDynamicsRepository = null;
                }

                if (_projectRepository != null)
                {
                    _projectRepository.Dispose();
                    _projectRepository = null;
                }

                if (_shipmentTermRepository != null)
                {
                    _shipmentTermRepository.Dispose();
                    _shipmentTermRepository = null;
                }

                if (_paymentTermRepository != null)
                {
                    _paymentTermRepository.Dispose();
                    _paymentTermRepository = null;
                }

                if (_materialRepository != null)
                {
                    _materialRepository.Dispose();
                    _materialRepository = null;
                }

                if (_coatingTypeRepository != null)
                {
                    _coatingTypeRepository.Dispose();
                    _coatingTypeRepository = null;
                }

                if (_htsNumberRepository != null)
                {
                    _htsNumberRepository.Dispose();
                    _htsNumberRepository = null;
                }

                if (_rfqRepository != null)
                {
                    _rfqRepository.Dispose();
                    _rfqRepository = null;
                }

                if (_priceSheetRepository != null)
                {
                    _priceSheetRepository.Dispose();
                    _priceSheetRepository = null;
                }

                if (_projectPartRepository != null)
                {
                    _projectPartRepository.Dispose();
                    _projectPartRepository = null;
                }

                if (_stateRepository != null)
                {
                    _stateRepository.Dispose();
                    _stateRepository = null;
                }

                if (_partRepository != null)
                {
                    _partRepository.Dispose();
                    _partRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}