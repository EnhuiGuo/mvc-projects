using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
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
    public class ShipmentController : ApplicationBaseController
    {
        private ICarrierRepository _carrierRepository;
        private IDestinationRepository _destinationRepository;
        private IPortRepository _portRepository;
        private IVesselRepository _vesselRepository;

        public ShipmentController()
        {
            _carrierRepository = new CarrierRepository();
            _destinationRepository = new DestinationRepository();
            _portRepository = new PortRepository();
            _vesselRepository = new VesselRepository();
        }

        public ShipmentController(ICarrierRepository carrierRepository,
                                  IDestinationRepository destinationRepository,
                                  IPortRepository portRepository,
                                  IVesselRepository vesselRepository)
        {
            _carrierRepository = carrierRepository;
            _destinationRepository = destinationRepository;
            _portRepository = portRepository;
            _vesselRepository = vesselRepository;
        }

        /// <summary>
        /// GET: Administration/Shipment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: Administration/Shipment/Carriers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Carriers()
        {
            var model = new CarrierViewModel();

            var carriers = new List<CarrierViewModel>();

            var tempCarriers = _carrierRepository.GetCarriers().Where(x => x.IsActive).ToList();

            if (tempCarriers != null && tempCarriers.Count > 0)
            {
                foreach (var tempCarrier in tempCarriers)
                {
                    CarrierViewModel convertedModel = new CarrierConverter().ConvertToView(tempCarrier);

                    carriers.Add(convertedModel);
                }
            }

            model.Carriers = carriers.OrderBy(x => x.CarrierName).ToList();

            return View(model);
        }

        /// <summary>
        /// add carrier modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddCarrier()
        {
            return PartialView();
        }

        /// <summary>
        /// add carrier
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddCarrier(CarrierViewModel model)
        {
            var operationResult = new OperationResult();

            Carrier newCarrier = new CarrierConverter().ConvertToDomain(model);

            operationResult = _carrierRepository.SaveCarrier(newCarrier);

            if (operationResult.Success)
            {
                model.Success = true;

                var carriers = new List<CarrierViewModel>();

                var tempCarriers = _carrierRepository.GetCarriers().Where(x => x.IsActive).ToList();

                if (tempCarriers != null && tempCarriers.Count > 0)
                {
                    foreach (var tempCarrier in tempCarriers)
                    {
                        CarrierViewModel convertedModel = new CarrierConverter().ConvertToView(tempCarrier);

                        carriers.Add(convertedModel);
                    }
                }

                model.Carriers = carriers.OrderBy(x => x.CarrierName).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit carrier modal
        /// </summary>
        /// <param name="carrierId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditCarrier(Guid carrierId)
        {
            var carrier = _carrierRepository.GetCarrier(carrierId);

            CarrierViewModel convertedModel = new CarrierConverter().ConvertToView(carrier);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit carrier
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditCarrier(CarrierViewModel model)
        {
            var operationResult = new OperationResult();

            Carrier carrier = new CarrierConverter().ConvertToDomain(model);

            operationResult = _carrierRepository.UpdateCarrier(carrier);

            if (operationResult.Success)
            {
                model.Success = true;

                var carriers = new List<CarrierViewModel>();

                var tempCarriers = _carrierRepository.GetCarriers().Where(x => x.IsActive).ToList();

                if (tempCarriers != null && tempCarriers.Count > 0)
                {
                    foreach (var tempCarrier in tempCarriers)
                    {
                        CarrierViewModel convertedModel = new CarrierConverter().ConvertToView(tempCarrier);

                        carriers.Add(convertedModel);
                    }
                }

                model.Carriers = carriers.OrderBy(x => x.CarrierName).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Shipment/Destinations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Destinations()
        {
            var model = new DestinationViewModel();

            var destinations = new List<DestinationViewModel>();

            var tempDestinations = _destinationRepository.GetDestinations().Where(x => x.IsActive).ToList();

            if (tempDestinations != null && tempDestinations.Count > 0)
            {
                foreach (var tempDestination in tempDestinations)
                {
                    DestinationViewModel convertedModel = new DestinationConverter().ConvertToView(tempDestination);

                    destinations.Add(convertedModel);
                }
            }

            model.Destinations = destinations.OrderBy(x => x.DestinationDescription).ToList();

            return View(model);
        }

        /// <summary>
        /// add destination
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddDestination()
        {
            return PartialView();
        }

        /// <summary>
        /// add destination
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddDestination(DestinationViewModel model)
        {
            var operationResult = new OperationResult();

            Destination newDestination = new DestinationConverter().ConvertToDomain(model);

            operationResult = _destinationRepository.SaveDestination(newDestination);

            if (operationResult.Success)
            {
                model.Success = true;

                var destinations = new List<DestinationViewModel>();

                var tempDestinations = _destinationRepository.GetDestinations().Where(x => x.IsActive).ToList();

                if (tempDestinations != null && tempDestinations.Count > 0)
                {
                    foreach (var tempDestination in tempDestinations)
                    {
                        DestinationViewModel convertedModel = new DestinationConverter().ConvertToView(tempDestination);

                        destinations.Add(convertedModel);
                    }
                }

                model.Destinations = destinations.OrderBy(x => x.DestinationDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit destination modal
        /// </summary>
        /// <param name="destinationId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditDestination(Guid destinationId)
        {
            var destination = _destinationRepository.GetDestination(destinationId);

            DestinationViewModel convertedModel = new DestinationConverter().ConvertToView(destination);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edti destination
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditDestination(DestinationViewModel model)
        {
            var operationResult = new OperationResult();

            Destination destination = new DestinationConverter().ConvertToDomain(model);

            operationResult = _destinationRepository.UpdateDestination(destination);

            if (operationResult.Success)
            {
                model.Success = true;

                var destinations = new List<DestinationViewModel>();

                var tempDestinations = _destinationRepository.GetDestinations().Where(x => x.IsActive).ToList();

                if (tempDestinations != null && tempDestinations.Count > 0)
                {
                    foreach (var tempDestination in tempDestinations)
                    {
                        DestinationViewModel convertedModel = new DestinationConverter().ConvertToView(tempDestination);

                        destinations.Add(convertedModel);
                    }
                }

                model.Destinations = destinations.OrderBy(x => x.DestinationDescription).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Shipment/Ports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Ports()
        {
            var model = new PortViewModel();

            var ports = new List<PortViewModel>();

            var tempPorts = _portRepository.GetPorts().Where(x => x.IsActive).ToList();

            if (tempPorts != null && tempPorts.Count > 0)
            {
                foreach (var tempPort in tempPorts)
                {
                    PortViewModel convertedModel = new PortConverter().ConvertToView(tempPort);

                    ports.Add(convertedModel);
                }
            }

            model.Ports = ports.OrderBy(x => x.PortName).ToList();

            return View(model);
        }

        /// <summary>
        /// add port modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddPort()
        {
            return PartialView();
        }

        /// <summary>
        /// add port 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddPort(PortViewModel model)
        {
            var operationResult = new OperationResult();

            Port newPort = new PortConverter().ConvertToDomain(model);

            operationResult = _portRepository.SavePort(newPort);

            if (operationResult.Success)
            {
                model.Success = true;

                var ports = new List<PortViewModel>();

                var tempPorts = _portRepository.GetPorts().Where(x => x.IsActive).ToList();

                if (tempPorts != null && tempPorts.Count > 0)
                {
                    foreach (var tempPort in tempPorts)
                    {
                        PortViewModel convertedModel = new PortConverter().ConvertToView(tempPort);

                        ports.Add(convertedModel);
                    }
                }

                model.Ports = ports.OrderBy(x => x.PortName).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit port 
        /// </summary>
        /// <param name="portId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditPort(Guid portId)
        {
            var port = _portRepository.GetPort(portId);

            PortViewModel convertedModel = new PortConverter().ConvertToView(port);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit port 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditPort(PortViewModel model)
        {
            var operationResult = new OperationResult();

            Port port = new PortConverter().ConvertToDomain(model);

            operationResult = _portRepository.UpdatePort(port);

            if (operationResult.Success)
            {
                model.Success = true;

                var ports = new List<PortViewModel>();

                var tempPorts = _portRepository.GetPorts().Where(x => x.IsActive).ToList();

                if (tempPorts != null && tempPorts.Count > 0)
                {
                    foreach (var tempPort in tempPorts)
                    {
                        PortViewModel convertedModel = new PortConverter().ConvertToView(tempPort);

                        ports.Add(convertedModel);
                    }
                }

                model.Ports = ports.OrderBy(x => x.PortName).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Administration/Shipment/Vessels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Vessels()
        {
            var model = new VesselViewModel();

            var vessels = new List<VesselViewModel>();

            var tempVessels = _vesselRepository.GetVessels().Where(x => x.IsActive).ToList();

            if (tempVessels != null && tempVessels.Count > 0)
            {
                foreach (var tempVessel in tempVessels)
                {
                    VesselViewModel convertedModel = new VesselConverter().ConvertToView(tempVessel);

                    vessels.Add(convertedModel);
                }
            }

            model.Vessels = vessels.OrderBy(x => x.VesselName).ToList();

            return View(model);
        }

        /// <summary>
        /// add vessel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _AddVessel()
        {
            return PartialView();
        }

        /// <summary>
        /// add vessel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult AddVessel(VesselViewModel model)
        {
            var operationResult = new OperationResult();

            Vessel newVessel = new VesselConverter().ConvertToDomain(model);

            operationResult = _vesselRepository.SaveVessel(newVessel);

            if (operationResult.Success)
            {
                model.Success = true;

                var vessels = new List<VesselViewModel>();

                var tempVessels = _vesselRepository.GetVessels().Where(x => x.IsActive).ToList();

                if (tempVessels != null && tempVessels.Count > 0)
                {
                    foreach (var tempVessel in tempVessels)
                    {
                        VesselViewModel convertedModel = new VesselConverter().ConvertToView(tempVessel);

                        vessels.Add(convertedModel);
                    }
                }

                model.Vessels = vessels.OrderBy(x => x.VesselName).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit vessel modal
        /// </summary>
        /// <param name="vesselId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult _EditVessel(Guid vesselId)
        {
            var vessel = _vesselRepository.GetVessel(vesselId);

            VesselViewModel convertedModel = new VesselConverter().ConvertToView(vessel);

            return PartialView(convertedModel);
        }

        /// <summary>
        /// edit vessel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditVessel(VesselViewModel model)
        {
            var operationResult = new OperationResult();

            Vessel vessel = new VesselConverter().ConvertToDomain(model);

            operationResult = _vesselRepository.UpdateVessel(vessel);

            if (operationResult.Success)
            {
                model.Success = true;

                var vessels = new List<VesselViewModel>();

                var tempVessels = _vesselRepository.GetVessels().Where(x => x.IsActive).ToList();

                if (tempVessels != null && tempVessels.Count > 0)
                {
                    foreach (var tempVessel in tempVessels)
                    {
                        VesselViewModel convertedModel = new VesselConverter().ConvertToView(tempVessel);

                        vessels.Add(convertedModel);
                    }
                }

                model.Vessels = vessels.OrderBy(x => x.VesselName).ToList();
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active carriers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveCarriers()
        {
            var model = new CarrierViewModel();

            var carriers = new List<CarrierViewModel>();

            var tempCarriers = _carrierRepository.GetCarriers().Where(x => x.IsActive).ToList();

            if (tempCarriers != null && tempCarriers.Count > 0)
            {
                foreach (var tempCarrier in tempCarriers)
                {
                    CarrierViewModel convertedModel = new CarrierConverter().ConvertToView(tempCarrier);

                    carriers.Add(convertedModel);
                }
            }

            model.Carriers = carriers.OrderBy(x => x.CarrierName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inavtive carriers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveCarriers()
        {
            var model = new CarrierViewModel();

            var carriers = new List<CarrierViewModel>();

            var tempCarriers = _carrierRepository.GetCarriers().Where(x => !x.IsActive).ToList();

            if (tempCarriers != null && tempCarriers.Count > 0)
            {
                foreach (var tempCarrier in tempCarriers)
                {
                    CarrierViewModel convertedModel = new CarrierConverter().ConvertToView(tempCarrier);

                    carriers.Add(convertedModel);
                }
            }

            model.Carriers = carriers.OrderBy(x => x.CarrierName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active destinations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveDestinations()
        {
            var model = new DestinationViewModel();

            var destinations = new List<DestinationViewModel>();

            var tempDestinations = _destinationRepository.GetDestinations().Where(x => x.IsActive).ToList();

            if (tempDestinations != null && tempDestinations.Count > 0)
            {
                foreach (var tempDestination in tempDestinations)
                {
                    DestinationViewModel convertedModel = new DestinationConverter().ConvertToView(tempDestination);

                    destinations.Add(convertedModel);
                }
            }

            model.Destinations = destinations.OrderBy(x => x.DestinationDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive destinations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveDestinations()
        {
            var model = new DestinationViewModel();

            var destinations = new List<DestinationViewModel>();

            var tempDestinations = _destinationRepository.GetDestinations().Where(x => !x.IsActive).ToList();

            if (tempDestinations != null && tempDestinations.Count > 0)
            {
                foreach (var tempDestination in tempDestinations)
                {
                    DestinationViewModel convertedModel = new DestinationConverter().ConvertToView(tempDestination);

                    destinations.Add(convertedModel);
                }
            }

            model.Destinations = destinations.OrderBy(x => x.DestinationDescription).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active ports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActivePorts()
        {
            var model = new PortViewModel();

            var ports = new List<PortViewModel>();

            var tempPorts = _portRepository.GetPorts().Where(x => x.IsActive).ToList();

            if (tempPorts != null && tempPorts.Count > 0)
            {
                foreach (var tempPort in tempPorts)
                {
                    PortViewModel convertedModel = new PortConverter().ConvertToView(tempPort);

                    ports.Add(convertedModel);
                }
            }

            model.Ports = ports.OrderBy(x => x.PortName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive ports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactivePorts()
        {
            var model = new PortViewModel();

            var ports = new List<PortViewModel>();

            var tempPorts = _portRepository.GetPorts().Where(x => !x.IsActive).ToList();

            if (tempPorts != null && tempPorts.Count > 0)
            {
                foreach (var tempPort in tempPorts)
                {
                    PortViewModel convertedModel = new PortConverter().ConvertToView(tempPort);

                    ports.Add(convertedModel);
                }
            }

            model.Ports = ports.OrderBy(x => x.PortName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get active vessels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetActiveVessels()
        {
            var model = new VesselViewModel();

            var vessels = new List<VesselViewModel>();

            var tempVessels = _vesselRepository.GetVessels().Where(x => x.IsActive).ToList();

            if (tempVessels != null && tempVessels.Count > 0)
            {
                foreach (var tempVessel in tempVessels)
                {
                    VesselViewModel convertedModel = new VesselConverter().ConvertToView(tempVessel);

                    vessels.Add(convertedModel);
                }
            }

            model.Vessels = vessels.OrderBy(x => x.VesselName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get inactive vessels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetInactiveVessels()
        {
            var model = new VesselViewModel();

            var vessels = new List<VesselViewModel>();

            var tempVessels = _vesselRepository.GetVessels().Where(x => !x.IsActive).ToList();

            if (tempVessels != null && tempVessels.Count > 0)
            {
                foreach (var tempVessel in tempVessels)
                {
                    VesselViewModel convertedModel = new VesselConverter().ConvertToView(tempVessel);

                    vessels.Add(convertedModel);
                }
            }

            model.Vessels = vessels.OrderBy(x => x.VesselName).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_carrierRepository != null)
                {
                    _carrierRepository.Dispose();
                    _carrierRepository = null;
                }

                if (_destinationRepository != null)
                {
                    _destinationRepository.Dispose();
                    _destinationRepository = null;
                }

                if (_portRepository != null)
                {
                    _portRepository.Dispose();
                    _portRepository = null;
                }

                if (_vesselRepository != null)
                {
                    _vesselRepository.Dispose();
                    _vesselRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}