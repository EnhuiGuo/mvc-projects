using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Models;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
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
    public class PricingController : ApplicationBaseController
    {
        private IPriceSheetRepository _priceSheetRepository;
        private IRfqRepository _rfqRepository;
        private IProjectRepository _projectRepository;
        private ICountryRepository _countryRepository;
        private IQuoteRepository _quoteRepository;
        private IPartRepository _partRepository;
        private IProjectPartRepository _projectPartRepository;
        private IPartDynamicsRepository _partDynamicsRepository;

        public PricingController()
        {
            _priceSheetRepository = new PriceSheetRepository();

            _rfqRepository = new RfqRepository();
            _projectRepository = new ProjectRepository();
            _countryRepository = new CountryRepository();
            _quoteRepository = new QuoteRepository();
            _partRepository = new PartRepository();
            _projectPartRepository = new ProjectPartRepository();
            _partDynamicsRepository = new PartDynamicsRepository();
        }

        public PricingController(IPriceSheetRepository priceSheetRepository,
                                     IRfqRepository rfqRepository,
                                     IProjectRepository projectRepository,
                                     ICountryRepository countryRepository,
                                     IQuoteRepository quoteRepository,
                                     IPartRepository partRepository,
                                     IProjectPartRepository projectPartRepository,
                                     IPartDynamicsRepository partDynamicsRepository)
        {
            _priceSheetRepository = priceSheetRepository;
            _projectPartRepository = projectPartRepository;
            _rfqRepository = rfqRepository;
            _projectRepository = projectRepository;
            _countryRepository = countryRepository;
            _quoteRepository = quoteRepository;
            _partRepository = partRepository;
            _partDynamicsRepository = partDynamicsRepository;
        }

        private string PriceSheetNumber()
        {
            var priceSheetNumber = _priceSheetRepository.PriceSheetNumber();
            return priceSheetNumber;
        }

        private OperationResult ConvertProjectPartsToParts(Guid priceSheetId)
        {
            var operationResult = new OperationResult();

            var projectParts = _projectPartRepository.GetProjectParts().Where(x => x.PriceSheetId == priceSheetId).ToList();

            if (projectParts != null && projectParts.Count > 0)
            {
                foreach (var projectPart in projectParts)
                {
                    var existingPart = _partRepository.GetPart(projectPart.PartId);

                    if (existingPart != null)
                    {
                        PartOperationModel convertedModel = new PartOperationConverter().ConvertFromProjectPart(projectPart);

                        Part part = new PartConverter().ConvertToUpdatePart(convertedModel);

                        operationResult = _partRepository.UpdatePart(part);

                        var existingProject = _projectRepository.GetProject(projectPart.ProjectId);

                        if (existingProject != null)
                        {
                            if (existingProject.Parts.FirstOrDefault(x => x.PartId == part.PartId) == null)
                            {
                                existingProject.Parts.Add(part);
                                operationResult = _projectRepository.UpdateProject(existingProject);
                            }
                        }

                        IV00101_Part_Master partMaster = new PartConverter().ConvertToUpdateMaster(convertedModel);

                        operationResult = _partDynamicsRepository.UpdatePartMaster(partMaster);

                        IV00105_Part_Currency partCurrency = new PartConverter().ConvertToUpdateCurrency(convertedModel);

                        operationResult = _partDynamicsRepository.UpdatePartCurrency(partCurrency);
                    }
                    else
                    {
                        var existingProject = _projectRepository.GetProject(projectPart.ProjectId);

                        if (existingProject != null)
                        {
                            Part part = new PartConverter().ConvertToCreatePart(projectPart);
                            existingProject.Parts.Add(part);
                            operationResult = _projectRepository.UpdateProject(existingProject);
                            existingPart = existingProject.Parts.FirstOrDefault(x => x.Number == part.Number);
                        }

                        projectPart.PartId = existingPart.PartId;

                        operationResult = _projectPartRepository.UpdateProjectPart(projectPart);

                        IV00101_Part_Master partMaster = new PartConverter().ConvertToCreateMaster(projectPart);

                        operationResult = _partDynamicsRepository.SavePartMaster(partMaster);

                        IV00105_Part_Currency partCurrency = new PartConverter().ConvertToCreateCurrency(projectPart);

                        operationResult = _partDynamicsRepository.SavePartCurrency(partCurrency);

                        IV00107_Part_Price_Option partPriceOption = new PartConverter().ConvertToCreatePriceOption(projectPart);

                        operationResult = _partDynamicsRepository.SavePartPriceOption(partPriceOption);

                        IV00108_Part_Price partPrice = new PartConverter().ConvertToCreatePrice(projectPart);

                        operationResult = _partDynamicsRepository.SavePartPrice(partPrice);

                        IV00103_Part_Vendor_Master partVendor = new PartConverter().ConvertToCreateVendor(projectPart);

                        operationResult = _partDynamicsRepository.SavePartVendorMaster(partVendor);

                        var projectPartDrawings = _projectPartRepository.GetProjectPartDrawings(projectPart.ProjectPartId);

                        if (projectPartDrawings != null && projectPartDrawings.Count > 0)
                        {
                            foreach (var projectPartDrawing in projectPartDrawings)
                            {
                                operationResult = _projectPartRepository.DeleteProjectPartDrawing(projectPartDrawing.ProjectPartDrawingId);
                            }
                        }

                        var projectPartLayouts = _projectPartRepository.GetProjectPartLayouts(projectPart.ProjectPartId);

                        if (projectPartLayouts != null && projectPartLayouts.Count > 0)
                        {
                            foreach (var projectPartLayout in projectPartLayouts)
                            {
                                operationResult = _projectPartRepository.DeleteProjectPartLayout(projectPartLayout.ProjectPartLayoutId);
                            }
                        }
                    }
                }
            }

            return operationResult;
        }

        private OperationResult EditPriceSheetParts(Guid priceSheetId)
        {
            var operationResult = new OperationResult();

            var projectParts = _projectPartRepository.GetProjectParts().Where(x => x.PriceSheetId == priceSheetId).ToList();

            if (projectParts != null && projectParts.Count > 0)
            {
                foreach (var projectPart in projectParts)
                {
                    var existingPart = _partRepository.GetPart(projectPart.PartId);

                    if (existingPart != null)
                    {
                        PartOperationModel convertedModel = new PartOperationConverter().ConvertFromProjectPart(projectPart);

                        Part part = new PartConverter().ConvertToUpdatePart(convertedModel);

                        operationResult = _partRepository.UpdatePart(part);

                        IV00101_Part_Master partMaster = new PartConverter().ConvertToUpdateMaster(convertedModel);

                        operationResult = _partDynamicsRepository.UpdatePartMaster(partMaster);

                        IV00105_Part_Currency partCurrency = new PartConverter().ConvertToUpdateCurrency(convertedModel);

                        operationResult = _partDynamicsRepository.UpdatePartCurrency(partCurrency);
                    }
                }
            }

            return operationResult;
        }

        /// <summary>
        /// GET: Operations/Pricing
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            PriceSheetListModel model = new PriceSheetListModel();

            model.PriceSheets = new List<PriceSheetListModel>();

            var priceSheets = _priceSheetRepository.GetPriceSheets().Where(x => x.IsProduction).ToList();

            if (priceSheets != null && priceSheets.Count > 0)
            {
                foreach (var priceSheet in priceSheets)
                {
                    PriceSheetListModel sheetModel = new PriceSheetConverter().ConvertToListView(priceSheet);

                    model.PriceSheets.Add(sheetModel);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Create Price Sheet
        /// </summary>
        /// <param name="priceSheet"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Create(PriceSheetViewModel priceSheet)
        {
            var operationResult = new OperationResult();

            var rfq = _rfqRepository.GetRfq(priceSheet.RfqId);

            var newPriceSheet = new PriceSheetConverter().ConvertToDomain(priceSheet);

            operationResult = _priceSheetRepository.SavePriceSheet(newPriceSheet);

            if (operationResult.Success && priceSheet.IsQuote)
            {
                rfq.IsOpen = false;
                operationResult = _rfqRepository.UpdateRfq(rfq);
            }

            var insertedPriceSheet = _priceSheetRepository.GetPriceSheet(priceSheet.Number);

            if (operationResult.Success)
            {
                if (priceSheet.CostDetailList != null && priceSheet.CostDetailList.Count > 0)
                {
                    foreach (var costDetail in priceSheet.CostDetailList)
                    {
                        var priceDetail = priceSheet.PriceDetailList.FirstOrDefault(x => x.ProjectPartId == costDetail.ProjectPartId);

                        var tempPart = _projectPartRepository.GetProjectPart(costDetail.ProjectPartId);
                        {
                            tempPart.Weight = costDetail.Weight;
                            tempPart.AnnualUsage = (int)costDetail.AnnualUsage;
                            tempPart.Cost = costDetail.Cost;
                            tempPart.PatternCost = costDetail.PatternCost;
                            tempPart.FixtureCost = costDetail.FixtureCost;
                            tempPart.PriceSheetId = insertedPriceSheet.PriceSheetId;
                            tempPart.Price = priceDetail.Price;
                            tempPart.PatternPrice = priceDetail.PatternPrice;
                            tempPart.FixturePrice = priceDetail.FixturePrice;
                        }
                        operationResult = _projectPartRepository.UpdateProjectPart(tempPart);
                    }
                }

                if (operationResult.Success && priceSheet.IsQuote)
                {
                    operationResult = _rfqRepository.UpdatePriceSheet(rfq.RfqId, priceSheet.Number);
                }
                else if (priceSheet.IsProduction && operationResult.Success)
                {
                    if (priceSheet.QuoteId != Guid.Empty)
                    {
                        var quote = _quoteRepository.GetQuote(priceSheet.QuoteId);
                        quote.IsOpen = false;
                        operationResult = _quoteRepository.UpdateQuote(quote);
                    }

                    ConvertProjectPartsToParts(insertedPriceSheet.PriceSheetId);
                }
            }

            operationResult.ReferenceId = insertedPriceSheet.PriceSheetId;

            return Json(operationResult, JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        /// GET: Operations/Pricing/Detail
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Detail(Guid priceSheetId)
        {
            var priceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId);

            PriceSheetViewModel model = new PriceSheetConverter().ConvertToView(priceSheet);

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Pricing/Edit
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Edit(Guid priceSheetId)
        {
            var priceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId);

            PriceSheetViewModel model = new PriceSheetConverter().ConvertToView(priceSheet);

            return View(model);
        }

        /// <summary>
        /// Edit price sheet
        /// </summary>
        /// <param name="priceSheet"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Edit(PriceSheetViewModel priceSheet)
        {
            var operationResult = new OperationResult();

            PriceSheet priceSheetToUpdate = new PriceSheet();

            var priceSheetId = priceSheet.PriceSheetId;

            priceSheetToUpdate = new PriceSheetConverter().ConvertToDomain(priceSheet);

            operationResult = _priceSheetRepository.UpdatePriceSheet(priceSheetToUpdate);

            if (operationResult.Success)
            {
                if (priceSheet.CostDetailList != null && priceSheet.CostDetailList.Count > 0)
                {
                    foreach (var costDetail in priceSheet.CostDetailList)
                    {
                        var priceDetail = priceSheet.PriceDetailList.FirstOrDefault(x => x.ProjectPartId == costDetail.ProjectPartId);

                        var tempPart = _projectPartRepository.GetProjectPart(costDetail.ProjectPartId);
                        {
                            tempPart.Weight = costDetail.Weight;
                            tempPart.AnnualUsage = (int)costDetail.AnnualUsage;
                            tempPart.Cost = costDetail.Cost;
                            tempPart.PatternCost = costDetail.PatternCost;
                            tempPart.FixtureCost = costDetail.FixtureCost;
                            tempPart.PriceSheetId = priceSheetId;
                            tempPart.Price = priceDetail.Price;
                            tempPart.PatternPrice = priceDetail.PatternPrice;
                            tempPart.FixturePrice = priceDetail.FixturePrice;
                        }

                        operationResult = _projectPartRepository.UpdateProjectPart(tempPart);
                    }
                }

                if (priceSheetToUpdate.IsProduction)
                {
                    operationResult = EditPriceSheetParts(priceSheetToUpdate.PriceSheetId);
                }

                operationResult.ReferenceId = priceSheetId;
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Quote price sheet create page
        /// </summary>
        /// <param name="rfqId"></param>
        /// <param name="includeRaw"></param>
        /// <param name="includeMachined"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Quote(Guid rfqId, bool includeRaw, bool includeMachined)
        {
            var model = new PriceSheetViewModel();

            var newPriceSheetNumber = PriceSheetNumber();

            model.Number = newPriceSheetNumber;
            model.RfqId = rfqId;
            model.IncludeRaw = includeRaw;
            model.IncludeMachined = includeMachined;

            _priceSheetRepository.RemovePriceSheetNumber(newPriceSheetNumber);

            var parts = new List<PriceSheetPartViewModel>();

            var tempParts = _projectPartRepository.GetProjectParts().Where(x => x.RfqId == rfqId).OrderBy(x => x.Number).ToList();

            if (tempParts != null && tempParts.Count > 0)
            {
                foreach (var tempPart in tempParts)
                {
                    if (tempPart.IsRaw && includeRaw)
                    {
                        PriceSheetPartViewModel convertedModel = new PriceSheetPartConverter().ConvertToCostView(tempPart);
                        parts.Add(convertedModel);
                    }
                    else if (tempPart.IsMachined && includeMachined)
                    {
                        PriceSheetPartViewModel convertedModel = new PriceSheetPartConverter().ConvertToCostView(tempPart);
                        parts.Add(convertedModel);
                    }
                }
            }

            model.PriceSheetParts = parts.OrderBy(x => x.PartNumber).ToList();

            return View(model);
        }

        /// <summary>
        /// convert quote price sheet infomation to production price sheet 
        /// </summary>
        /// <param name="priceSheetId">quote price sheet id</param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult ConvertToProduction(Guid priceSheetId, Guid quoteId)
        {
            var model = new PriceSheetViewModel();

            var existingPriceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId);

            if(existingPriceSheet != null)
            {
                model = new PriceSheetConverter().ConvertToView(existingPriceSheet);

                var newPriceSheetNumber = PriceSheetNumber();

                model.Number = newPriceSheetNumber;

                _priceSheetRepository.RemovePriceSheetNumber(newPriceSheetNumber);

                model.QuoteId = quoteId;

                var quoteParts = _projectPartRepository.GetProjectParts().Where(x => x.QuoteId == quoteId).ToList();

                if (model.CostDetailList != null && quoteParts != null)
                {
                    var deletePartList = new List<PriceSheetCostDetailViewModel>();

                    foreach (var costPart in model.CostDetailList)
                    {
                        var existing = quoteParts.FirstOrDefault(x => x.ProjectPartId == costPart.ProjectPartId);
                        if (existing == null)
                            deletePartList.Add(costPart);
                    }

                    if (deletePartList != null && deletePartList.Count > 0)
                    {
                        foreach (var deletePart in deletePartList)
                        {
                            model.CostDetailList.Remove(deletePart);
                        }
                    }
                } 
            }

            return View(model);
        }

        /// <summary>
        /// create production price sheet
        /// </summary>
        /// <param name="priceSheet"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult CreateProductionPriceSheet(PriceSheetViewModel priceSheet)
        {
            var operationResult = new OperationResult();

            var existingPriceSheet = _priceSheetRepository.GetPriceSheet(priceSheet.Number);

            if (existingPriceSheet == null)
            {
                priceSheet.IsProduction = true;

                return Create(priceSheet);
            }
            else
            {
                priceSheet.IsProduction = true;

                return Edit(priceSheet);
            }
        }

        /// <summary>
        /// production price sheet view page
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Production(Guid priceSheetId)
        {
            var priceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId);

            PriceSheetViewModel model = new PriceSheetConverter().ConvertToView(priceSheet);

            return View(model);
        }

        /// <summary>
        /// Delete price sheet page
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Delete(Guid priceSheetId)
        {
            var operationResult = new OperationResult();

            operationResult = _priceSheetRepository.DeletePriceSheet(priceSheetId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get all price sheets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAllPriceSheets()
        {
            var model = new PriceSheetListModel();

            var priceSheets = new List<PriceSheetListModel>();

            var tempPriceSheets = _priceSheetRepository.GetPriceSheets();

            if (tempPriceSheets != null && tempPriceSheets.Count > 0)
            {
                foreach (var tempPriceSheet in tempPriceSheets)
                {
                    var priceSheetModel = new PriceSheetConverter().ConvertToListView(tempPriceSheet);

                    priceSheets.Add(priceSheetModel);
                }
            }

            model.PriceSheets = priceSheets.OrderBy(x => x.Number).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get price sheet by id
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public PriceSheetViewModel GetPriceSheet(Guid priceSheetId)
        {
            var priceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId);

            PriceSheetViewModel model = new PriceSheetConverter().ConvertToView(priceSheet);

            return model;
        }

        /// <summary>
        /// get parts for price sheet
        /// </summary>
        /// <param name="rfqId"></param>
        /// <param name="includeRaw"></param>
        /// <param name="includeMachined"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPartsForPriceSheet(Guid rfqId, bool includeRaw, bool includeMachined)
        {
            var model = new PriceSheetPartViewModel();

            var parts = new List<PriceSheetPartViewModel>();

            var tempParts = _projectPartRepository.GetProjectParts().Where(x => x.RfqId == rfqId).OrderBy(x => x.Number).ToList();

            if (tempParts != null && tempParts.Count > 0)
            {
                foreach (var tempPart in tempParts)
                {
                    if (tempPart.IsRaw && includeRaw)
                    {
                        PriceSheetPartViewModel costPart = new PriceSheetPartConverter().ConvertToCostView(tempPart);
                        parts.Add(costPart);
                    }
                    else if (tempPart.IsMachined && includeMachined)
                    {
                        PriceSheetPartViewModel costPart = new PriceSheetPartConverter().ConvertToCostView(tempPart);
                        parts.Add(costPart);
                    }
                }
            }

            model.OrderParts = parts.OrderBy(x => x.PartNumber).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get all production price sheet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetProductionPriceSheets()
        {
            var model = new PriceSheetListModel();

            var priceSheets = new List<PriceSheetListModel>();

            var tempPriceSheets = _priceSheetRepository.GetPriceSheets().Where(x => x.IsProduction).ToList();

            if (tempPriceSheets != null && tempPriceSheets.Count > 0)
            {
                foreach (var tempPriceSheet in tempPriceSheets)
                {
                    var priceSheetModel = new PriceSheetConverter().ConvertToListView(tempPriceSheet);

                    priceSheets.Add(priceSheetModel);
                }
            }

            model.PriceSheets = priceSheets.OrderBy(x => x.Number).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get quote price sheet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetQuotePriceSheets()
        {
            var model = new PriceSheetListModel();

            var priceSheets = new List<PriceSheetListModel>();

            var tempPriceSheets = _priceSheetRepository.GetPriceSheets().Where(x => x.IsQuote).ToList();

            if (tempPriceSheets != null && tempPriceSheets.Count > 0)
            {
                foreach (var tempPriceSheet in tempPriceSheets)
                {
                    var priceSheetModel = new PriceSheetConverter().ConvertToListView(tempPriceSheet);

                    priceSheets.Add(priceSheetModel);
                }
            }

            model.PriceSheets = priceSheets.OrderBy(x => x.Number).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get price sheet by project with filter
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPriceSheetsByProject(Guid projectId, DateTime? fromDate, DateTime? toDate, string orderType)
        {
            var model = new PriceSheetViewModel();

            var priceSheets = new List<PriceSheetViewModel>();

            if (orderType.Equals("Sample") || orderType.Equals("Tooling"))
            {
                var dbPriceSheets = _priceSheetRepository.GetPriceSheets().Where(x => x.ProjectId == projectId && x.IsQuote == true).ToList();

                if (dbPriceSheets != null && dbPriceSheets.Count > 0)
                {
                    foreach (var priceSheet in dbPriceSheets)
                    {
                        var priceSheetModel = new PriceSheetConverter().ConvertToView(priceSheet);
                        priceSheets.Add(priceSheetModel);
                    }
                }
            }
            else
            {
                var dbPriceSheets = _priceSheetRepository.GetPriceSheets().Where(x => x.ProjectId == projectId && x.IsProduction == true).ToList();

                if (dbPriceSheets != null && dbPriceSheets.Count > 0)
                {
                    foreach (var priceSheet in dbPriceSheets)
                    {
                        var priceSheetModel = new PriceSheetConverter().ConvertToView(priceSheet);
                        priceSheets.Add(priceSheetModel);
                    }
                }
            }

            if (fromDate != null)
            {
                priceSheets = priceSheets.Where(x => x.DueDate >= fromDate).ToList();
            }

            if (toDate != null)
            {
                priceSheets = priceSheets.Where(x => x.DueDate <= toDate).ToList();
            }

            model.PriceSheets = priceSheets;

            return Json(model.PriceSheets, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_priceSheetRepository != null)
                {
                    _priceSheetRepository.Dispose();
                    _priceSheetRepository = null;
                }

                if (_rfqRepository != null)
                {
                    _rfqRepository.Dispose();
                    _rfqRepository = null;
                }

                if (_projectRepository != null)
                {
                    _projectRepository.Dispose();
                    _projectRepository = null;
                }

                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }

                if (_quoteRepository != null)
                {
                    _quoteRepository.Dispose();
                    _quoteRepository = null;
                }

                if (_partRepository != null)
                {
                    _partRepository.Dispose();
                    _partRepository = null;
                }

                if (_projectPartRepository != null)
                {
                    _projectPartRepository.Dispose();
                    _projectPartRepository = null;
                }

                if (_partDynamicsRepository != null)
                {
                    _partDynamicsRepository.Dispose();
                    _partDynamicsRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}