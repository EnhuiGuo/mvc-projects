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
    public class RfqController : ApplicationBaseController
    {
        private IRfqRepository _rfqRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private IShipmentTermRepository _shipmentTermRepository;
        private IMaterialRepository _materialRepository;
        private ICoatingTypeRepository _coatingTypeRepository;
        private ISpecificationMaterialRepository _specificationMaterialRepository;
        private IProjectPartRepository _projectPartRepository;
        private ICountryRepository _countryRepository;
        private IProjectRepository _projectRepository;
        private IPriceSheetRepository _priceSheetRepository;
        private IPartRepository _partRepository;

        public RfqController()
        {
            _rfqRepository = new RfqRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _shipmentTermRepository = new ShipmentTermRepository();
            _materialRepository = new MaterialRepository();
            _coatingTypeRepository = new CoatingTypeRepository();
            _specificationMaterialRepository = new SpecificationMaterialRepository();
            _projectPartRepository = new ProjectPartRepository();
            _countryRepository = new CountryRepository();
            _projectRepository = new ProjectRepository();
            _priceSheetRepository = new PriceSheetRepository();
            _partRepository = new PartRepository();
        }

        public RfqController(IRfqRepository rfqRepository,
                                     ICustomerDynamicsRepository customerDynamicsRepository,
                                     IFoundryDynamicsRepository foundryDynamicsRepository,
                                     IShipmentTermRepository shipmentTermRepository,
                                     IMaterialRepository materialRepository,
                                     ICoatingTypeRepository coatingTypeRepository,
                                     ISpecificationMaterialRepository specificationMaterialRepository,
                                     IProjectPartRepository projectPartRepository,
                                     ICountryRepository countryRepository,
                                     IProjectRepository projectRepository,
                                     IPriceSheetRepository priceSheetRepository,
                                     IPartRepository partRepository)
        {
            _rfqRepository = rfqRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _shipmentTermRepository = shipmentTermRepository;
            _materialRepository = materialRepository;
            _coatingTypeRepository = coatingTypeRepository;
            _specificationMaterialRepository = specificationMaterialRepository;
            _projectPartRepository = projectPartRepository;
            _countryRepository = countryRepository;
            _projectRepository = projectRepository;
            _priceSheetRepository = priceSheetRepository;
            _partRepository = partRepository;
        }

        private string RfqNumber()
        {
            var rfqNumber = _rfqRepository.RfqNumber();

            return rfqNumber;
        }

        /// <summary>
        /// GET: Operations/Rfq
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            var model = new RfqViewModel();

            var rfqs = _rfqRepository.GetRfqs().Where(x=>x.IsOpen).ToList();

            if (rfqs != null && rfqs.Count > 0)
            {
                model.Rfqs = new List<RfqViewModel>();
                foreach (var rfq in rfqs)
                {
                    RfqViewModel convertedModel = new RfqConverter().ConvertToListView(rfq);
                    model.Rfqs.Add(convertedModel);
                }
            }

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Rfq/Create
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Create()
        {
            var model = new RfqViewModel();
            {
                model.RfqNumber = RfqNumber();
            }

            _rfqRepository.RemoveRfqNumber(model.RfqNumber);

            model.Date = DateTime.Now.ToShortDateString();
            model.RfqDateStr = DateTime.Now.ToShortDateString();
            model.PrintsSent = DateTime.Now.ToShortDateString();
            model.Machining = "Not Included";

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Ship To--",
                Value = null
            };

            model.SelectableShipmentTerms.Insert(0, defaultShipmentTerm);

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

            model.SelectableCoatingTypes = _coatingTypeRepository.GetSelectableCoatingTypes();

            var defaultCoatingType = new SelectListItem()
            {
                Text = "--Select Coating Type--",
                Value = null
            };

            model.SelectableCoatingTypes.Insert(0, defaultCoatingType);

            model.SelectableSpecificationMaterial = _specificationMaterialRepository.GetSelectableSpecificationMaterials();

            var defaultSpecificationMaterial = new SelectListItem()
            {
                Text = "--Select Specification--",
                Value = null
            };

            model.SelectableSpecificationMaterial.Insert(0, defaultSpecificationMaterial);

            model.SelectableMaterial = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            model.SelectableMaterial.Insert(0, defaultMaterial);

            return View(model);
        }

        /// <summary>
        /// create rfq
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Create(RfqViewModel model)
        {
            var operationResult = new OperationResult();

            var existingCoatingType = _coatingTypeRepository.GetCoatingType(model.CoatingType);
            if (existingCoatingType == null)
            {
                var newCoatingType = new CoatingType();
                {
                    newCoatingType.Description = model.CoatingType;
                    newCoatingType.IsActive = true;
                }
                operationResult = _coatingTypeRepository.SaveCoatingType(newCoatingType);
                existingCoatingType = _coatingTypeRepository.GetCoatingType(model.CoatingType);
            }

            var existingSpecificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial(model.SpecificationMaterialDescription);
            if (existingSpecificationMaterial == null)
            {
                var newSpecificationMaterial = new SpecificationMaterial();
                {
                    newSpecificationMaterial.Description = model.SpecificationMaterialDescription;
                    newSpecificationMaterial.IsActive = true;
                }
                operationResult = _specificationMaterialRepository.SaveSpecificationMaterial(newSpecificationMaterial);
                existingSpecificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial(model.SpecificationMaterialDescription);
            }

            var project = _projectRepository.GetProject(model.ProjectName);

            if (project == null)
            {
                var newProject = new ProjectConverter().ConvertToCreate(model);

                operationResult = _projectRepository.SaveProject(newProject);

                model.ProjectId = operationResult.ReferenceId;
            }
            else
            {
                model.ProjectId = project.ProjectId;
            }

            var newRfq = new RfqConverter().ConvertToDomain(model);

            newRfq.ProjectId = model.ProjectId;

            operationResult = _rfqRepository.SaveRfq(newRfq);

            newRfq.RfqId = operationResult.ReferenceId;

            if (operationResult.Success && model != null)
            {
                foreach (var rfqPart in model.RfqParts)
                {
                    var newProjectPart = new ProjectPartConverter().ConvertToDomain(rfqPart);

                    newProjectPart.RfqId = operationResult.ReferenceId;
                    newProjectPart.ProjectId = model.ProjectId;
                    newProjectPart.MaterialId = rfqPart.MaterialId;
                    newProjectPart.MaterialSpecificationId = existingSpecificationMaterial.SpecificationMaterialId;
                    newProjectPart.CoatingTypeId = existingCoatingType.CoatingTypeId;

                    operationResult = _projectPartRepository.SaveProjectPart(newProjectPart);
                }
            }

            operationResult.ReferenceId = newRfq.RfqId;

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Rfq/Detail
        /// </summary>
        /// <param name="rfqId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Detail(Guid rfqId)
        {
            var rfq = _rfqRepository.GetRfq(rfqId);

            RfqViewModel model = new RfqConverter().ConvertToView(rfq);

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Rfq/Edit
        /// </summary>
        /// <param name="RfqNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Edit(Guid rfqId)
        {
            RfqViewModel model = new RfqViewModel();

            var currentRfq = _rfqRepository.GetRfq(rfqId);

            model = new RfqConverter().ConvertToView(currentRfq);

            model.Parts = _partRepository.GetSelectablePartsByCustomer(currentRfq.CustomerId);

            model.SelectableShipmentTerms = _shipmentTermRepository.GetSelectableShipmentTerms();

            var defaultShipmentTerm = new SelectListItem()
            {
                Text = "--Select Shipment Term--",
                Value = null
            };

            model.SelectableShipmentTerms.Insert(0, defaultShipmentTerm);

            model.SelectableCoatingTypes = _coatingTypeRepository.GetSelectableCoatingTypes();

            model.SelectableSpecificationMaterial = _specificationMaterialRepository.GetSelectableSpecificationMaterials();

            model.SelectableMaterial = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            model.SelectableMaterial.Insert(0, defaultMaterial);

            model.MaterialId = (model.RfqParts != null) ? model.RfqParts[0].MaterialId : null;

            model.CurrentUser = User.Identity.Name;

            return View(model);
        }

        /// <summary>
        /// edit Rfq
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Edit(RfqViewModel model)
        {
            var operationResult = new OperationResult();
            var existingCoatingType = _coatingTypeRepository.GetCoatingType(model.CoatingType);
            if (existingCoatingType == null)
            {
                var newCoatingType = new CoatingType();
                {
                    newCoatingType.Description = model.CoatingType;
                    newCoatingType.IsActive = true;
                }
                operationResult = _coatingTypeRepository.SaveCoatingType(newCoatingType);
                existingCoatingType = _coatingTypeRepository.GetCoatingType(model.CoatingType);
            }

            var existingSpecificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial(model.SpecificationMaterialDescription.ToLower().Replace(" ", string.Empty));
            if (existingSpecificationMaterial == null)
            {
                var newSpecificationMaterial = new SpecificationMaterial();
                {
                    newSpecificationMaterial.Description = model.SpecificationMaterialDescription;
                    newSpecificationMaterial.IsActive = true;
                }
                operationResult = _specificationMaterialRepository.SaveSpecificationMaterial(newSpecificationMaterial);
                existingSpecificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial(model.SpecificationMaterialDescription);
            }

            var rfqToUpdate = _rfqRepository.GetRfq(model.RfqId);
            rfqToUpdate = new RfqConverter().ConvertToDomain(model);

            var projectToUpdate = _projectRepository.GetProject(model.ProjectId);
            if (model.Status != null)
            {
                if (model.IsOpen)
                {
                    projectToUpdate.IsOpen = true;
                }
                else
                {
                    projectToUpdate.IsOpen = false;
                }

                if (model.IsHold)
                {
                    projectToUpdate.IsHold = true;
                    projectToUpdate.HoldExpirationDate = model.HoldExpirationDate;
                    projectToUpdate.HoldNotes = model.HoldNotes;
                }
                else
                {
                    projectToUpdate.IsHold = false;
                    projectToUpdate.HoldExpirationDate = null;
                    projectToUpdate.HoldNotes = null;
                }

                if (model.IsCanceled)
                {
                    projectToUpdate.IsCanceled = true;
                    projectToUpdate.CancelNotes = model.CancelNotes;
                }
                else
                {
                    projectToUpdate.IsCanceled = false;
                    projectToUpdate.CancelNotes = null;
                }
            }

            operationResult = _rfqRepository.UpdateRfq(rfqToUpdate);

            if (operationResult.Success)
            {
                model.Success = true;
                model.IsHold = rfqToUpdate.IsHold;
                model.IsCanceled = rfqToUpdate.IsCanceled;

                operationResult = _projectRepository.UpdateProject(projectToUpdate);

                if (model.RfqParts != null && model.RfqParts.Count() > 0)
                {
                    foreach (var rfqPart in model.RfqParts)
                    {
                        if (rfqPart.ProjectPartId != Guid.Empty)
                        {
                            var partToUpdate = new ProjectPartConverter().ConvertToDomain(rfqPart);
                            partToUpdate.RfqId = model.RfqId;
                            partToUpdate.ProjectId = model.ProjectId;
                            partToUpdate.MaterialSpecificationId = existingSpecificationMaterial.SpecificationMaterialId;
                            partToUpdate.MaterialId = rfqPart.MaterialId;
                            partToUpdate.CoatingTypeId = existingCoatingType.CoatingTypeId;
                            operationResult = _projectPartRepository.UpdateProjectPart(partToUpdate);
                        }
                        else
                        {
                            var newProjectPart = new ProjectPartConverter().ConvertToDomain(rfqPart);

                            newProjectPart.RfqId = model.RfqId;
                            newProjectPart.ProjectId = model.ProjectId;
                            newProjectPart.MaterialId = rfqPart.MaterialId;
                            newProjectPart.MaterialSpecificationId = existingSpecificationMaterial.SpecificationMaterialId;
                            newProjectPart.CoatingTypeId = existingCoatingType.CoatingTypeId;

                            operationResult = _projectPartRepository.SaveProjectPart(newProjectPart);
                        }
                    }
                }

                var existingProjectParts = _projectPartRepository.GetProjectParts().Where(x => x.ProjectId == model.ProjectId).ToList();

                if (existingProjectParts != null && existingProjectParts.Count > 0)
                {
                    foreach (var existingProjectPart in existingProjectParts)
                    {
                        var projectPart = model.RfqParts.FirstOrDefault(x => x.PartNumber == existingProjectPart.Number);

                        if (projectPart == null)
                        {
                            operationResult = _projectPartRepository.DeleteProjectPart(existingProjectPart.ProjectPartId);
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
        /// delete Rfq
        /// </summary>
        /// <param name="RfqNumber"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult Delete(Guid rfqId)
        {
            var operationResult = new OperationResult();

            var existingRfq = _rfqRepository.GetRfq(rfqId);

            if(existingRfq != null)
            {
                var rfqCount = _rfqRepository.GetRfqs().Where(x => x.ProjectId == existingRfq.ProjectId).Count();

                if (rfqCount == 1)
                {
                    operationResult = _projectRepository.DeleteProject(existingRfq.ProjectId);
                }

                var projectParts = _projectPartRepository.GetProjectParts().Where(x => x.RfqId == rfqId).ToList();

                if (projectParts != null && projectParts.Count() > 0)
                {
                    foreach (var projectPart in projectParts)
                    {
                        var partDrawings = _projectPartRepository.GetProjectPartDrawings(projectPart.ProjectPartId);

                        if (partDrawings != null && partDrawings.Count() > 0)
                        {
                            foreach (var partDrawing in partDrawings)
                            {
                                operationResult = _projectPartRepository.DeleteProjectPartDrawing(partDrawing.ProjectPartDrawingId);
                            }
                        }

                        var partLayouts = _projectPartRepository.GetProjectPartLayouts(projectPart.ProjectPartId);

                        if (partLayouts != null && partLayouts.Count() > 0)
                        {
                            foreach (var partLayout in partLayouts)
                            {
                                operationResult = _projectPartRepository.DeleteProjectPartLayout(partLayout.ProjectPartLayoutId);
                            }
                        }

                        operationResult = _projectPartRepository.DeleteProjectPart(projectPart.ProjectPartId);
                    }
                }

                operationResult = _rfqRepository.DeleteRfq(rfqId);
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add rfq part modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddRfqPart()
        {
            var model = new RfqViewModel();

            model.SelectableMaterial = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            model.SelectableMaterial.Insert(0, defaultMaterial);

            return PartialView(model);
        }

        /// <summary>
        /// edit rfq part modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditRfqPart()
        {
            var model = new RfqViewModel();

            model.SelectableMaterial = _materialRepository.GetSelectableMaterials();

            var defaultMaterial = new SelectListItem()
            {
                Text = "--Select Material--",
                Value = null
            };

            model.SelectableMaterial.Insert(0, defaultMaterial);

            return PartialView(model);
        }

        /// <summary>
        /// get all rfqs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAllRfqs()
        {
            var model = new RfqViewModel();

            var tempRfqs = _rfqRepository.GetRfqs().ToList();

            if (tempRfqs != null && tempRfqs.Count > 0)
            {
                model.Rfqs = new List<RfqViewModel>();
                foreach (var tempRfq in tempRfqs)
                {
                    RfqViewModel convertedModel = new RfqConverter().ConvertToListView(tempRfq);
                    model.Rfqs.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get open rfqs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOpenRfqs()
        {
            var model = new RfqViewModel();

            var tempRfqs = _rfqRepository.GetRfqs().Where(x => x.IsOpen).ToList();

            if (tempRfqs != null && tempRfqs.Count > 0)
            {
                model.Rfqs = new List<RfqViewModel>();
                foreach (var tempRfq in tempRfqs)
                {
                    RfqViewModel convertedModel = new RfqConverter().ConvertToListView(tempRfq);
                    model.Rfqs.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get hold rfqs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetHoldRfqs()
        {
            var model = new RfqViewModel();

            var tempRfqs = _rfqRepository.GetRfqs().Where(x => x.IsHold).ToList();

            if (tempRfqs != null && tempRfqs.Count > 0)
            {
                model.Rfqs = new List<RfqViewModel>();
                foreach (var tempRfq in tempRfqs)
                {
                    RfqViewModel convertedModel = new RfqConverter().ConvertToListView(tempRfq);
                    model.Rfqs.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get canceled rfqs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCanceledRfqs()
        {
            var model = new RfqViewModel();

            var tempRfqs = _rfqRepository.GetRfqs().Where(x => x.IsCanceled).ToList();

            if (tempRfqs != null && tempRfqs.Count > 0)
            {
                model.Rfqs = new List<RfqViewModel>();
                foreach (var tempRfq in tempRfqs)
                {
                    RfqViewModel convertedModel = new RfqConverter().ConvertToListView(tempRfq);
                    model.Rfqs.Add(convertedModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get rfq by project name
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetRfqByProject(string projectName)
        {
            var project = _projectRepository.GetProject(projectName);

            var rfq = _rfqRepository.GetRfqByProject((project != null) ? project.ProjectId : Guid.Empty);

            RfqViewModel model = new RfqConverter().ConvertToView(rfq);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get rfq part by part
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetRfqPartByPart(Guid partId)
        {
            var part = _partRepository.GetPart(partId);

            RfqPartViewModel model = new RfqPartConverter().ConvertToView(part);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_rfqRepository != null)
                {
                    _rfqRepository.Dispose();
                    _rfqRepository = null;
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

                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }

                if (_projectRepository != null)
                {
                    _projectRepository.Dispose();
                    _projectRepository = null;
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
            }

            base.Dispose(disposing);
        }
    }
}