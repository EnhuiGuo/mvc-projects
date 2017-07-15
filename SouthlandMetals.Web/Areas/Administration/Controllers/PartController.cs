using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Administration.Controllers
{
    public class PartController : ApplicationBaseController
    {
        private IHtsNumberRepository _htsNumberRepository;
        private IMaterialRepository _materialRepository;
        private ISpecificationMaterialRepository _specificationMaterialRepository;
        private IPartStatusRepository _partStatusRepository;
        private IPartTypeRepository _partTypeRepository;
        private IPaymentTermRepository _paymentTermRepository;
        private IShipmentTermRepository _shipmentTermRepository;
        private ISurchargeRepository _surchargeRepository;
        private ITrackingCodeRepository _trackingCodeRepository;
        private IPatternMaterialRepository _patternMaterialRepository;
        private ICoatingTypeRepository _coatingTypeRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private IPartRepository _partRepository;

        public PartController()
        {
            _htsNumberRepository = new HtsNumberRepository();
            _materialRepository = new MaterialRepository();
            _specificationMaterialRepository = new SpecificationMaterialRepository();
            _partStatusRepository = new PartStatusRepository();
            _partTypeRepository = new PartTypeRepository();
            _paymentTermRepository = new PaymentTermRepository();
            _shipmentTermRepository = new ShipmentTermRepository();
            _surchargeRepository = new SurchargeRepository();
            _trackingCodeRepository = new TrackingCodeRepository();
            _patternMaterialRepository = new PatternMaterialRepository();
            _coatingTypeRepository = new CoatingTypeRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _partRepository = new PartRepository();
        }

        public PartController(IHtsNumberRepository htsNumberRepository,
                              IMaterialRepository materialRepository,
                              ISpecificationMaterialRepository specificationMaterialRepository,
                              IPartStatusRepository partStatusRepository,
                              IPartTypeRepository partTypeRepository,
                              IPaymentTermRepository paymentTermRepository,
                              IShipmentTermRepository shipmentTermRepository,
                              ISurchargeRepository surchargeRepository,
                              ITrackingCodeRepository trackingCodeRepository,
                              IPatternMaterialRepository patternMaterialRepository,
                              ICoatingTypeRepository coatingTypeRepository,
                              ICustomerDynamicsRepository customerDynamicsRepository,
                              IPartRepository partRepository)
        {
            _htsNumberRepository = htsNumberRepository;
            _materialRepository = materialRepository;
            _specificationMaterialRepository = specificationMaterialRepository;
            _partStatusRepository = partStatusRepository;
            _partTypeRepository = partTypeRepository;
            _paymentTermRepository = paymentTermRepository;
            _shipmentTermRepository = shipmentTermRepository;
            _surchargeRepository = surchargeRepository;
            _trackingCodeRepository = trackingCodeRepository;
            _patternMaterialRepository = patternMaterialRepository;
            _coatingTypeRepository = coatingTypeRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _partRepository = partRepository;
        }

        /// <summary>
        /// GET: Administration/Part
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: Administration/HtsNumbers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult HtsNumbers()
        {
            var model = new HtsNumberViewModel();

            var htsNumbers = new List<HtsNumberViewModel>();

            var tempHtsNumbers = _htsNumberRepository.GetHtsNumbers().Where(x => x.IsActive).ToList();

            if (tempHtsNumbers != null && tempHtsNumbers.Count > 0)
            {
                foreach (var tempHtsNumber in tempHtsNumbers)
                {
                    HtsNumberViewModel convertedModel = new HtsNumberConverter().ConvertToView(tempHtsNumber);

                    htsNumbers.Add(convertedModel);
                }
            }

            model.HtsNumbers = htsNumbers.OrderBy(x => x.HtsNumberDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add htsNumber modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddHtsNumber()
        {
            return PartialView();
        }

        /// <summary>
        /// add htsNumber 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddHtsNumber(HtsNumberViewModel model)
        {
            var operationResult = new OperationResult();

            HtsNumber newHtsNumber = new HtsNumberConverter().ConvertToDomain(model);

            operationResult = _htsNumberRepository.SaveHtsNumber(newHtsNumber);

            if (operationResult.Success)
            {
                model.Success = true;

                var htsNumbers = new List<HtsNumberViewModel>();

                var tempHtsNumbers = _htsNumberRepository.GetHtsNumbers().Where(x => x.IsActive).ToList();

                if (tempHtsNumbers != null && tempHtsNumbers.Count > 0)
                {
                    foreach (var tempHtsNumber in tempHtsNumbers)
                    {
                        HtsNumberViewModel convertedModel = new HtsNumberConverter().ConvertToView(tempHtsNumber);

                        htsNumbers.Add(convertedModel);
                    }
                }

                model.HtsNumbers = htsNumbers.OrderBy(x => x.HtsNumberDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit htsNumber modal
        /// </summary>
        /// <param name="htsNumberId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditHtsNumber(Guid htsNumberId)
        {
            var htsNumber = _htsNumberRepository.GetHtsNumber(htsNumberId);

            HtsNumberViewModel model = new HtsNumberConverter().ConvertToView(htsNumber);

            return PartialView(model);
        }

        /// <summary>
        /// edit htsNumber modal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditHtsNumber(HtsNumberViewModel model)
        {
            var operationResult = new OperationResult();

            HtsNumber htsNumber = new HtsNumberConverter().ConvertToDomain(model);

            operationResult = _htsNumberRepository.UpdateHtsNumber(htsNumber);

            if (operationResult.Success)
            {
                model.Success = true;

                var htsNumbers = new List<HtsNumberViewModel>();

                var tempHtsNumbers = _htsNumberRepository.GetHtsNumbers().Where(x => x.IsActive).ToList();

                if (tempHtsNumbers != null && tempHtsNumbers.Count > 0)
                {
                    foreach (var tempHtsNumber in tempHtsNumbers)
                    {
                        HtsNumberViewModel convertedModel = new HtsNumberConverter().ConvertToView(tempHtsNumber);

                        htsNumbers.Add(convertedModel);
                    }
                }

                model.HtsNumbers = htsNumbers.OrderBy(x => x.HtsNumberDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }
       
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/Material
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Material()
        {
            var model = new MaterialViewModel();

            var materials = new List<MaterialViewModel>();

            var tempMaterials = _materialRepository.GetMaterials().Where(x => x.IsActive).ToList();

            if (tempMaterials != null && tempMaterials.Count > 0)
            {
                foreach (var tempMaterial in tempMaterials)
                {
                    MaterialViewModel convertedModel = new MaterialConverter().ConvertToView(tempMaterial);

                    materials.Add(convertedModel);
                }
            }

            model.Materials = materials.OrderBy(x => x.MaterialDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add material modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddMaterial()
        {
            return PartialView();
        }

        /// <summary>
        /// add material 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddMaterial(MaterialViewModel model)
        {
            var operationResult = new OperationResult();

            Material newMaterial = new MaterialConverter().ConvertToDomain(model);

            operationResult = _materialRepository.SaveMaterial(newMaterial);

            if (operationResult.Success)
            {
                model.Success = true;

                var materials = new List<MaterialViewModel>();

                var tempMaterials = _materialRepository.GetMaterials().Where(x => x.IsActive).ToList();

                if (tempMaterials != null && tempMaterials.Count > 0)
                {
                    foreach (var tempMaterial in tempMaterials)
                    {
                        MaterialViewModel convertedModel = new MaterialConverter().ConvertToView(tempMaterial);

                        materials.Add(convertedModel);
                    }
                }

                model.Materials = materials.OrderBy(x => x.MaterialDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit material modal
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditMaterial(Guid materialId)
        {
            var material = _materialRepository.GetMaterial(materialId);

            MaterialViewModel convertedModel = new MaterialConverter().ConvertToView(material);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit material
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditMaterial(MaterialViewModel model)
        {
            var operationResult = new OperationResult();

            Material material = new MaterialConverter().ConvertToDomain(model);

            operationResult = _materialRepository.UpdateMaterial(material);

            if (operationResult.Success)
            {
                model.Success = true;

                var materials = new List<MaterialViewModel>();

                var tempMaterials = _materialRepository.GetMaterials().Where(x => x.IsActive).ToList();

                if (tempMaterials != null && tempMaterials.Count > 0)
                {
                    foreach (var tempMaterial in tempMaterials)
                    {
                        MaterialViewModel convertedModel = new MaterialConverter().ConvertToView(tempMaterial);

                        materials.Add(convertedModel);
                    }
                }

                model.Materials = materials.OrderBy(x => x.MaterialDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/SpecificationMaterial
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult SpecificationMaterial()
        {
            var model = new SpecificationMaterialViewModel();

            var specificationMaterials = new List<SpecificationMaterialViewModel>();

            var tempSpecificationMaterials = _specificationMaterialRepository.GetSpecificationMaterials().Where(x => x.IsActive).ToList();

            if (tempSpecificationMaterials != null && tempSpecificationMaterials.Count > 0)
            {
                foreach (var tempSpecificationMaterial in tempSpecificationMaterials)
                {
                    SpecificationMaterialViewModel convertedModel = new SpecificationMaterialConverter().ConvertToView(tempSpecificationMaterial);

                    specificationMaterials.Add(convertedModel);
                }
            }

            model.SpecificationMaterials = specificationMaterials.OrderBy(x => x.SpecificationDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add specification material modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddSpecificationMaterial()
        {
            return PartialView();
        }

        /// <summary>
        /// add specification material
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddSpecificationMaterial(SpecificationMaterialViewModel model)
        {
            var operationResult = new OperationResult();

            SpecificationMaterial newSpecificationMaterial = new SpecificationMaterialConverter().ConvertToDomain(model);

            operationResult = _specificationMaterialRepository.SaveSpecificationMaterial(newSpecificationMaterial);

            if (operationResult.Success)
            {
                model.Success = true;

                var specificationMaterials = new List<SpecificationMaterialViewModel>();

                var tempSpecificationMaterials = _specificationMaterialRepository.GetSpecificationMaterials().Where(x => x.IsActive).ToList();

                if (tempSpecificationMaterials != null && tempSpecificationMaterials.Count > 0)
                {
                    foreach (var tempSpecificationMaterial in tempSpecificationMaterials)
                    {
                        SpecificationMaterialViewModel convertedModel = new SpecificationMaterialConverter().ConvertToView(tempSpecificationMaterial);

                        specificationMaterials.Add(convertedModel);
                    }
                }

                model.SpecificationMaterials = specificationMaterials.OrderBy(x => x.SpecificationDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit specififcation material modal
        /// </summary>
        /// <param name="specificationMaterialId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditSpecificationMaterial(Guid specificationMaterialId)
        {
            var specificationMaterial = _specificationMaterialRepository.GetSpecificationMaterial(specificationMaterialId);

            SpecificationMaterialViewModel convertedModel = new SpecificationMaterialConverter().ConvertToView(specificationMaterial);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit specification material
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditSpecificationMaterial(SpecificationMaterialViewModel model)
        {
            var operationResult = new OperationResult();

            SpecificationMaterial specificationMaterial = new SpecificationMaterialConverter().ConvertToDomain(model);

            operationResult = _specificationMaterialRepository.UpdateSpecificationMaterial(specificationMaterial);

            if (operationResult.Success)
            {
                model.Success = true;

                var specificationMaterials = new List<SpecificationMaterialViewModel>();

                var tempSpecificationMaterials = _specificationMaterialRepository.GetSpecificationMaterials().Where(x => x.IsActive).ToList();

                if (tempSpecificationMaterials != null && tempSpecificationMaterials.Count > 0)
                {
                    foreach (var tempSpecificationMaterial in tempSpecificationMaterials)
                    {
                        SpecificationMaterialViewModel convertedModel = new SpecificationMaterialConverter().ConvertToView(tempSpecificationMaterial);

                        specificationMaterials.Add(convertedModel);
                    }
                }

                model.SpecificationMaterials = specificationMaterials.OrderBy(x => x.SpecificationDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/PartStates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult PartStates()
        {
            var model = new PartStatusViewModel();

            var partStates = new List<PartStatusViewModel>();

            var tempPartStates = _partStatusRepository.GetPartStates().Where(x => x.IsActive).ToList();

            if (tempPartStates != null && tempPartStates.Count > 0)
            {
                foreach (var tempPartStatus in tempPartStates)
                {
                    PartStatusViewModel convertedModel = new PartStatusConverter().ConvertToView(tempPartStatus);

                    partStates.Add(convertedModel);
                }
            }

            model.PartStates = partStates.OrderBy(x => x.PartStatusDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add part status modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddPartStatus()
        {
            return PartialView();
        }

        /// <summary>
        /// add part status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddPartStatus(PartStatusViewModel model)
        {
            var operationResult = new OperationResult();

            PartStatus newPartStatus = new PartStatusConverter().ConvertToDomain(model);

            operationResult = _partStatusRepository.SavePartStatus(newPartStatus);

            if (operationResult.Success)
            {
                model.Success = true;

                var partStates = new List<PartStatusViewModel>();

                var tempPartStates = _partStatusRepository.GetPartStates().Where(x => x.IsActive).ToList();

                if (tempPartStates != null && tempPartStates.Count > 0)
                {
                    foreach (var tempPartStatus in tempPartStates)
                    {
                        PartStatusViewModel convertedModel = new PartStatusConverter().ConvertToView(tempPartStatus);

                        partStates.Add(convertedModel);
                    }
                }

                model.PartStates = partStates.OrderBy(x => x.PartStatusDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit part status modal
        /// </summary>
        /// <param name="partStatusId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditPartStatus(Guid partStatusId)
        {
            var partStatus = _partStatusRepository.GetPartStatus(partStatusId);

            PartStatusViewModel convertedModel = new PartStatusConverter().ConvertToView(partStatus);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit part status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditPartStatus(PartStatusViewModel model)
        {
            var operationResult = new OperationResult();

            PartStatus newPartStatus = new PartStatusConverter().ConvertToDomain(model);

            operationResult = _partStatusRepository.UpdatePartStatus(newPartStatus);

            if (operationResult.Success)
            {
                model.Success = true;

                var partStates = new List<PartStatusViewModel>();

                var tempPartStates = _partStatusRepository.GetPartStates().Where(x => x.IsActive).ToList();

                if (tempPartStates != null && tempPartStates.Count > 0)
                {
                    foreach (var tempPartStatus in tempPartStates)
                    {
                        PartStatusViewModel convertedModel = new PartStatusConverter().ConvertToView(tempPartStatus);

                        partStates.Add(convertedModel);
                    }
                }

                model.PartStates = partStates.OrderBy(x => x.PartStatusDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/PartTypes 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult PartTypes()
        {
            var model = new PartTypeViewModel();

            var partTypes = new List<PartTypeViewModel>();

            var tempPartTypes = _partTypeRepository.GetPartTypes().Where(x => x.IsActive).ToList();

            if (tempPartTypes != null && tempPartTypes.Count > 0)
            {
                foreach (var tempPartType in tempPartTypes)
                {
                    PartTypeViewModel convertedModel = new PartTypeConverter().ConvertToView(tempPartType);

                    partTypes.Add(convertedModel);
                }
            }

            model.PartTypes = partTypes.OrderBy(x => x.PartTypeDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add part type modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddPartType()
        {
            return PartialView();
        }

        /// <summary>
        /// add part type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddPartType(PartTypeViewModel model)
        {
            var operationResult = new OperationResult();

            PartType newPartType = new PartTypeConverter().ConvertToDomain(model);

            operationResult = _partTypeRepository.SavePartType(newPartType);

            if (operationResult.Success)
            {
                model.Success = true;

                var partTypes = new List<PartTypeViewModel>();

                var tempPartTypes = _partTypeRepository.GetPartTypes().Where(x => x.IsActive).ToList();

                if (tempPartTypes != null && tempPartTypes.Count > 0)
                {
                    foreach (var tempPartType in tempPartTypes)
                    {
                        PartTypeViewModel convertedModel = new PartTypeConverter().ConvertToView(tempPartType);

                        partTypes.Add(convertedModel);
                    }
                }

                model.PartTypes = partTypes.OrderBy(x => x.PartTypeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit part type modal
        /// </summary>
        /// <param name="partTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditPartType(Guid partTypeId)
        {
            var partType = _partTypeRepository.GetPartType(partTypeId);

            PartTypeViewModel convertedModel = new PartTypeConverter().ConvertToView(partType);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit part type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditPartType(PartTypeViewModel model)
        {
            var operationResult = new OperationResult();

            PartType partType = new PartTypeConverter().ConvertToDomain(model);

            operationResult = _partTypeRepository.UpdatePartType(partType);

            if (operationResult.Success)
            {
                model.Success = true;

                var partTypes = new List<PartTypeViewModel>();

                var tempPartTypes = _partTypeRepository.GetPartTypes().Where(x => x.IsActive).ToList();

                if (tempPartTypes != null && tempPartTypes.Count > 0)
                {
                    foreach (var tempPartType in tempPartTypes)
                    {
                        PartTypeViewModel convertedModel = new PartTypeConverter().ConvertToView(tempPartType);

                        partTypes.Add(convertedModel);
                    }
                }

                model.PartTypes = partTypes.OrderBy(x => x.PartTypeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/PaymentTerms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult PaymentTerms()
        {
            var model = new PaymentTermViewModel();

            var paymentTerms = new List<PaymentTermViewModel>();

            var tempPaymentTerms = _paymentTermRepository.GetPaymentTerms().Where(x => x.IsActive).ToList();

            if (tempPaymentTerms != null && tempPaymentTerms.Count > 0)
            {
                foreach (var tempPaymentTerm in tempPaymentTerms)
                {
                    PaymentTermViewModel convertedModel = new PaymentTermConverter().ConvertToView(tempPaymentTerm);

                    paymentTerms.Add(convertedModel);
                }
            }

            model.PaymentTerms = paymentTerms.OrderBy(x => x.PaymentTermDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add payment terms modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddPaymentTerm()
        {
            return PartialView();
        }

        /// <summary>
        /// add payment term
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddPaymentTerm(PaymentTermViewModel model)
        {
            var operationResult = new OperationResult();

            PaymentTerm newPaymentTerm = new PaymentTermConverter().ConvertToDomain(model);

            operationResult = _paymentTermRepository.SavePaymentTerm(newPaymentTerm);

            if (operationResult.Success)
            {
                model.Success = true;

                var paymentTerms = new List<PaymentTermViewModel>();

                var tempPaymentTerms = _paymentTermRepository.GetPaymentTerms().Where(x => x.IsActive).ToList();

                if (tempPaymentTerms != null && tempPaymentTerms.Count > 0)
                {
                    foreach (var tempPaymentTerm in tempPaymentTerms)
                    {
                        PaymentTermViewModel convertedModel = new PaymentTermConverter().ConvertToView(tempPaymentTerm);

                        paymentTerms.Add(convertedModel);
                    }
                }

                model.PaymentTerms = paymentTerms.OrderBy(x => x.PaymentTermDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit payment term
        /// </summary>
        /// <param name="paymentTermId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditPaymentTerm(Guid paymentTermId)
        {
            var paymentTerm = _paymentTermRepository.GetPaymentTerm(paymentTermId);

            PaymentTermViewModel convertedModel = new PaymentTermConverter().ConvertToView(paymentTerm);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit payment term
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditPaymentTerm(PaymentTermViewModel model)
        {
            var operationResult = new OperationResult();

            PaymentTerm newPaymentTerm = new PaymentTermConverter().ConvertToDomain(model);

            operationResult = _paymentTermRepository.UpdatePaymentTerm(newPaymentTerm);

            if (operationResult.Success)
            {
                model.Success = true;

                var paymentTerms = new List<PaymentTermViewModel>();

                var tempPaymentTerms = _paymentTermRepository.GetPaymentTerms().Where(x => x.IsActive).ToList();

                if (tempPaymentTerms != null && tempPaymentTerms.Count > 0)
                {
                    foreach (var tempPaymentTerm in tempPaymentTerms)
                    {
                        PaymentTermViewModel convertedModel = new PaymentTermConverter().ConvertToView(tempPaymentTerm);

                        paymentTerms.Add(convertedModel);
                    }
                }

                model.PaymentTerms = paymentTerms.OrderBy(x => x.PaymentTermDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/ShipmentTerms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ShipmentTerms()
        {
            var model = new ShipmentTermViewModel();

            var shipmentTerms = new List<ShipmentTermViewModel>();

            var tempShipmentTerms = _shipmentTermRepository.GetShipmentTerms().Where(x => x.IsActive).ToList();

            if (tempShipmentTerms != null && tempShipmentTerms.Count > 0)
            {
                foreach (var tempShipmentTerm in tempShipmentTerms)
                {
                    ShipmentTermViewModel convertedModel = new ShipmentTermConverter().ConvertToView(tempShipmentTerm);

                    shipmentTerms.Add(convertedModel);
                }
            }

            model.ShipmentTerms = shipmentTerms.OrderBy(x => x.ShipmentTermDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add shipment term modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddShipmentTerm()
        {
            return PartialView();
        }

        /// <summary>
        /// add shipment term
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddShipmentTerm(ShipmentTermViewModel model)
        {
            var operationResult = new OperationResult();

            ShipmentTerm newShipmentTerm = new ShipmentTermConverter().ConvertToDomain(model);

            operationResult = _shipmentTermRepository.SaveShipmentTerm(newShipmentTerm);

            if (operationResult.Success)
            {
                model.Success = true;

                var shipmentTerms = new List<ShipmentTermViewModel>();

                var tempShipmentTerms = _shipmentTermRepository.GetShipmentTerms().Where(x => x.IsActive).ToList();

                if (tempShipmentTerms != null && tempShipmentTerms.Count > 0)
                {
                    foreach (var tempShipmentTerm in tempShipmentTerms)
                    {
                        ShipmentTermViewModel convertedModel = new ShipmentTermConverter().ConvertToView(tempShipmentTerm);

                        shipmentTerms.Add(convertedModel);
                    }
                }

                model.ShipmentTerms = shipmentTerms.OrderBy(x => x.ShipmentTermDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit shipment term modal
        /// </summary>
        /// <param name="shipmentTermId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditShipmentTerm(Guid shipmentTermId)
        {
            var shipmentTerm = _shipmentTermRepository.GetShipmentTerm(shipmentTermId);

            ShipmentTermViewModel convertedModel = new ShipmentTermConverter().ConvertToView(shipmentTerm);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit shipment term
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditShipmentTerm(ShipmentTermViewModel model)
        {
            var operationResult = new OperationResult();

            ShipmentTerm shipmentTerm = new ShipmentTermConverter().ConvertToDomain(model);

            operationResult = _shipmentTermRepository.UpdateShipmentTerm(shipmentTerm);

            if (operationResult.Success)
            {
                model.Success = true;

                var shipmentTerms = new List<ShipmentTermViewModel>();

                var tempShipmentTerms = _shipmentTermRepository.GetShipmentTerms().Where(x => x.IsActive).ToList();

                if (tempShipmentTerms != null && tempShipmentTerms.Count > 0)
                {
                    foreach (var tempShipmentTerm in tempShipmentTerms)
                    {
                        ShipmentTermViewModel convertedModel = new ShipmentTermConverter().ConvertToView(tempShipmentTerm);

                        shipmentTerms.Add(convertedModel);
                    }
                }

                model.ShipmentTerms = shipmentTerms.OrderBy(x => x.ShipmentTermDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/Surcharge
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Surcharge()
        {
            var model = new SurchargeViewModel();

            var surcharges = new List<SurchargeViewModel>();

            var tempSurcharges = _surchargeRepository.GetSurcharges().Where(x => x.IsActive).ToList();

            if (tempSurcharges != null && tempSurcharges.Count > 0)
            {
                foreach (var tempSurcharge in tempSurcharges)
                {
                    SurchargeViewModel convertedModel = new SurchargeConverter().ConvertToView(tempSurcharge);

                    surcharges.Add(convertedModel);
                }
            }

            model.Surcharges = surcharges.OrderBy(x => x.SurchargeDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add surcharge modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddSurcharge()
        {
            return PartialView();
        }

        /// <summary>
        /// add surcharge
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddSurcharge(SurchargeViewModel model)
        {
            var operationResult = new OperationResult();

            Surcharge newSurcharge = new SurchargeConverter().ConvertToDomain(model);

            operationResult = _surchargeRepository.SaveSurcharge(newSurcharge);

            if (operationResult.Success)
            {
                model.Success = true;

                var surcharges = new List<SurchargeViewModel>();

                var tempSurcharges = _surchargeRepository.GetSurcharges().Where(x => x.IsActive).ToList();

                if (tempSurcharges != null && tempSurcharges.Count > 0)
                {
                    foreach (var tempSurcharge in tempSurcharges)
                    {
                        SurchargeViewModel convertedModel = new SurchargeConverter().ConvertToView(tempSurcharge);

                        surcharges.Add(convertedModel);
                    }
                }

                model.Surcharges = surcharges.OrderBy(x => x.SurchargeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit surcharge modal
        /// </summary>
        /// <param name="surchargeId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditSurcharge(Guid surchargeId)
        {
            var surcharge = _surchargeRepository.GetSurcharge(surchargeId);

            SurchargeViewModel convertedModel = new SurchargeConverter().ConvertToView(surcharge);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit surchage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditSurcharge(SurchargeViewModel model)
        {
            var operationResult = new OperationResult();

            Surcharge surcharge = new SurchargeConverter().ConvertToDomain(model);

            operationResult = _surchargeRepository.UpdateSurcharge(surcharge);

            if (operationResult.Success)
            {
                model.Success = true;

                var surcharges = new List<SurchargeViewModel>();

                var tempSurcharges = _surchargeRepository.GetSurcharges().Where(x => x.IsActive).ToList();

                if (tempSurcharges != null && tempSurcharges.Count > 0)
                {
                    foreach (var tempSurcharge in tempSurcharges)
                    {
                        SurchargeViewModel convertedModel = new SurchargeConverter().ConvertToView(tempSurcharge);

                        surcharges.Add(convertedModel);
                    }
                }

                model.Surcharges = surcharges.OrderBy(x => x.SurchargeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/TrackingCodes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult TrackingCodes()
        {
            var model = new TrackingCodeViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            var trackingCodes = new List<TrackingCodeViewModel>();

            var tempTrackingCodes = _trackingCodeRepository.GetTrackingCodes().Where(x => x.IsActive).ToList();

            if (tempTrackingCodes != null && tempTrackingCodes.Count > 0)
            {
                foreach (var tempTrackingCode in tempTrackingCodes)
                {
                    TrackingCodeViewModel convertedModel = new TrackingCodeConverter().ConvertToView(tempTrackingCode);

                    trackingCodes.Add(convertedModel);
                }
            }

            model.TrackingCodes = trackingCodes.OrderBy(x => x.TrackingCodeDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add tracking code modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddTrackingCode()
        {
            var model = new TrackingCodeViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            return PartialView(model);
        }

        /// <summary>
        /// add tracking code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddTrackingCode(TrackingCodeViewModel model)
        {
            var operationResult = new OperationResult();

            TrackingCode newTrackingCode = new TrackingCodeConverter().ConvertToDomain(model);

            operationResult = _trackingCodeRepository.SaveTrackingCode(newTrackingCode);

            if (operationResult.Success)
            {
                model.Success = true;

                var trackingCodes = new List<TrackingCodeViewModel>();

                var tempTrackingCodes = _trackingCodeRepository.GetTrackingCodes().Where(x => x.IsActive).ToList();

                if (tempTrackingCodes != null && tempTrackingCodes.Count > 0)
                {
                    foreach (var tempTrackingCode in tempTrackingCodes)
                    {
                        TrackingCodeViewModel convertedModel = new TrackingCodeConverter().ConvertToView(tempTrackingCode);

                        trackingCodes.Add(convertedModel);
                    }
                }

                model.TrackingCodes = trackingCodes.OrderBy(x => x.TrackingCodeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit tracking code
        /// </summary>
        /// <param name="trackingCodeId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditTrackingCode(Guid trackingCodeId)
        {
            var trackingCode = _trackingCodeRepository.GetTrackingCode(trackingCodeId);

            TrackingCodeViewModel model = new TrackingCodeConverter().ConvertToView(trackingCode);

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            return PartialView(model);
        }

        /// <summary>
        /// edit tracking code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditTrackingCode(TrackingCodeViewModel model)
        {
            var operationResult = new OperationResult();

            TrackingCode trackingCode = new TrackingCodeConverter().ConvertToDomain(model);

            operationResult = _trackingCodeRepository.UpdateTrackingCode(trackingCode);

            if (operationResult.Success)
            {
                model.Success = true;

                var trackingCodes = new List<TrackingCodeViewModel>();

                var tempTrackingCodes = _trackingCodeRepository.GetTrackingCodes().Where(x => x.IsActive).ToList();

                if (tempTrackingCodes != null && tempTrackingCodes.Count > 0)
                {
                    foreach (var tempTrackingCode in tempTrackingCodes)
                    {
                        TrackingCodeViewModel convertedModel = new TrackingCodeConverter().ConvertToView(tempTrackingCode);

                        trackingCodes.Add(convertedModel);
                    }
                }

                model.TrackingCodes = trackingCodes.OrderBy(x => x.TrackingCodeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/PattrtnMaterial
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult PatternMaterial()
        {
            var model = new PatternMaterialViewModel();

            var patternMaterials = new List<PatternMaterialViewModel>();

            var tempPatternMaterials = _patternMaterialRepository.GetPatternMaterials().Where(x => x.IsActive).ToList();

            if (tempPatternMaterials != null && tempPatternMaterials.Count > 0)
            {
                foreach (var tempPatternMaterial in tempPatternMaterials)
                {
                    PatternMaterialViewModel convertedModel = new PatternMaterialConverter().ConvertToView(tempPatternMaterial);

                    patternMaterials.Add(convertedModel);
                }
            }

            model.PatternMaterials = patternMaterials.OrderBy(x => x.PatternDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add pattern material modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddPatternMaterial()
        {
            return PartialView();
        }

        /// <summary>
        /// add parttern material
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddPatternMaterial(PatternMaterialViewModel model)
        {
            var operationResult = new OperationResult();

            PatternMaterial newPatternMaterial = new PatternMaterialConverter().ConvertToDomain(model);

            operationResult = _patternMaterialRepository.SavePatternMaterial(newPatternMaterial);

            if (operationResult.Success)
            {
                model.Success = true;

                var patternMaterials = new List<PatternMaterialViewModel>();

                var tempPatternMaterials = _patternMaterialRepository.GetPatternMaterials().Where(x => x.IsActive).ToList();

                if (tempPatternMaterials != null && tempPatternMaterials.Count > 0)
                {
                    foreach (var tempPatternMaterial in tempPatternMaterials)
                    {
                        PatternMaterialViewModel convertedModel = new PatternMaterialConverter().ConvertToView(tempPatternMaterial);

                        patternMaterials.Add(convertedModel);
                    }
                }

                model.PatternMaterials = patternMaterials.OrderBy(x => x.PatternDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit pattern material modal
        /// </summary>
        /// <param name="patternMaterialId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditPatternMaterial(Guid patternMaterialId)
        {
            var patternMaterial = _patternMaterialRepository.GetPatternMaterial(patternMaterialId);

            PatternMaterialViewModel convertedModel = new PatternMaterialConverter().ConvertToView(patternMaterial);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit pattern material
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditPatternMaterial(PatternMaterialViewModel model)
        {
            var operationResult = new OperationResult();

            PatternMaterial patternMaterial = new PatternMaterialConverter().ConvertToDomain(model);

            operationResult = _patternMaterialRepository.UpdatePatternMaterial(patternMaterial);

            if (operationResult.Success)
            {
                model.Success = true;

                var patternMaterials = new List<PatternMaterialViewModel>();

                var tempPatternMaterials = _patternMaterialRepository.GetPatternMaterials().Where(x => x.IsActive).ToList();

                if (tempPatternMaterials != null && tempPatternMaterials.Count > 0)
                {
                    foreach (var tempPatternMaterial in tempPatternMaterials)
                    {
                        PatternMaterialViewModel convertedModel = new PatternMaterialConverter().ConvertToView(tempPatternMaterial);

                        patternMaterials.Add(convertedModel);
                    }
                }

                model.PatternMaterials = patternMaterials.OrderBy(x => x.PatternDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Part/CoatingTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult CoatingTypes()
        {
            var model = new CoatingTypeViewModel();

            var coatingTypes = new List<CoatingTypeViewModel>();

            var tempCoatingTypes = _coatingTypeRepository.GetCoatingTypes().Where(x => x.IsActive).ToList();

            if (tempCoatingTypes != null && tempCoatingTypes.Count > 0)
            {
                foreach (var tempCoatingType in tempCoatingTypes)
                {
                    CoatingTypeViewModel convertedModel = new CoatingTypeConverter().ConvertToView(tempCoatingType);

                    coatingTypes.Add(convertedModel);
                }
            }

            model.CoatingTypes = coatingTypes.OrderBy(x => x.CoatingTypeDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add coating type modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddCoatingType()
        {
            return PartialView();
        }

        /// <summary>
        /// add coating type 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddCoatingType(CoatingTypeViewModel model)
        {
            var operationResult = new OperationResult();

            CoatingType newCoatingType = new CoatingTypeConverter().ConvertToDomain(model);

            operationResult = _coatingTypeRepository.SaveCoatingType(newCoatingType);

            if (operationResult.Success)
            {
                model.Success = true;

                var coatingTypes = new List<CoatingTypeViewModel>();

                var tempCoatingTypes = _coatingTypeRepository.GetCoatingTypes().Where(x => x.IsActive).ToList();

                if (tempCoatingTypes != null && tempCoatingTypes.Count > 0)
                {
                    foreach (var tempCoatingType in tempCoatingTypes)
                    {
                        CoatingTypeViewModel convertedModel = new CoatingTypeConverter().ConvertToView(tempCoatingType);

                        coatingTypes.Add(convertedModel);
                    }
                }

                model.CoatingTypes = coatingTypes.OrderBy(x => x.CoatingTypeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit coating type modal
        /// </summary>
        /// <param name="coatingTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditCoatingType(Guid coatingTypeId)
        {
            var coatingType = _coatingTypeRepository.GetCoatingType(coatingTypeId);

            CoatingTypeViewModel convertedModel = new CoatingTypeConverter().ConvertToView(coatingType);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit coating type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditCoatingType(CoatingTypeViewModel model)
        {
            var operationResult = new OperationResult();

            CoatingType coatingType = new CoatingTypeConverter().ConvertToDomain(model);

            operationResult = _coatingTypeRepository.UpdateCoatingType(coatingType);

            if (operationResult.Success)
            {
                model.Success = true;

                var coatingTypes = new List<CoatingTypeViewModel>();

                var tempCoatingTypes = _coatingTypeRepository.GetCoatingTypes().Where(x => x.IsActive).ToList();

                if (tempCoatingTypes != null && tempCoatingTypes.Count > 0)
                {
                    foreach (var tempCoatingType in tempCoatingTypes)
                    {
                        CoatingTypeViewModel convertedModel = new CoatingTypeConverter().ConvertToView(tempCoatingType);

                        coatingTypes.Add(convertedModel);
                    }
                }

                model.CoatingTypes = coatingTypes.OrderBy(x => x.CoatingTypeDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active htsNumbers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveHtsNumbers()
        {
            var model = new HtsNumberViewModel();

            var htsNumbers = new List<HtsNumberViewModel>();

            var tempHtsNumbers = _htsNumberRepository.GetHtsNumbers().Where(x => x.IsActive).ToList();

            if (tempHtsNumbers != null && tempHtsNumbers.Count > 0)
            {
                foreach (var tempHtsNumber in tempHtsNumbers)
                {
                    HtsNumberViewModel convertedModel = new HtsNumberConverter().ConvertToView(tempHtsNumber);

                    htsNumbers.Add(convertedModel);
                }
            }

            model.HtsNumbers = htsNumbers.OrderBy(x => x.HtsNumberDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive htsNumbers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveHtsNumbers()
        {
            var model = new HtsNumberViewModel();

            var htsNumbers = new List<HtsNumberViewModel>();

            var tempHtsNumbers = _htsNumberRepository.GetHtsNumbers().Where(x => !x.IsActive).ToList();

            if (tempHtsNumbers != null && tempHtsNumbers.Count > 0)
            {
                foreach (var tempHtsNumber in tempHtsNumbers)
                {
                    HtsNumberViewModel convertedModel = new HtsNumberConverter().ConvertToView(tempHtsNumber);

                    htsNumbers.Add(convertedModel);
                }
            }

            model.HtsNumbers = htsNumbers.OrderBy(x => x.HtsNumberDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active materials
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveMaterial()
        {
            var model = new MaterialViewModel();

            var materials = new List<MaterialViewModel>();

            var tempMaterials = _materialRepository.GetMaterials().Where(x => x.IsActive).ToList();

            if (tempMaterials != null && tempMaterials.Count > 0)
            {
                foreach (var tempMaterial in tempMaterials)
                {
                    MaterialViewModel convertedModel = new MaterialConverter().ConvertToView(tempMaterial);

                    materials.Add(convertedModel);
                }
            }

            model.Materials = materials.OrderBy(x => x.MaterialDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive material
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveMaterial()
        {
            var model = new MaterialViewModel();

            var materials = new List<MaterialViewModel>();

            var tempMaterials = _materialRepository.GetMaterials().Where(x => !x.IsActive).ToList();

            if (tempMaterials != null && tempMaterials.Count > 0)
            {
                foreach (var tempMaterial in tempMaterials)
                {
                    MaterialViewModel convertedModel = new MaterialConverter().ConvertToView(tempMaterial);

                    materials.Add(convertedModel);
                }
            }

            model.Materials = materials.OrderBy(x => x.MaterialDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active specifications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveSpecifications()
        {
            var model = new SpecificationMaterialViewModel();

            var specificationMaterials = new List<SpecificationMaterialViewModel>();

            var tempSpecificationMaterials = _specificationMaterialRepository.GetSpecificationMaterials().Where(x => x.IsActive).ToList();

            if (tempSpecificationMaterials != null && tempSpecificationMaterials.Count > 0)
            {
                foreach (var tempSpecificationMaterial in tempSpecificationMaterials)
                {
                    SpecificationMaterialViewModel convertedModel = new SpecificationMaterialConverter().ConvertToView(tempSpecificationMaterial);

                    specificationMaterials.Add(convertedModel);
                }
            }

            model.SpecificationMaterials = specificationMaterials.OrderBy(x => x.SpecificationDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive specifications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveSpecifications()
        {
            var model = new SpecificationMaterialViewModel();

            var specificationMaterials = new List<SpecificationMaterialViewModel>();

            var tempSpecificationMaterials = _specificationMaterialRepository.GetSpecificationMaterials().Where(x => !x.IsActive).ToList();

            if (tempSpecificationMaterials != null && tempSpecificationMaterials.Count > 0)
            {
                foreach (var tempSpecificationMaterial in tempSpecificationMaterials)
                {
                    SpecificationMaterialViewModel convertedModel = new SpecificationMaterialConverter().ConvertToView(tempSpecificationMaterial);

                    specificationMaterials.Add(convertedModel);
                }
            }

            model.SpecificationMaterials = specificationMaterials.OrderBy(x => x.SpecificationDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active part states
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActivePartStates()
        {
            var model = new PartStatusViewModel();

            var partStates = new List<PartStatusViewModel>();

            var tempPartStates = _partStatusRepository.GetPartStates().Where(x => x.IsActive).ToList();

            if (tempPartStates != null && tempPartStates.Count > 0)
            {
                foreach (var tempPartStatus in tempPartStates)
                {
                    PartStatusViewModel convertedModel = new PartStatusConverter().ConvertToView(tempPartStatus);

                    partStates.Add(convertedModel);
                }
            }

            model.PartStates = partStates.OrderBy(x => x.PartStatusDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive part states
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactivePartStates()
        {
            var model = new PartStatusViewModel();

            var partStates = new List<PartStatusViewModel>();

            var tempPartStates = _partStatusRepository.GetPartStates().Where(x => !x.IsActive).ToList();

            if (tempPartStates != null && tempPartStates.Count > 0)
            {
                foreach (var tempPartStatus in tempPartStates)
                {
                    PartStatusViewModel convertedModel = new PartStatusConverter().ConvertToView(tempPartStatus);

                    partStates.Add(convertedModel);
                }
            }

            model.PartStates = partStates.OrderBy(x => x.PartStatusDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active part types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActivePartTypes()
        {
            var model = new PartTypeViewModel();

            var partTypes = new List<PartTypeViewModel>();

            var tempPartTypes = _partTypeRepository.GetPartTypes().Where(x => x.IsActive).ToList();

            if (tempPartTypes != null && tempPartTypes.Count > 0)
            {
                foreach (var tempPartType in tempPartTypes)
                {
                    PartTypeViewModel convertedModel = new PartTypeConverter().ConvertToView(tempPartType);

                    partTypes.Add(convertedModel);
                }
            }

            model.PartTypes = partTypes.OrderBy(x => x.PartTypeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive part types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactivePartTypes()
        {
            var model = new PartTypeViewModel();

            var partTypes = new List<PartTypeViewModel>();

            var tempPartTypes = _partTypeRepository.GetPartTypes().Where(x => !x.IsActive).ToList();

            if (tempPartTypes != null && tempPartTypes.Count > 0)
            {
                foreach (var tempPartType in tempPartTypes)
                {
                    PartTypeViewModel convertedModel = new PartTypeConverter().ConvertToView(tempPartType);

                    partTypes.Add(convertedModel);
                }
            }

            model.PartTypes = partTypes.OrderBy(x => x.PartTypeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active payment terms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActivePaymentTerms()
        {
            var model = new PaymentTermViewModel();

            var paymentTerms = new List<PaymentTermViewModel>();

            var tempPaymentTerms = _paymentTermRepository.GetPaymentTerms().Where(x => x.IsActive).ToList();

            if (tempPaymentTerms != null && tempPaymentTerms.Count > 0)
            {
                foreach (var tempPaymentTerm in tempPaymentTerms)
                {
                    PaymentTermViewModel convertedModel = new PaymentTermConverter().ConvertToView(tempPaymentTerm);

                    paymentTerms.Add(convertedModel);
                }
            }

            model.PaymentTerms = paymentTerms.OrderBy(x => x.PaymentTermDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inavtive payment terms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactivePaymentTerms()
        {
            var model = new PaymentTermViewModel();

            var paymentTerms = new List<PaymentTermViewModel>();

            var tempPaymentTerms = _paymentTermRepository.GetPaymentTerms().Where(x => !x.IsActive).ToList();

            if (tempPaymentTerms != null && tempPaymentTerms.Count > 0)
            {
                foreach (var tempPaymentTerm in tempPaymentTerms)
                {
                    PaymentTermViewModel convertedModel = new PaymentTermConverter().ConvertToView(tempPaymentTerm);

                    paymentTerms.Add(convertedModel);
                }
            }

            model.PaymentTerms = paymentTerms.OrderBy(x => x.PaymentTermDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active shipment terms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveShipmentTerms()
        {
            var model = new ShipmentTermViewModel();

            var shipmentTerms = new List<ShipmentTermViewModel>();

            var tempShipmentTerms = _shipmentTermRepository.GetShipmentTerms().Where(x => x.IsActive).ToList();

            if (tempShipmentTerms != null && tempShipmentTerms.Count > 0)
            {
                foreach (var tempShipmentTerm in tempShipmentTerms)
                {
                    ShipmentTermViewModel convertedModel = new ShipmentTermConverter().ConvertToView(tempShipmentTerm);

                    shipmentTerms.Add(convertedModel);
                }
            }

            model.ShipmentTerms = shipmentTerms.OrderBy(x => x.ShipmentTermDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive shipment terms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveShipmentTerms()
        {
            var model = new ShipmentTermViewModel();

            var shipmentTerms = new List<ShipmentTermViewModel>();

            var tempShipmentTerms = _shipmentTermRepository.GetShipmentTerms().Where(x => !x.IsActive).ToList();

            if (tempShipmentTerms != null && tempShipmentTerms.Count > 0)
            {
                foreach (var tempShipmentTerm in tempShipmentTerms)
                {
                    ShipmentTermViewModel convertedModel = new ShipmentTermConverter().ConvertToView(tempShipmentTerm);

                    shipmentTerms.Add(convertedModel);
                }
            }

            model.ShipmentTerms = shipmentTerms.OrderBy(x => x.ShipmentTermDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active surcharge
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveSurcharge()
        {
            var model = new SurchargeViewModel();

            var surcharges = new List<SurchargeViewModel>();

            var tempSurcharges = _surchargeRepository.GetSurcharges().Where(x => x.IsActive).ToList();

            if (tempSurcharges != null && tempSurcharges.Count > 0)
            {
                foreach (var tempSurcharge in tempSurcharges)
                {
                    SurchargeViewModel convertedModel = new SurchargeConverter().ConvertToView(tempSurcharge);

                    surcharges.Add(convertedModel);
                }
            }

            model.Surcharges = surcharges.OrderBy(x => x.SurchargeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive surcharge
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveSurcharge()
        {
            var model = new SurchargeViewModel();

            var surcharges = new List<SurchargeViewModel>();

            var tempSurcharges = _surchargeRepository.GetSurcharges().Where(x => !x.IsActive).ToList();

            if (tempSurcharges != null && tempSurcharges.Count > 0)
            {
                foreach (var tempSurcharge in tempSurcharges)
                {
                    SurchargeViewModel convertedModel = new SurchargeConverter().ConvertToView(tempSurcharge);

                    surcharges.Add(convertedModel);
                }
            }

            model.Surcharges = surcharges.OrderBy(x => x.SurchargeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get acitve tracking codes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveTrackingCodes()
        {
            var model = new TrackingCodeViewModel();

            var trackingCodes = new List<TrackingCodeViewModel>();

            var tempTrackingCodes = _trackingCodeRepository.GetTrackingCodes().Where(x => x.IsActive).ToList();

            if (tempTrackingCodes != null && tempTrackingCodes.Count > 0)
            {
                foreach (var tempTrackingCode in tempTrackingCodes)
                {
                    TrackingCodeViewModel convertedModel = new TrackingCodeConverter().ConvertToView(tempTrackingCode);

                    trackingCodes.Add(convertedModel);
                }
            }

            model.TrackingCodes = trackingCodes.OrderBy(x => x.TrackingCodeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive tracking codes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveTrackingCodes()
        {
            var model = new TrackingCodeViewModel();

            var trackingCodes = new List<TrackingCodeViewModel>();

            var tempTrackingCodes = _trackingCodeRepository.GetTrackingCodes().Where(x => !x.IsActive).ToList();

            if (tempTrackingCodes != null && tempTrackingCodes.Count > 0)
            {
                foreach (var tempTrackingCode in tempTrackingCodes)
                {
                    TrackingCodeViewModel convertedModel = new TrackingCodeConverter().ConvertToView(tempTrackingCode);

                    trackingCodes.Add(convertedModel);
                }
            }

            model.TrackingCodes = trackingCodes.OrderBy(x => x.TrackingCodeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active coating types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveCoatingTypes()
        {
            var model = new CoatingTypeViewModel();

            var coatingTypes = new List<CoatingTypeViewModel>();

            var tempCoatingTypes = _coatingTypeRepository.GetCoatingTypes().Where(x => x.IsActive).ToList();

            if (tempCoatingTypes != null && tempCoatingTypes.Count > 0)
            {
                foreach (var tempCoatingType in tempCoatingTypes)
                {
                    CoatingTypeViewModel convertedModel = new CoatingTypeConverter().ConvertToView(tempCoatingType);

                    coatingTypes.Add(convertedModel);
                }
            }

            model.CoatingTypes = coatingTypes.OrderBy(x => x.CoatingTypeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive coating types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveCoatingTypes()
        {
            var model = new CoatingTypeViewModel();

            var coatingTypes = new List<CoatingTypeViewModel>();

            var tempCoatingTypes = _coatingTypeRepository.GetCoatingTypes().Where(x => !x.IsActive).ToList();

            if (tempCoatingTypes != null && tempCoatingTypes.Count > 0)
            {
                foreach (var tempCoatingType in tempCoatingTypes)
                {
                    CoatingTypeViewModel convertedModel = new CoatingTypeConverter().ConvertToView(tempCoatingType);

                    coatingTypes.Add(convertedModel);
                }
            }

            model.CoatingTypes = coatingTypes.OrderBy(x => x.CoatingTypeDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active patterns
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActivePatterns()
        {
            var model = new PatternMaterialViewModel();

            var patternMaterials = new List<PatternMaterialViewModel>();

            var tempPatternMaterials = _patternMaterialRepository.GetPatternMaterials().Where(x => x.IsActive).ToList();

            if (tempPatternMaterials != null && tempPatternMaterials.Count > 0)
            {
                foreach (var tempPatternMaterial in tempPatternMaterials)
                {
                    PatternMaterialViewModel convertedModel = new PatternMaterialConverter().ConvertToView(tempPatternMaterial);

                    patternMaterials.Add(convertedModel);
                }
            }

            model.PatternMaterials = patternMaterials.OrderBy(x => x.PatternDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive patterns
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactivePatterns()
        {
            var model = new PatternMaterialViewModel();

            var patternMaterials = new List<PatternMaterialViewModel>();

            var tempPatternMaterials = _patternMaterialRepository.GetPatternMaterials().Where(x => !x.IsActive).ToList();

            if (tempPatternMaterials != null && tempPatternMaterials.Count > 0)
            {
                foreach (var tempPatternMaterial in tempPatternMaterials)
                {
                    PatternMaterialViewModel convertedModel = new PatternMaterialConverter().ConvertToView(tempPatternMaterial);

                    patternMaterials.Add(convertedModel);
                }
            }

            model.PatternMaterials = patternMaterials.OrderBy(x => x.PatternDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_htsNumberRepository != null)
                {
                    _htsNumberRepository.Dispose();
                    _htsNumberRepository = null;
                }

                if (_materialRepository != null)
                {
                    _materialRepository.Dispose();
                    _materialRepository = null;
                }

                if (_specificationMaterialRepository != null)
                {
                    _specificationMaterialRepository.Dispose();
                    _specificationMaterialRepository = null;
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

                if (_shipmentTermRepository != null)
                {
                    _shipmentTermRepository.Dispose();
                    _shipmentTermRepository = null;
                }

                if (_surchargeRepository != null)
                {
                    _surchargeRepository.Dispose();
                    _surchargeRepository = null;
                }

                if (_trackingCodeRepository != null)
                {
                    _trackingCodeRepository.Dispose();
                    _trackingCodeRepository = null;
                }

                if (_patternMaterialRepository != null)
                {
                    _patternMaterialRepository.Dispose();
                    _patternMaterialRepository = null;
                }

                if (_coatingTypeRepository != null)
                {
                    _coatingTypeRepository.Dispose();
                    _coatingTypeRepository = null;
                }

                if (_customerDynamicsRepository != null)
                {
                    _customerDynamicsRepository.Dispose();
                    _customerDynamicsRepository = null;
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