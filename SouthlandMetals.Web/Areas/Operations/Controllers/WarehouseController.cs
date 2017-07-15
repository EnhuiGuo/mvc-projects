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
    public class WarehouseController : ApplicationBaseController
    {
        private IPackingListRepository _packingListRepository;
        private IPartRepository _partRepository;
        private IPartDynamicsRepository _partDynamicsRepository;
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private ICustomerAddressDynamicsRepository _customerAddressDynamicsRepository;
        private ICarrierRepository _carrierRepository;
        private ICountryRepository _countryRepository;
        private IContainerRepository _containerRepository;
        private ICustomerOrderRepository _customerOrderRepository;
        private IFoundryOrderRepository _foundryOrderRepository;

        public WarehouseController()
        {
            _packingListRepository = new PackingListRepository();
            _partRepository = new PartRepository();
            _partDynamicsRepository = new PartDynamicsRepository();
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            _carrierRepository = new CarrierRepository();
            _countryRepository = new CountryRepository();
            _containerRepository = new ContainerRepository();
            _customerOrderRepository = new CustomerOrderRepository();
            _foundryOrderRepository = new FoundryOrderRepository();
        }

        public WarehouseController(IPackingListRepository packingListRepository,
                                   IPartRepository partRepository,
                                   IPartDynamicsRepository partDynamicsRepository,
                                   ICustomerDynamicsRepository customerDynamicsRepository,
                                   ICustomerAddressDynamicsRepository customerAddressDynamicsRepository,
                                   ICarrierRepository carrierRepository,
                                   ICountryRepository countryRepository,
                                   IContainerRepository containerRepository,
                                   ICustomerOrderRepository customerOrderRepository,
                                   IFoundryOrderRepository foundryOrderRepository)
        {
            _packingListRepository = packingListRepository;
            _partRepository = partRepository;
            _partDynamicsRepository = partDynamicsRepository;
            _customerDynamicsRepository = customerDynamicsRepository;
            _customerAddressDynamicsRepository = customerAddressDynamicsRepository;
            _carrierRepository = carrierRepository;
            _containerRepository = containerRepository;
            _customerOrderRepository = customerOrderRepository;
            _foundryOrderRepository = foundryOrderRepository;
        }

        private IEnumerable<PackingListPartViewModel> ConsolidatePalletParts(List<PackingListPartViewModel> parts)
        {
            List<PackingListPartViewModel> consolidatedParts = parts.GroupBy(x => new { x.PartNumber, x.PONumber }).Select(g => new PackingListPartViewModel
            {
                ShipCode = g.First().ShipCode,
                PartNumber = g.First().PartNumber,
                PalletNumbers = g.Select(x => x.PalletNumber).ToList(),
                PalletQuantity = g.First().PalletQuantity,
                PartWeight = g.First().PartWeight,
                TotalPalletQuantity = g.Sum(x => x.TotalPalletQuantity),
                PONumber = g.First().PONumber,
                InvoiceNumber = g.First().InvoiceNumber,
            }).ToList();

            if (consolidatedParts != null)
            {
                foreach (var part in consolidatedParts)
                {
                    if (part.PalletNumbers != null)
                    {
                        part.PalletNumbers = part.PalletNumbers.Distinct().ToList();

                        part.PalletTotal = part.PalletNumbers.Count;
                        part.TotalPalletQuantity = part.PalletTotal * part.PalletQuantity;
                        part.PalletWeight = 50.00m;

                        if (part.PalletNumbers != null && part.PalletNumbers.Count > 1)
                        {
                            part.PalletNumber = part.PalletNumbers.First() + '-' + part.PalletNumbers.Last();
                        }
                        else
                        {
                            part.PalletNumber = part.PalletNumbers.First();
                        }
                    }
                }
            }

            consolidatedParts = consolidatedParts.OrderBy(x => x.ShipCode).ThenBy(y => y.PartNumber).ToList();

            return consolidatedParts;
        }

        /// <summary>
        /// GET: Operations/Warehouse
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: Operations/Warehouse/Inventory 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Inventory()
        {
            var model = new WarehouseViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableCountries = _countryRepository.GetSelectableCountries();

            var defaultCountry = new SelectListItem()
            {
                Text = "--Select Country--",
                Value = null
            };

            model.SelectableCountries.Insert(0, defaultCountry);

            return View(model);
        }

        /// <summary>
        /// search warehouse inventory by customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SearchWarehouseInventory(string customerId)
        {
            var inventory = new List<WarehouseInventoryViewModel>();

            var tempParts = _partDynamicsRepository.GetPartQuantityMasters().Where(x => x.QTYONHND > 0.00m).ToList();

            if (tempParts != null && tempParts.Count > 0)
            {
                foreach (var tempPart in tempParts)
                {
                    var parts = _partRepository.GetParts().Where(x => x.Number == tempPart.ITEMNMBR).ToList();

                    foreach (var part in parts)
                    {
                        var foundryOrderParts = _foundryOrderRepository.GetFoundryOrderParts().Where(x => x.PartId == part.PartId).ToList();

                        foreach (var foundryOrderPart in foundryOrderParts)
                        {
                            var containerParts = _containerRepository.GetContainerParts().Where(x => x.FoundryOrderPartId == foundryOrderPart.FoundryOrderPartId).ToList();

                            foreach(var containerPart in containerParts)
                            {
                                WarehouseInventoryViewModel convertedModel = new WarehouseInventoryConverter().ConvertToListView(containerPart);

                                inventory.Add(convertedModel);
                            }
                        }
                    }
                }
            }

            if (customerId != "--Select Customer--")
            {
                inventory = inventory.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == customerId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            inventory = inventory.OrderBy(x => x.ShipCode).ThenBy(y => y.PartNumber).ThenBy(z => z.PalletNumber).ToList();

            var jsonResult = Json(inventory, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// GET: Operations/Warehouse/PackingLists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult PackingLists()
        {
            var model = new PackingListViewModel();

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

            model.PackingLists = packingLists.OrderBy(x => x.CustomerName).ThenByDescending(y => y.CreatedDate).ToList();

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Warehouse/CreatePackingList
        /// </summary>
        /// <param name="palletNumbers"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreatePackingList(List<string> palletNumbers, string customerId)
        {
            var model = new PackingListViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableCarriers = _carrierRepository.GetSelectableCarriers();

            var defaultCarrier = new SelectListItem()
            {
                Text = "--Select Carrier--",
                Value = null
            };

            model.SelectableCarriers.Insert(0, defaultCarrier);

            model.ShipDateStr = DateTime.Now.ToShortDateString();

            var parts = new List<PackingListPartViewModel>();

            var pallets = palletNumbers[0].Split(',').ToList();

            if (pallets != null && pallets.Count > 0)
            {
                foreach (var pallet in pallets)
                {
                    var tempParts = _containerRepository.GetContainerParts().Where(x => x.PalletNumber == pallet).ToList();

                    if (tempParts != null && tempParts.Count > 0)
                    {
                        foreach (var tempPart in tempParts)
                        {
                            PackingListPartViewModel convertedModel = new PackingListPartConverter().ConvertToView(tempPart);
                            parts.Add(convertedModel);
                        }
                    }
                }

                model.PackingListParts = ConsolidatePalletParts(parts);
            }

            model.CustomerId = customerId;

            return View(model);
        }

        /// <summary>
        /// create paking list
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult CreatePackingList(PackingListViewModel model)
        {
            var operationResult = new OperationResult();

            PackingList newPackingList = new PackingListConverter().ConvertToDomain(model);

            operationResult = _packingListRepository.SavePackingList(newPackingList);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Warehouse/EditPackingList
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult EditPackingList(Guid packingListId)
        {
            var packingList = _packingListRepository.GetPackingList(packingListId);

            PackingListViewModel model = new PackingListConverter().ConvertToView(packingList);

            var carriers = _carrierRepository.GetSelectableCarriers();

            var defaultCarrier = new SelectListItem()
            {
                Text = "--Select Carrier--",
                Value = null
            };

            carriers.Insert(0, defaultCarrier);

            model.SelectableCarriers = carriers;

            model.SelectableCustomerAddresses = _customerAddressDynamicsRepository.GetSelectableCustomerAddresses(model.CustomerId);

            var parts = _partRepository.GetSelectableParts();

            model.SelectableParts = parts;

            model.SelectablePONumbers = new List<SelectListItem>();

            return View(model);
        }

        /// <summary>
        /// edit packing list
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditPackingList(PackingListViewModel model)
        {
            var result = new OperationResult();

            PackingList packingList = new PackingListConverter().ConvertToDomain(model);

            result = _packingListRepository.UpdatePackingList(packingList);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Warehouse/PackingListDetail
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult PackingListDetail(Guid packingListId)
        {
            var packingList = _packingListRepository.GetPackingList(packingListId);

            PackingListViewModel model = new PackingListConverter().ConvertToView(packingList);

            return View(model);
        }

        /// <summary>
        /// delete packing list
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeletePackingList(Guid packingListId)
        {
            var result = new OperationResult();

            result = _packingListRepository.DeletePackingList(packingListId);

            if (!result.Success)
            {
                this.AddNotification(result.Message, NotificationType.ERROR);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// close packing list
        /// </summary>
        /// <param name="packingListId"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult ClosePackingList(Guid packingListId)
        {
            var result = new OperationResult();

            result = _packingListRepository.ClosePackingList(packingListId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add packing list part modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddPackingListPart()
        {
            var model = new PackingListViewModel();

            var parts = _partRepository.GetSelectableParts();

            var defaultPart = new SelectListItem()
            {
                Text = "--Select Part--",
                Value = null
            };

            parts.Insert(0, defaultPart);

            model.SelectableParts = parts;

            return PartialView();
        }

        /// <summary>
        /// edit packing list modal
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditPackingListPart(Guid partId)
        {
            var model = new PackingListViewModel();

            model.SelectableParts = _partRepository.GetSelectableParts();

            model.SelectablePONumbers = _customerOrderRepository.GetSelectableCustomerOrdersByPart(partId);

            return PartialView(model);
        }

        /// <summary>
        /// get all packingt lists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAllPackingLists()
        {
            var model = new PackingListViewModel();

            var packingLists = new List<PackingListViewModel>();

            var tempPackingLists = _packingListRepository.GetPackingLists().ToList();

            if (tempPackingLists != null && tempPackingLists.Count > 0)
            {
                foreach (var tempPackingList in tempPackingLists)
                {
                    PackingListViewModel convertedModel = new PackingListConverter().ConvertToListView(tempPackingList);

                    packingLists.Add(convertedModel);
                }
            }

            model.PackingLists = packingLists.OrderBy(x => x.CustomerName).ThenByDescending(y => y.CreatedDate).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get open packingt lists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOpenPackingLists()
        {
            var model = new PackingListViewModel();

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

            model.PackingLists = packingLists.OrderBy(x => x.CustomerName).ThenByDescending(y => y.CreatedDate).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get closed packingt lists
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetClosedPackingLists()
        {
            var model = new PackingListViewModel();

            var packingLists = new List<PackingListViewModel>();

            var tempPackingLists = _packingListRepository.GetPackingLists().Where(x => x.IsClosed).ToList();

            if (tempPackingLists != null && tempPackingLists.Count > 0)
            {
                foreach (var tempPackingList in tempPackingLists)
                {
                    PackingListViewModel convertedModel = new PackingListConverter().ConvertToListView(tempPackingList);

                    packingLists.Add(convertedModel);
                }
            }

            model.PackingLists = packingLists.OrderBy(x => x.CustomerName).ThenByDescending(y => y.CreatedDate).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_packingListRepository != null)
                {
                    _packingListRepository.Dispose();
                    _packingListRepository = null;
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

                if (_carrierRepository != null)
                {
                    _carrierRepository.Dispose();
                    _carrierRepository = null;
                }

                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }

                if (_containerRepository != null)
                {
                    _containerRepository.Dispose();
                    _containerRepository = null;
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
            }

            base.Dispose(disposing);
        }
    }
}