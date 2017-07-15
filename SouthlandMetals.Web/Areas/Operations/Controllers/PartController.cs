using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Models;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SouthlandMetals.Web.Areas.Operations.Controllers
{
    public class PartController : ApplicationBaseController
    {
        private IProjectRepository _projectRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private ICustomerAddressDynamicsRepository _customerAddressDynamicsRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private IShipmentTermRepository _shipmentTermRepository;
        private IMaterialRepository _materialRepository;
        private ICoatingTypeRepository _coatingTypeRepository;
        private ISpecificationMaterialRepository _specificationMaterialRepository;
        private IProjectPartRepository _projectPartRepository;
        private IPriceSheetRepository _priceSheetRepository;
        private IPartRepository _partRepository;
        private IPartDynamicsRepository _partDynamicsRepository;
        private IHtsNumberRepository _htsNumberRepository;
        private IPartStatusRepository _partStatusRepository;
        private IPartTypeRepository _partTypeRepository;
        private IPaymentTermRepository _paymentTermRepository;
        private ISurchargeRepository _surchargeRepository;
        private ISiteDynamicsRepository _siteDynamicsRepository;
        private IPatternMaterialRepository _patternMaterialRepository;
        private IDestinationRepository _destinationRepository;
        private IFoundryOrderRepository _foundryOrderRepository;
        private ICustomerOrderRepository _customerOrderRepository;
        private IAccountCodeRepository _accountCodeRepository;

        public PartController()
        {
            _projectRepository = new ProjectRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _shipmentTermRepository = new ShipmentTermRepository();
            _materialRepository = new MaterialRepository();
            _coatingTypeRepository = new CoatingTypeRepository();
            _specificationMaterialRepository = new SpecificationMaterialRepository();
            _projectPartRepository = new ProjectPartRepository();
            _priceSheetRepository = new PriceSheetRepository();
            _partRepository = new PartRepository();
            _partDynamicsRepository = new PartDynamicsRepository();
            _htsNumberRepository = new HtsNumberRepository();
            _partStatusRepository = new PartStatusRepository();
            _partTypeRepository = new PartTypeRepository();
            _paymentTermRepository = new PaymentTermRepository();
            _surchargeRepository = new SurchargeRepository();
            _siteDynamicsRepository = new SiteDynamicsRepository();
            _patternMaterialRepository = new PatternMaterialRepository();
            _destinationRepository = new DestinationRepository();
            _foundryOrderRepository = new FoundryOrderRepository();
            _customerOrderRepository = new CustomerOrderRepository();
            _accountCodeRepository = new AccountCodeRepository();
        }

        public PartController(IProjectRepository projectRepository,
                                ICustomerDynamicsRepository customerDynamicsRepository,
                                ICustomerAddressDynamicsRepository customerAddressDynamicsRepository,
                                IFoundryDynamicsRepository foundryDynamicsRepository,
                                IShipmentTermRepository shipmentTermRepository,
                                IMaterialRepository materialRepository,
                                ICoatingTypeRepository coatingTypeRepository,
                                ISpecificationMaterialRepository specificationMaterialRepository,
                                IProjectPartRepository projectPartRepository,
                                IPriceSheetRepository priceSheetRepository,
                                IPartRepository partRepository,
                                IPartDynamicsRepository partDynamicsRepository,
                                IHtsNumberRepository htsNumberRepository,
                                IPartStatusRepository partStatusRepository,
                                IPartTypeRepository partTypeRepository,
                                IPaymentTermRepository paymentTermRepository,
                                ISurchargeRepository surchargeRepository,
                                ISiteDynamicsRepository siteDynamicsRepository,
                                IPatternMaterialRepository patternMaterialRepository,
                                IDestinationRepository destinationRepository,
                                IFoundryOrderRepository foundryOrderRepository,
                                ICustomerOrderRepository customerOrderRepository,
                                IAccountCodeRepository accountCodeRepository)
        {
            _projectRepository = projectRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _customerAddressDynamicsRepository = customerAddressDynamicsRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _shipmentTermRepository = shipmentTermRepository;
            _materialRepository = materialRepository;
            _coatingTypeRepository = coatingTypeRepository;
            _specificationMaterialRepository = specificationMaterialRepository;
            _projectPartRepository = projectPartRepository;
            _priceSheetRepository = priceSheetRepository;
            _partRepository = partRepository;
            _htsNumberRepository = htsNumberRepository;
            _partStatusRepository = partStatusRepository;
            _partDynamicsRepository = partDynamicsRepository;
            _partTypeRepository = partTypeRepository;
            _paymentTermRepository = paymentTermRepository;
            _surchargeRepository = surchargeRepository;
            _siteDynamicsRepository = siteDynamicsRepository;
            _patternMaterialRepository = patternMaterialRepository;
            _destinationRepository = destinationRepository;
            _foundryOrderRepository = foundryOrderRepository;
            _customerOrderRepository = customerOrderRepository;
            _accountCodeRepository = accountCodeRepository;
        }

        /// <summary>
        /// GET: Operations/Part
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            var model = new PartViewModel();

            model.SelectableProjects = _projectRepository.GetSelectableProjects();

            var defaultProject = new SelectListItem()
            {
                Text = "--Select Project--",
                Value = null
            };

            model.SelectableProjects.Insert(0, defaultProject);

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            var htsNumbers = _htsNumberRepository.GetSelectableHtsNumbers();

            var defaultHtsNumber = new SelectListItem()
            {
                Text = "--Select HtsNumber--",
                Value = null
            };

            htsNumbers.Insert(0, defaultHtsNumber);

            model.SelectableHtsNumbers = htsNumbers;

            var partStates = _partStatusRepository.GetSelectablePartStates();

            var defaultPartStatus = new SelectListItem()
            {
                Text = "--Select Part Status--",
                Value = null
            };

            partStates.Insert(0, defaultPartStatus);

            model.SelectablePartStates = partStates;

            var partTypes = _partTypeRepository.GetSelectablePartTypes();

            var defaultPartType = new SelectListItem()
            {
                Text = "--Select Part Type--",
                Value = null
            };

            partTypes.Insert(0, defaultPartType);

            model.SelectablePartTypes = partTypes;

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Part/Detail
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Detail(Guid partId)
        {
            var model = new PartViewModel();

            var currentPart = _projectPartRepository.GetProjectPart(partId);

            if (currentPart != null)
            {
                var projectPart = _projectPartRepository.GetProjectPart(partId);

                model = new ProjectPartConverter().ConvertToView(projectPart);
            }
            else
            {
                var part = _partRepository.GetPart(partId);

                model = new PartConverter().ConvertToView(part);
            }

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            var htsNumbers = _htsNumberRepository.GetSelectableHtsNumbers();

            var defaultHtsNumber = new SelectListItem()
            {
                Text = "--Select Hts Number--",
                Value = null
            };

            htsNumbers.Insert(0, defaultHtsNumber);

            model.SelectableHtsNumbers = htsNumbers;

            var material = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            material.Insert(0, defaultMaterial);

            model.SelectableMaterial = material;

            var specificationMaterial = _specificationMaterialRepository.GetSelectableSpecificationMaterials();

            var defaultSpecification = new SelectListItem()
            {
                Text = "--Select Material Specification--",
                Value = null
            };

            specificationMaterial.Insert(0, defaultSpecification);

            model.SelectableSpecificationMaterial = specificationMaterial;

            var partStates = _partStatusRepository.GetSelectablePartStates();

            var defaultPartStatus = new SelectListItem()
            {
                Text = "--Select Part Status--",
                Value = null
            };

            partStates.Insert(0, defaultPartStatus);

            model.SelectablePartStates = partStates;

            var destinations = _destinationRepository.GetSelectableDestinations();

            var defaultDestination = new SelectListItem()
            {
                Text = "--Select Destination--",
                Value = null
            };

            destinations.Insert(0, defaultDestination);

            model.SelectableDestinations = destinations;

            var partTypes = _partTypeRepository.GetSelectablePartTypes();

            var defaultPartType = new SelectListItem()
            {
                Text = "--Select Part Type--",
                Value = null
            };

            partTypes.Insert(0, defaultPartType);

            model.SelectablePartTypes = partTypes;

            model.SelectableProjects = _projectRepository.GetSelectablePartProjects(partId);

            var paymentTerms = _paymentTermRepository.GetSelectablePaymentTerms();

            var defaultPaymentTerm = new SelectListItem()
            {
                Text = "--Select Payment Term--",
                Value = null
            };

            paymentTerms.Insert(0, defaultPaymentTerm);

            model.SelectablePaymentTerms = paymentTerms;

            var shipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Shipment Term--",
                Value = null
            };

            shipmentTerms.Insert(0, defaultShipmentTerm);

            model.SelectableShipmentTerms = shipmentTerms;

            var surcharge = _surchargeRepository.GetSelectableSurcharges();

            model.SelectableSurcharge = surcharge;

            var defaultSurcharge = new SelectListItem()
            {
                Text = "--Select Surcharge--",
                Value = null
            };

            surcharge.Insert(0, defaultSurcharge);

            model.SelectableSites = _siteDynamicsRepository.GetSelectableSites();

            var defaultSite = new SelectListItem()
            {
                Text = "--Select Site--",
                Value = null
            };

            model.SelectableSites.Insert(0, defaultSite);

            var coatingTypes = _coatingTypeRepository.GetSelectableCoatingTypes();

            var defaultCoatingType = new SelectListItem()
            {
                Text = "--Select Coating Type--",
                Value = null
            };

            coatingTypes.Insert(0, defaultCoatingType);

            model.SelectableCoatingTypes = coatingTypes;

            var patternMaterial = _patternMaterialRepository.GetSelectablePatternMaterials();

            var defaultPattern = new SelectListItem()
            {
                Text = "--Select Pattern Material--",
                Value = null
            };

            patternMaterial.Insert(0, defaultPattern);

            model.SelectablePatternMaterial = patternMaterial;

            return View(model);
        }

        /// <summary>
        /// get parts with a filter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SearchParts(PartViewModel model)
        {
            var parts = new List<PartViewModel>();

            var tempParts = _partRepository.GetParts();

            if (model.PartNumber != null && model.PartNumber != string.Empty)
            {
                tempParts = tempParts.Where(x => x.Number.ToLower().Contains(model.PartNumber.ToLower())).ToList();
            }

            if (model.ProjectId != null && model.ProjectId != Guid.Empty)
            {
                tempParts = tempParts.Where(x => x.Projects.Any(y => y.ProjectId == model.ProjectId)).ToList();
            }

            if (model.CustomerId != null && model.CustomerId != string.Empty)
            {
                tempParts = tempParts.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.FoundryId != null && model.FoundryId != string.Empty)
            {
                tempParts = tempParts.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.HtsNumberId != null && model.HtsNumberId != Guid.Empty)
            {
                tempParts = tempParts.Where(x => x.HtsNumberId == model.HtsNumberId).ToList();
            }

            if (model.MaterialId != null && model.MaterialId != Guid.Empty)
            {
                tempParts = tempParts.Where(x => x.MaterialId == model.MaterialId).ToList();
            }

            if (model.PartStatusId != null && model.PartStatusId != Guid.Empty)
            {
                tempParts = tempParts.Where(x => x.PartStatusId == model.PartStatusId).ToList();
            }
            else
            {
                var activeStatus = _partStatusRepository.GetPartStates().FirstOrDefault(x => x.Description.ToLower() == "active");

                tempParts = tempParts.Where(x => x.PartStatusId == activeStatus.PartStatusId).ToList();
            }

            if (model.PartTypeId != null && model.PartTypeId != Guid.Empty)
            {
                tempParts = tempParts.Where(x => x.PartTypeId == model.PartTypeId).ToList();
            }

            var partsQuery = new List<PartViewModel>();

            if (tempParts != null && tempParts.Count > 0)
            {
                foreach (var tempPart in tempParts)
                {
                    PartViewModel convertedModel = new PartConverter().ConvertToListView(tempPart);

                    partsQuery.Add(convertedModel);
                }
            }

            parts = partsQuery.OrderBy(x => x.PartNumber).ToList();

            var jsonResult = Json(parts, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// add drawing to part
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult AddDrawing(HttpPostedFileBase drawing, Guid partId)
        {
            var operationResult = new OperationResult();

            var model = new DrawingPdf();

            if (drawing != null && drawing.ContentLength > 0)
            {
                byte[] tempFile = new byte[drawing.ContentLength];
                drawing.InputStream.Read(tempFile, 0, drawing.ContentLength);

                var currentPart = _projectPartRepository.GetProjectPart(partId);

                if (currentPart != null)
                {
                    var partDrawings = _projectPartRepository.GetProjectPartDrawings(partId);

                    foreach (var partDrawing in partDrawings)
                    {
                        partDrawing.IsLatest = false;
                        _projectPartRepository.UpdateProjectPartDrawing(partDrawing);
                    }

                    var newPartDrawing = new ProjectPartDrawingConverter().ConvertToDomain(drawing);

                    newPartDrawing.ProjectPartId = partId;
                    newPartDrawing.IsLatest = true;
                    newPartDrawing.IsMachined = currentPart.IsMachined;
                    newPartDrawing.IsRaw = currentPart.IsRaw;

                    operationResult = _projectPartRepository.SaveProjectPartDrawing(newPartDrawing);

                    model.Success = true;
                    model.DrawingId = operationResult.ReferenceId;
                    model.RevisionNumber = operationResult.Number;
                    model.IsProject = true;
                }
                else
                {
                    var newPartDrawing = new PartDrawingConverter().ConvertToDomain(drawing);

                    newPartDrawing.PartId = partId;
                    newPartDrawing.IsLatest = true;
                    newPartDrawing.IsMachined = false;
                    newPartDrawing.IsRaw = false;

                    operationResult = _partRepository.SavePartDrawing(newPartDrawing);

                    if (operationResult.Success)
                    {
                        var partDrawings = _partRepository.GetPartDrawings(partId);

                        foreach (var partDrawing in partDrawings)
                        {
                            partDrawing.IsLatest = false;
                            _partRepository.UpdatePartDrawing(partDrawing);
                        }

                        model.Success = true;
                        model.DrawingId = operationResult.ReferenceId;
                        model.RevisionNumber = operationResult.Number;
                        model.IsProject = true;
                    }
                    else
                    {
                        model.Success = false;
                        model.Message = operationResult.Message;
                    }
                }
            }
            else
            {
                model.Success = false;
                model.Message = "Unable to add drawing.";
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete part drawing
        /// </summary>
        /// <param name="partId"></param>
        /// <param name="drawingId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteDrawing(Guid partId, Guid drawingId)
        {
            var operationResult = new OperationResult();

            var projectDrawingToDelete = _projectPartRepository.GetProjectPartDrawing(drawingId);

            if (projectDrawingToDelete != null)
            {
                operationResult = _projectPartRepository.DeleteProjectPartDrawing(drawingId);

                if (operationResult.Success)
                {
                    var latestDrawing = _projectPartRepository.GetProjectPartDrawings(partId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                    if (latestDrawing != null)
                    {
                        latestDrawing.IsLatest = true;
                    }

                    operationResult = _projectPartRepository.UpdateProjectPartDrawing(latestDrawing);
                }
            }
            else
            {
                var drawingToDelete = _partRepository.GetPartDrawing(drawingId);

                if (drawingToDelete != null)
                {
                    operationResult = _partRepository.DeletePartDrawing(drawingToDelete);

                    if (operationResult.Success)
                    {
                        var latestDrawing = _partRepository.GetPartDrawings(partId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                        if (latestDrawing != null)
                        {
                            latestDrawing.IsLatest = true;

                            operationResult = _partRepository.UpdatePartDrawing(latestDrawing);
                        }
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit part
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditPart(PartOperationModel model)
        {
            var operationResult = new OperationResult();

            if (model.IsProjectPart)
            {
                var currentPart = _projectPartRepository.GetProjectPart(model.ProjectPartId);

                if (currentPart != null)
                {
                    currentPart = new ProjectPartConverter().ConvertToDomain(model);

                    operationResult = _projectPartRepository.UpdateProjectPart(currentPart);

                    if (operationResult.Success)
                    {
                        var partToEdit = _priceSheetRepository.GetProductionPriceSheetPartByProjectPart(currentPart.ProjectPartId);

                        if (partToEdit != null)
                        {
                            partToEdit.Cost = model.Cost;
                            partToEdit.Price = model.Price;
                            partToEdit.AnnualCost = model.Cost * partToEdit.AnnualUsage;
                            partToEdit.AnnualPrice = model.Price * partToEdit.AnnualUsage;

                            operationResult = _priceSheetRepository.UpdatePriceSheetPart(partToEdit);

                            var totalAnnualCost = _priceSheetRepository.GetPriceSheetParts(partToEdit.PriceSheetId).Select(x => x.AnnualCost).Sum();
                            var totalAnnualPrice = _priceSheetRepository.GetPriceSheetParts(partToEdit.PriceSheetId).Select(x => x.AnnualPrice).Sum();

                            var priceSheetToEdit = _priceSheetRepository.GetPriceSheet(partToEdit.PriceSheetId);

                            priceSheetToEdit.AnnualMargin = (totalAnnualPrice - totalAnnualCost) / totalAnnualCost;

                            operationResult = _priceSheetRepository.UpdatePriceSheet(priceSheetToEdit);
                        }
                    }
                }
            }
            else
            {
                var existingPart = _partRepository.GetPart(model.PartId);

                if (existingPart != null)
                {
                    Part part = new PartConverter().ConvertToUpdatePart(model);

                    operationResult = _partRepository.UpdatePart(part);
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Part/Layout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Layout()
        {
            return View();
        }

        /// <summary>
        /// add layout to part
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult AddLayout(HttpPostedFileBase layout, Guid partId)
        {
            var operationResult = new OperationResult();

            var model = new LayoutPdf();

            if (layout != null && layout.ContentLength > 0)
            {
                byte[] tempFile = new byte[layout.ContentLength];
                layout.InputStream.Read(tempFile, 0, layout.ContentLength);

                var currentPart = _projectPartRepository.GetProjectPart(partId);

                if (currentPart != null)
                {
                    var newPartLayout = new ProjectPartLayoutConverter().ConvertToDomain(layout);

                    newPartLayout.ProjectPartId = partId;
                    newPartLayout.IsLatest = true;
                    newPartLayout.IsMachined = currentPart.IsMachined;
                    newPartLayout.IsRaw = currentPart.IsRaw;

                    operationResult = _projectPartRepository.SaveProjectPartLayout(newPartLayout);

                    if (operationResult.Success)
                    {
                        var partLayouts = _projectPartRepository.GetProjectPartLayouts(partId);

                        foreach (var partLayout in partLayouts)
                        {
                            partLayout.IsLatest = false;
                            _projectPartRepository.UpdateProjectPartLayout(partLayout);
                        }

                        model.Success = true;
                        model.LayoutId = operationResult.ReferenceId;
                        model.Description = operationResult.Description;
                        model.IsProject = true; 
                    }
                    else
                    {
                        model.Success = false;
                        model.Message = operationResult.Message;
                    }
                }
                else
                {
                    var newPartLayout = new PartLayoutConverter().ConvertToDomain(layout);

                    newPartLayout.PartId = partId;
                    newPartLayout.IsLatest = true;
                    newPartLayout.IsMachined = false;
                    newPartLayout.IsRaw = false;

                    operationResult = _partRepository.SavePartLayout(newPartLayout);

                    if (operationResult.Success)
                    {
                        var partLayouts = _partRepository.GetPartLayouts(partId);

                        foreach (var partLayout in partLayouts)
                        {
                            partLayout.IsLatest = false;
                            _partRepository.UpdatePartLayout(partLayout);
                        }

                        model.Success = true;
                        model.LayoutId = operationResult.ReferenceId;
                        model.Description = operationResult.Description;
                        model.IsProject = true;
                    }
                    else
                    {
                        model.Success = false;
                        model.Message = operationResult.Message;
                    }
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete part layout
        /// </summary>
        /// <param name="partId"></param>
        /// <param name="layoutId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteLayout(Guid partId, Guid layoutId)
        {
            var operationResult = new OperationResult();

            var projectLayoutToDelete = _projectPartRepository.GetProjectPartLayout(layoutId);

            if (projectLayoutToDelete != null)
            {
                operationResult = _projectPartRepository.DeleteProjectPartLayout(projectLayoutToDelete.ProjectPartLayoutId);

                if (operationResult.Success)
                {
                    var latestLayout = _projectPartRepository.GetProjectPartLayouts(partId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                    if (latestLayout != null)
                    {
                        latestLayout.IsLatest = true;

                        operationResult = _projectPartRepository.UpdateProjectPartLayout(latestLayout);
                    }
                }
            }
            else
            {
                var layoutToDelete = _partRepository.GetPartLayout(layoutId);

                if (layoutToDelete != null)
                {
                    operationResult = _partRepository.DeletePartLayout(layoutToDelete);

                    if (operationResult.Success)
                    {
                        var latestLayout = _partRepository.GetPartLayouts(partId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                        if (latestLayout != null)
                        {
                            latestLayout.IsLatest = true;

                            operationResult = _partRepository.UpdatePartLayout(latestLayout);
                        }
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Part/Tooling
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Tooling()
        {
            return View();
        }

        /// <summary>
        /// get part by number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SearchProjectPart(string number)
        {
            var model = new PartViewModel();

            var projectPart = _projectPartRepository.GetProjectPart(number);

            if (projectPart == null)
            {
                var part = _partRepository.GetPart(number);

                if (part != null)
                {
                    model.PartId = part.PartId;
                }
            }
            else
            {
                model.PartId = projectPart.PartId;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Part/Update
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Update()
        {
            return View();
        }

        /// <summary>
        /// get part by id
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPart(Guid partId)
        {
            var part = _partRepository.GetPart(partId);

            PartViewModel model = new PartConverter().ConvertToView(part);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get drawing data
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetDrawingImageData(HttpPostedFileBase drawing)
        {
            var model = new DrawingPdf();

            if (drawing == null || drawing.ContentLength < 1)
            {
                model.Success = false;
                model.Message = "Error occurred...image file not found or contains no data.";
            }
            else
            {
                if (!drawing.FileName.EndsWith(".png") && !drawing.FileName.EndsWith(".jpg") && !drawing.FileName.EndsWith(".pdf"))
                {
                    model.Success = false;
                    model.Message = "Error occurred...image file not in correct format.";
                }
                else
                {
                    string trimmedFileName = string.Empty;

                    byte[] tempFile = new byte[drawing.ContentLength];
                    drawing.InputStream.Read(tempFile, 0, drawing.ContentLength);

                    if (drawing.FileName.EndsWith("png"))
                    {
                        trimmedFileName = drawing.FileName.Replace(".png", "");
                    }
                    else if (drawing.FileName.EndsWith("jpg"))
                    {
                        trimmedFileName = drawing.FileName.Replace(".jpg", "");
                    }
                    else if (drawing.FileName.EndsWith("pdf"))
                    {
                        trimmedFileName = drawing.FileName.Replace(".pdf", "");
                    }

                    model.Success = true;
                    model.RevisionNumber = trimmedFileName;
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get drawing by id
        /// </summary>
        /// <param name="drawingId"></param>
        /// <param name="isProject"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public FileStreamResult GetDrawing(Guid drawingId, bool isProject)
        {
            var drawing = new DrawingPdf();

            if (isProject)
            {
                var projectDrawing = _projectPartRepository.GetProjectPartDrawing(drawingId);

                drawing = new ProjectPartDrawingConverter().ConvertToPdf(projectDrawing);
            }
            else
            {
                var partDrawing = _partRepository.GetPartDrawing(drawingId);

                drawing = new PartDrawingConverter().ConvertToPdf(partDrawing);
            }

            MemoryStream ms = new MemoryStream(drawing.Content, 0, 0, true, true);
            Response.ContentType = drawing.Type;
            Response.AddHeader("content-disposition", "inline;filename=" + drawing.RevisionNumber);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            string mineType = "application/pdf";

            return new FileStreamResult(Response.OutputStream, mineType);
        }

        /// <summary>
        /// get layout data
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetLayoutImageData(HttpPostedFileBase layout)
        {
            var model = new LayoutPdf();

            if (layout == null && layout.ContentLength < 1)
            {
                model.Success = false;
                model.Message = "Error occurred...image file not found or contains no data.";
            }
            else
            {
                if (!layout.FileName.EndsWith(".png") && !layout.FileName.EndsWith(".jpg") && !layout.FileName.EndsWith(".pdf"))
                {
                    model.Success = false;
                    model.Message = "Error occurred...image file not in correct format.";
                }
                else
                {
                    string trimmedFileName = string.Empty;

                    byte[] tempFile = new byte[layout.ContentLength];
                    layout.InputStream.Read(tempFile, 0, layout.ContentLength);

                    if (layout.FileName.EndsWith("png"))
                    {
                        trimmedFileName = layout.FileName.Replace(".png", "");
                    }
                    else if (layout.FileName.EndsWith("jpg"))
                    {
                        trimmedFileName = layout.FileName.Replace(".jpg", "");
                    }
                    else if (layout.FileName.EndsWith("pdf"))
                    {
                        trimmedFileName = layout.FileName.Replace(".pdf", "");
                    }

                    model.Success = true;
                    model.Description = trimmedFileName;
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get layout by id
        /// </summary>
        /// <param name="layoutId"></param>
        /// <param name="isProject"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public FileStreamResult GetLayout(Guid layoutId, bool isProject)
        {
            var layout = new LayoutPdf();

            if (isProject)
            {
                var dbLayout = _projectPartRepository.GetProjectPartLayout(layoutId);

                layout = new ProjectPartLayoutConverter().ConvertToPdf(dbLayout);
            }
            else
            {
                var dbLayout = _partRepository.GetPartLayout(layoutId);

                layout = new PartLayoutConverter().ConvertToPdf(dbLayout);
            }

            MemoryStream ms = new MemoryStream(layout.Content, 0, 0, true, true);
            Response.ContentType = layout.Type;
            Response.AddHeader("content-disposition", "inline;filename=" + layout.Description);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            string mineType = "application/pdf";

            return new FileStreamResult(Response.OutputStream, mineType);
        }

        /// <summary>
        /// get layouts of part
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPartLayouts(Guid partId)
        {
            var model = new PartViewModel();

            var projectPart = _projectPartRepository.GetProjectPart(partId);

            if (projectPart != null)
            {
                var layouts = _projectPartRepository.GetProjectPartLayouts(partId);

                model.Layouts = new List<LayoutViewModel>();

                foreach (var layout in layouts)
                {
                    LayoutViewModel layoutModel = new ProjectPartLayoutConverter().ConvertToView(layout);
                    model.Layouts.Add(layoutModel);
                }
            }
            else
            {
                var layouts = _partRepository.GetPartLayouts(partId);

                model.Layouts = new List<LayoutViewModel>();

                foreach (var layout in layouts)
                {
                    var layoutModel = new PartLayoutConverter().ConvertToView(layout);
                    model.Layouts.Add(layoutModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get parts by customer and foundry
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="foundryId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSelectablePartsByCustomerAndFoundry(string customerId, string foundryId)
        {
            var selectableParts = _partRepository.GetSelectablePartsByCustomerAndFoundry(customerId, foundryId);

            var defaultPart = new SelectListItem()
            {
                Text = "--Select Part--",
                Value = null
            };

            selectableParts.Insert(0, defaultPart);

            return Json(selectableParts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get tooling information by part
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetToolingInfo(Guid partId)
        {
            var model = new PartViewModel();

            var projectPart = _projectPartRepository.GetProjectPart(partId);

            if (projectPart != null)
            {
                var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderParts().FirstOrDefault(x => x.ProjectPartId == partId);

                if (foundryOrderPart != null)
                {
                    var toolingOrder = _foundryOrderRepository.GetFoundryOrders().FirstOrDefault(x => x.FoundryOrderId == foundryOrderPart.FoundryOrderId &&
                                                                                                      x.IsTooling);

                    model.PartNumber = (projectPart != null && !string.IsNullOrEmpty(projectPart.Number)) ? projectPart.Number : "N/A";
                    model.ToolingDescription = (projectPart != null && !string.IsNullOrEmpty(projectPart.ToolingDescription)) ? projectPart.ToolingDescription : "N/A";
                    model.ToolingOrderNumber = (toolingOrder != null && !string.IsNullOrEmpty(toolingOrder.Number)) ? toolingOrder.Number : "N/A";
                    model.Notes = (projectPart != null && !string.IsNullOrEmpty(projectPart.Notes)) ? projectPart.Notes : "N/A";
                }
            }
            else
            {
                var part = _partRepository.GetPart(partId);

                if (part != null)
                {
                    var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderParts().FirstOrDefault(x => x.PartId == partId);

                    if (foundryOrderPart != null)
                    {
                        var toolingOrder = _foundryOrderRepository.GetFoundryOrders().FirstOrDefault(x => x.FoundryOrderId == foundryOrderPart.FoundryOrderId &&
                                                                                                          x.IsTooling);

                        model.PartNumber = (part != null && !string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
                        model.ToolingDescription = (part != null && !string.IsNullOrEmpty(part.ToolingDescription)) ? part.ToolingDescription : "N/A";
                        model.ToolingOrderNumber = (toolingOrder != null && !string.IsNullOrEmpty(toolingOrder.Number)) ? toolingOrder.Number : "N/A";
                        model.Notes = (part != null && !string.IsNullOrEmpty(part.Notes)) ? part.Notes : "N/A";
                    }
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
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
                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_shipmentTermRepository != null)
                {
                    _shipmentTermRepository.Dispose();
                    _shipmentTermRepository = null;
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

                if (_specificationMaterialRepository != null)
                {
                    _specificationMaterialRepository.Dispose();
                    _specificationMaterialRepository = null;
                }

                if (_projectPartRepository != null)
                {
                    _projectPartRepository.Dispose();
                    _projectPartRepository = null;
                }

                if (_priceSheetRepository != null)
                {
                    _priceSheetRepository.Dispose();
                    _priceSheetRepository = null;
                }

                if (_partRepository != null)
                {
                    _partRepository.Dispose();
                    _partRepository = null;
                }

                if (_partDynamicsRepository != null)
                {
                    _partDynamicsRepository.Dispose();
                    _partDynamicsRepository = null;
                }

                if (_htsNumberRepository != null)
                {
                    _htsNumberRepository.Dispose();
                    _htsNumberRepository = null;
                }

                if (_partStatusRepository != null)
                {
                    _partStatusRepository.Dispose();
                    _partStatusRepository = null;
                }

                if (_partTypeRepository != null)
                {
                    _partTypeRepository.Dispose();
                    _partTypeRepository = null;
                }

                if (_paymentTermRepository != null)
                {
                    _paymentTermRepository.Dispose();
                    _paymentTermRepository = null;
                }

                if (_surchargeRepository != null)
                {
                    _surchargeRepository.Dispose();
                    _surchargeRepository = null;
                }

                if (_siteDynamicsRepository != null)
                {
                    _siteDynamicsRepository.Dispose();
                    _siteDynamicsRepository = null;
                }

                if (_patternMaterialRepository != null)
                {
                    _patternMaterialRepository.Dispose();
                    _patternMaterialRepository = null;
                }

                if (_destinationRepository != null)
                {
                    _destinationRepository.Dispose();
                    _destinationRepository = null;
                }

                if (_foundryOrderRepository != null)
                {
                    _foundryOrderRepository.Dispose();
                    _foundryOrderRepository = null;
                }

                if (_customerOrderRepository != null)
                {
                    _customerOrderRepository.Dispose();
                    _customerOrderRepository = null;
                }

                if (_accountCodeRepository != null)
                {
                    _accountCodeRepository.Dispose();
                    _accountCodeRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}