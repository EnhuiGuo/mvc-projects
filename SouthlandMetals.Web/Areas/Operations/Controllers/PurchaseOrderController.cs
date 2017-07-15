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
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Controllers
{
    public class PurchaseOrderController : ApplicationBaseController
    {
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private ICustomerAddressDynamicsRepository _customerAddressDynamicsRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private IPriceSheetRepository _priceSheetRepository;
        private ISiteDynamicsRepository _siteDynamicsRepository;
        private IStateRepository _stateRepository;
        private IProjectRepository _projectRepository;
        private ICustomerOrderRepository _customerOrderRepository;
        private IFoundryOrderRepository _foundryOrderRepository;
        private IOrderTermRepository _orderTermRepository;
        private IProjectPartRepository _projectPartRepository;
        private IPurchaseOrderDynamicsRepository _purchaseOrderDynamicsRepository;

        public PurchaseOrderController()
        {
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _priceSheetRepository = new PriceSheetRepository();
            _siteDynamicsRepository = new SiteDynamicsRepository();
            _stateRepository = new StateRepository();
            _projectRepository = new ProjectRepository();
            _customerOrderRepository = new CustomerOrderRepository();
            _foundryOrderRepository = new FoundryOrderRepository();
            _orderTermRepository = new OrderTermRepository();
            _projectPartRepository = new ProjectPartRepository();
            _purchaseOrderDynamicsRepository = new PurchaseOrderDynamicsRepository();
        }

        public PurchaseOrderController(ICustomerDynamicsRepository customerDynamicsRepository,
                                       ICustomerAddressDynamicsRepository customerAddressDynamicsRepository,
                                       IFoundryDynamicsRepository foundryDynamicsRepository,
                                       IPriceSheetRepository priceSheetRepository,
                                       ISiteDynamicsRepository siteDynamicsRepository,
                                       IStateRepository stateRepository,
                                       IProjectRepository projectRepository,
                                       ICustomerOrderRepository customerOrderRepository,
                                       IFoundryOrderRepository foundryOrderRepository,
                                       IOrderTermRepository orderTermRepository,
                                       IProjectPartRepository projectPartRepository,
                                       IPurchaseOrderDynamicsRepository purchaseOrderDynamicsRepository)
        {
            _customerDynamicsRepository = customerDynamicsRepository;
            _customerAddressDynamicsRepository = customerAddressDynamicsRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _priceSheetRepository = priceSheetRepository;
            _siteDynamicsRepository = siteDynamicsRepository;
            _stateRepository = stateRepository;
            _projectRepository = projectRepository;
            _customerOrderRepository = customerOrderRepository;
            _foundryOrderRepository = foundryOrderRepository;
            _orderTermRepository = orderTermRepository;
            _projectPartRepository = projectPartRepository;
            _purchaseOrderDynamicsRepository = purchaseOrderDynamicsRepository;
        }

        private string FoundryOrderNumber()
        {
            var foundryOrderNumber = _foundryOrderRepository.FoundryOrderNumber();

            return foundryOrderNumber;
        }

        private string ShipCodeNumber()
        {
            var shipCodeNumber = _foundryOrderRepository.ShipCodeNumber();

            return shipCodeNumber;
        }

        private string ShipCodeNumber(string prefix, string suffix)
        {
            var shipCodeNumber = _foundryOrderRepository.ShipCodeNumber(prefix, suffix);

            return shipCodeNumber;
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/CustomerOrders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            var customerOrders = _customerOrderRepository.GetCustomerOrders();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrderModel = new CustomerOrderConverter().ConvertToView(customerOrder);

                    model.CustomerOrders.Add(customerOrderModel);
                }
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

            var orderTypes = new List<SelectListItem>();

            var orderType1 = new SelectListItem() { Text = "Sample", Value = "Sample" };
            orderTypes.Insert(0, orderType1);
            var orderType2 = new SelectListItem() { Text = "Tooling", Value = "Tooling" };
            orderTypes.Insert(0, orderType2);
            var orderType3 = new SelectListItem() { Text = "Production", Value = "Production" };
            orderTypes.Insert(0, orderType3);

            var defaultOrderType = new SelectListItem() { Text = "--Select Order Type--", Value = null };

            orderTypes.Insert(0, defaultOrderType);

            model.SelectableOrderTypes = orderTypes;

            return View(model);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/CreateCustomerOrder
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreateCustomerOrder()
        {
            var model = new CustomerOrderViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            var orderTypes = new List<SelectListItem>();

            var orderType1 = new SelectListItem() { Text = "Sample", Value = "Sample" };
            orderTypes.Insert(0, orderType1);
            var orderType2 = new SelectListItem() { Text = "Tooling", Value = "Tooling" };
            orderTypes.Insert(0, orderType2);
            var orderType3 = new SelectListItem() { Text = "Production", Value = "Production" };
            orderTypes.Insert(0, orderType3);

            var defaultOrderType = new SelectListItem() { Text = "--Select Order Type--", Value = null };

            orderTypes.Insert(0, defaultOrderType);

            model.SelectableOrderTypes = orderTypes;

            return View(model);
        }

        /// <summary>
        /// create customer order
        /// </summary>
        /// <param name="model"></param>
        /// <param name="customerOrderParts"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult CreateCustomerOrder(CustomerOrderViewModel model, List<CustomerOrderPartViewModel> customerOrderParts)
        {
            var operationResult = new OperationResult();

            model.CustomerOrderParts = customerOrderParts;

            CustomerOrder newCustomerOrder = new CustomerOrderConverter().ConvertToDomain(model);

            operationResult = _customerOrderRepository.SaveCustomerOrder(newCustomerOrder);

            if (operationResult.Success)
            {
                var customerOrderId = operationResult.ReferenceId;

                if (model.CustomerOrderParts != null && model.CustomerOrderParts.Count > 0)
                {
                    foreach (var customerOrderPart in model.CustomerOrderParts)
                    {
                        var priceSheetPart = _priceSheetRepository.GetPriceSheetPart(customerOrderPart.PriceSheetPartId);

                        if (priceSheetPart != null)
                        {
                            priceSheetPart.AvailableQuantity = priceSheetPart.AvailableQuantity - customerOrderPart.CustomerOrderQuantity;
                            _priceSheetRepository.UpdatePriceSheetPart(priceSheetPart);
                        }

                        var projectPart = _projectPartRepository.GetProjectPart(customerOrderPart.ProjectPartId);

                        if (projectPart != null)
                        {
                            projectPart.CustomerOrderId = operationResult.ReferenceId;
                            projectPart.CustomerAddressId = model.CustomerAddressId;
                            _projectPartRepository.UpdateProjectPart(projectPart);
                        }
                    }
                }

                operationResult.ReferenceId = customerOrderId;
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/CustomerOrderDetail
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CustomerOrderDetail(Guid customerOrderId)
        {
            var customerOrder = _customerOrderRepository.GetCustomerOrder(customerOrderId);

            CustomerOrderViewModel model = new CustomerOrderConverter().ConvertToView(customerOrder);

            return View(model);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/EditCustomerOrder
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult EditCustomerOrder(Guid customerOrderId)
        {
            var customerOrder = _customerOrderRepository.GetCustomerOrder(customerOrderId);

            CustomerOrderViewModel model = new CustomerOrderConverter().ConvertToView(customerOrder);

            if (customerOrderId != null)
            {
                var addresses = _customerAddressDynamicsRepository.GetSelectableCustomerAddresses(model.CustomerId);

                if (addresses != null)
                {
                    model.SelectableCustomerAddresses = addresses;
                }

                var sites = _siteDynamicsRepository.GetSelectableSites(model.CustomerId);

                if (sites != null)
                {
                    model.SelectableSites = sites;
                }
            }

            model.CurrentUser = User.Identity.Name;

            return View(model);
        }

        /// <summary>
        /// edit customer order
        /// </summary>
        /// <param name="model"></param>
        /// <param name="customerOrderParts"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditCustomerOrder(CustomerOrderViewModel model, List<CustomerOrderPartViewModel> customerOrderParts)
        {
            var operationResult = new OperationResult();

            model.CustomerOrderParts = customerOrderParts;

            CustomerOrder orderToUpdate = new CustomerOrderConverter().ConvertToDomain(model);

            operationResult = _customerOrderRepository.UpdateCustomerOrder(orderToUpdate);

            if (operationResult.Success)
            {
                model.Success = true;
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete customer order
        /// </summary>
        /// <param name="customerOrderId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteCustomerOrder(Guid customerOrderId)
        {
            var operationResult = new OperationResult();

            operationResult = _customerOrderRepository.DeleteCustomerOrder(customerOrderId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/foundryOrders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult FoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
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

            var orderTypes = new List<SelectListItem>();

            var orderType1 = new SelectListItem() { Text = "Sample", Value = "Sample" };
            orderTypes.Insert(0, orderType1);
            var orderType2 = new SelectListItem() { Text = "Tooling", Value = "Tooling" };
            orderTypes.Insert(0, orderType2);
            var orderType3 = new SelectListItem() { Text = "Production", Value = "Production" };
            orderTypes.Insert(0, orderType3);

            var defaultOrderType = new SelectListItem() { Text = "--Select Order Type--", Value = null };

            orderTypes.Insert(0, defaultOrderType);

            model.SelectableOrderTypes = orderTypes;

            return View(model);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/CreateFoundryOrder
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreateFoundryOrder()
        {
            var model = new FoundryOrderViewModel();

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            var orderTypes = new List<SelectListItem>();

            var orderType1 = new SelectListItem() { Text = "Sample", Value = "Sample" };
            orderTypes.Insert(0, orderType1);
            var orderType2 = new SelectListItem() { Text = "Tooling", Value = "Tooling" };
            orderTypes.Insert(0, orderType2);
            var orderType3 = new SelectListItem() { Text = "Production", Value = "Production" };
            orderTypes.Insert(0, orderType3);

            var defaultOrderType = new SelectListItem() { Text = "--Select Order Type--", Value = null };

            orderTypes.Insert(0, defaultOrderType);

            model.SelectableOrderTypes = orderTypes;

            model.OrderNumber = FoundryOrderNumber();

            _foundryOrderRepository.RemoveFoundryOrderNumber(model.OrderNumber);

            return View(model);
        }

        /// <summary>
        /// create foundry order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult CreateFoundryOrder(FoundryOrderViewModel model)
        {
            var operationResult = new OperationResult();

            FoundryOrder newFoundryOrder = new FoundryOrderConverter().ConvertToDomain(model);

            operationResult = _foundryOrderRepository.SaveFoundryOrder(newFoundryOrder);

            if (operationResult.Success && model.FoundryOrderParts != null && model.FoundryOrderParts.Count > 0)
            {
                var foundryOrderId = operationResult.ReferenceId;

                foreach (var part in model.FoundryOrderParts)
                {
                    var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart(part.CustomerOrderPartId);

                    if (customerOrderPart != null)
                    {
                        customerOrderPart.AvailableQuantity = customerOrderPart.AvailableQuantity - part.Quantity;
                    }

                    _customerOrderRepository.UpdateCustomerOrderPart(customerOrderPart);
                }

                operationResult.ReferenceId = foundryOrderId;
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/FoundryOrderDetail
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult FoundryOrderDetail(Guid foundryOrderId)
        {
            var foundryOrder = _foundryOrderRepository.GetFoundryOrder(foundryOrderId);

            FoundryOrderViewModel model = new FoundryOrderConverter().ConvertToView(foundryOrder);

            return View(model);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/EditFoundryOrder
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult EditFoundryOrder(Guid foundryOrderId)
        {
            var foundryOrder = _foundryOrderRepository.GetFoundryOrder(foundryOrderId);

            FoundryOrderViewModel model = new FoundryOrderConverter().ConvertToView(foundryOrder);

            model.SelectableSites = _siteDynamicsRepository.GetSelectableSites(foundryOrder.CustomerId);

            model.SelectableCustomerAddresses = _customerAddressDynamicsRepository.GetSelectableCustomerAddresses(foundryOrder.CustomerId);

            model.CurrentUser = User.Identity.Name;

            return View(model);
        }

        /// <summary>
        /// edit foundry order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditFoundryOrder(FoundryOrderViewModel model)
        {
            var operationResult = new OperationResult();

            FoundryOrder foundryOrder = new FoundryOrderConverter().ConvertToDomain(model);

            operationResult = _foundryOrderRepository.UpdateFoundryOrder(foundryOrder);

            if (operationResult.Success)
            {
                model.Success = true;
            }
            else
            {
                model.Success = false;
                model.Message = operationResult.Message;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete foundry order
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteFoundryOrder(Guid foundryOrderId)
        {
            var operationResult = new OperationResult();

            operationResult = _foundryOrderRepository.DeleteFoundryOrder(foundryOrderId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/PurchaseOrder/Scheduling
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Scheduling()
        {
            return View();
        }

        /// <summary>
        /// import schedule from file
        /// </summary>
        /// <param name="schedulingFile"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult ImportSchedule(HttpPostedFileBase schedulingFile)
        {
            var model = new FoundryOrderViewModel();

            var operationResult = new OperationResult();

            model.FoundryOrderDifferents = new List<FoundryOrderViewModel>();

            if (schedulingFile == null || schedulingFile.ContentLength == 0)
            {
                model.Success = false;
                model.Message = "Error occurred...scheduling file not found or contains no data.";

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (schedulingFile.FileName.EndsWith(".xls") || schedulingFile.FileName.EndsWith(".xlsx") ||
                    schedulingFile.FileName.EndsWith(".csv"))
                {
                    DataTable dataTable = new CSVManager().ProcessCSV(schedulingFile);

                    var foundryOrderParts = new List<FoundryOrderPartViewModel>();

                    var _db = new SouthlandDb();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        //var newFoundryOrderPart = new FoundryOrderPartViewModel()
                        //{
                        //    PartNumber = row["Part"].ToString(),
                        //    OrderNumber = row["PONumber"].ToString(),
                        //    Quantity = int.Parse(row["Quantity"].ToString()),
                        //    ShipCodePrefix = row["ShipCodePrefix"].ToString(),
                        //    ShipCodeSuffix = row["ShipCodeSuffix"].ToString(),
                        //    ShipDate = DateTime.Parse(row["ShipDate"].ToString()),
                        //    EstArrivalDate = DateTime.Parse(row["EstArrivalDate"].ToString()),
                        //    IsScheduled = true
                        //};

                        //foundryOrderParts.Add(newFoundryOrderPart);
                    }

                    if (foundryOrderParts != null && foundryOrderParts.Count > 0)
                    {
                        //foreach (var foundryOrderPart in foundryOrderParts)
                        //{
                        //    //generate new ship code
                        //    var shipCodeNumber = ShipCodeNumber(foundryOrderPart.ShipCodePrefix, foundryOrderPart.ShipCodeSuffix);

                        //    _foundryOrderRepository.RemoveShipCodeNumber(shipCodeNumber);

                        //    var part = _db.Part.FirstOrDefault(x => x.Number == foundryOrderPart.PartNumber);
                        //    var fOrder = _db.FoundryOrder.FirstOrDefault(x => x.Number == foundryOrderPart.PONumber);
                        //    var foPart = _db.FoundryOrderPart.FirstOrDefault(x => x.PartId == part.PartId && x.FoundryOrderId == fOrder.FoundryOrderId);

                        //    var newFOPart = new FoundryOrderPartViewModel()
                        //    {
                        //        FoundryOrderId = foPart.FoundryOrderId,
                        //        CustomerOrderPartId = foPart.CustomerOrderPartId,
                        //        ProjectPartId = foPart.ProjectPartId,
                        //        PartId = foPart.PartId,
                        //        AvailableQuantity = foundryOrderPart.Quantity,
                        //        Quantity = foundryOrderPart.Quantity,
                        //        EstArrivalDate = foundryOrderPart.EstArrivalDate,
                        //        ShipDate = foundryOrderPart.ShipDate,
                        //        //ShipCode = foundryOrderPart.ShipCode,
                        //        ShipCode = foundryOrderPart.ShipCodePrefix + shipCodeNumber + foundryOrderPart.ShipCodeSuffix,
                        //        ShipCodeNotes = null,
                        //        Price = foPart.Price,
                        //        Cost = foPart.Cost,
                        //        IsScheduled = foundryOrderPart.IsScheduled,
                        //        ScheduledDate = DateTime.Now,
                        //        HasBeenReceived = false,
                        //        ReceiptDate = null,
                        //        CreatedDate = foPart.CreatedDate,
                        //        CreatedBy = foPart.CreatedBy,
                        //        ModifiedDate = foPart.ModifiedDate,
                        //        ModifiedBy = foPart.ModifiedBy
                        //    };

                        //    FoundryOrderPart convertedModel = new FoundryOrderPartConverter().ConvertToDomain(foundryOrderPart);

                        //    operationResult = _foundryOrderRepository.SaveFoundryOrderPart(convertedModel);

                        //    if (operationResult.Success)
                        //    {
                        //        operationResult = _foundryOrderRepository.DeleteFoundryOrderPart(foPart.FoundryOrderPartId);
                        //    }
                        //}
                    }

                    if (operationResult.Success)
                    {
                        model.Success = true;
                    }
                    else
                    {
                        model.Success = false;
                        model.Message = operationResult.Message;
                    }
                }
                else
                {
                    model.Success = false;
                    model.Message = "Error occurred...scheduling file not in correct format.";
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// add price sheet parts modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddPriceSheetParts()
        {
            return PartialView();
        }

        /// <summary>
        /// add customer order parts modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddCustomerOrderParts()
        {
            return PartialView();
        }

        /// <summary>
        /// view ship codes modal
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewShipCodes(Guid foundryOrderId)
        {
            var model = new FoundryOrderViewModel();

            var shipCodes = _foundryOrderRepository.GetFoundryOrderParts().Where(x => x.FoundryOrderId == foundryOrderId).Select(x => x.ShipCode).ToList();

            shipCodes = shipCodes.Distinct().ToList();

            for (var i = 0; i < shipCodes.Count(); i++)
            {
                if (i == 0)
                {
                    model.ShipCodeList += shipCodes[i];
                }
                else
                {
                    model.ShipCodeList += $", {shipCodes[i]}";
                }
            }

            if (string.IsNullOrEmpty(model.ShipCodeList))
            {
                model.ShipCodeList = "N/A";
            }

            return PartialView(model);
        }

        /// <summary>
        /// edit ship code and ship code notes modal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditShipCodeAndNotes(ShipCodeViewModel model)
        {
            return PartialView(model);
        }

        /// <summary>
        /// print open customer orders modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _PrintOpenCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

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

            var orderTypes = new List<SelectListItem>();

            var orderType1 = new SelectListItem() { Text = "Sample", Value = "Sample" };
            orderTypes.Insert(0, orderType1);
            var orderType2 = new SelectListItem() { Text = "Tooling", Value = "Tooling" };
            orderTypes.Insert(0, orderType2);
            var orderType3 = new SelectListItem() { Text = "Production", Value = "Production" };
            orderTypes.Insert(0, orderType3);

            var defaultOrderType = new SelectListItem() { Text = "--Select Order Type--", Value = null };

            orderTypes.Insert(0, defaultOrderType);

            model.SelectableOrderTypes = orderTypes;

            return PartialView(model);
        }

        /// <summary>
        /// print unattached customer orders modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _PrintUnattachedCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

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

            var orderTypes = new List<SelectListItem>();

            var orderType1 = new SelectListItem() { Text = "Sample", Value = "Sample" };
            orderTypes.Insert(0, orderType1);
            var orderType2 = new SelectListItem() { Text = "Tooling", Value = "Tooling" };
            orderTypes.Insert(0, orderType2);
            var orderType3 = new SelectListItem() { Text = "Production", Value = "Production" };
            orderTypes.Insert(0, orderType3);

            var defaultOrderType = new SelectListItem() { Text = "--Select Order Type--", Value = null };

            orderTypes.Insert(0, defaultOrderType);

            model.SelectableOrderTypes = orderTypes;

            return PartialView(model);
        }

        /// <summary>
        /// print open foundry orders modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _PrintOpenFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

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

            var orderTypes = new List<SelectListItem>();

            var orderType1 = new SelectListItem() { Text = "Sample", Value = "Sample" };
            orderTypes.Insert(0, orderType1);
            var orderType2 = new SelectListItem() { Text = "Tooling", Value = "Tooling" };
            orderTypes.Insert(0, orderType2);
            var orderType3 = new SelectListItem() { Text = "Production", Value = "Production" };
            orderTypes.Insert(0, orderType3);

            var defaultOrderType = new SelectListItem() { Text = "--Select Order Type--", Value = null };

            orderTypes.Insert(0, defaultOrderType);

            model.SelectableOrderTypes = orderTypes;

            return PartialView(model);
        }

        /// <summary>
        /// get new ship code number
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public string newShipCodeNumber()
        {
            var newShipCodeNumber = ShipCodeNumber();

            _foundryOrderRepository.RemoveShipCodeNumber(newShipCodeNumber);

            return newShipCodeNumber;
        }

        /// <summary>
        /// get all customer orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAllCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get open customer orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOpenCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => x.IsOpen).ToList();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get hold customer orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetHoldCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => x.IsHold).ToList();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get canceled customer orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCanceledCsutomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => x.IsCanceled).ToList();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get complete customer orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCompleteCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => x.IsComplete).ToList();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get sample customer orders 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSampleCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            model.CustomerOrders = model.CustomerOrders.Where(x => x.IsSample).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get tooling customer orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetToolingCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            model.CustomerOrders = model.CustomerOrders.Where(x => x.IsTooling).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get production customer orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetProductionCustomerOrders()
        {
            var model = new CustomerOrderViewModel();

            model.CustomerOrders = new List<CustomerOrderViewModel>();

            var customerOrders = _customerOrderRepository.GetCustomerOrders();

            if (customerOrders != null && customerOrders.Count > 0)
            {
                foreach (var customerOrder in customerOrders)
                {
                    CustomerOrderViewModel customerOrdeModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    model.CustomerOrders.Add(customerOrdeModel);
                }
            }

            model.CustomerOrders = model.CustomerOrders.Where(x => x.IsProduction).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get all foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAllFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get open foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOpenFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders().Where(x => x.IsOpen).ToList();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get hold foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetHoldFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders().Where(x => x.IsHold).ToList();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get complete foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCompleteFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders().Where(x => x.IsComplete).ToList();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get canceled foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetCanceledFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders().Where(x => x.IsCanceled).ToList();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get sample foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSampleFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            model.FoundryOrders = model.FoundryOrders.Where(x => x.IsSample).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get tooling foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetToolingFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            model.FoundryOrders = model.FoundryOrders.Where(x => x.IsTooling).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get production foundry orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetProductionFoundryOrders()
        {
            var model = new FoundryOrderViewModel();

            model.FoundryOrders = new List<FoundryOrderViewModel>();

            var foundryOrders = _foundryOrderRepository.GetFoundryOrders();

            if (foundryOrders != null && foundryOrders.Count > 0)
            {
                foreach (var foundryOrder in foundryOrders)
                {
                    FoundryOrderViewModel foundryOrderModel = new FoundryOrderConverter().ConvertToView(foundryOrder);
                    model.FoundryOrders.Add(foundryOrderModel);
                }
            }

            model.FoundryOrders = model.FoundryOrders.Where(x => x.IsProduction).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get customer order parts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPartsByCustomerOrder(Guid customerOrderId)
        {
            var model = new CustomerOrderViewModel();
            var parts = new List<CustomerOrderPartViewModel>();

            var customerOrder = _customerOrderRepository.GetCustomerOrder(customerOrderId);

            var customerOrderParts = _customerOrderRepository.GetCustomerOrderParts().Where(x => x.CustomerOrderId == customerOrderId && x.AvailableQuantity > 0);

            foreach (var customerOrderPart in customerOrderParts)
            {
                var customerOrderPartModel = new CustomerOrderPartViewModel();

                if (customerOrder.IsSample || customerOrder.IsTooling)
                {
                    customerOrderPartModel = new CustomerOrderPartConverter().ConvertToProjectPartView(customerOrderPart);
                }
                else
                {
                    customerOrderPartModel = new CustomerOrderPartConverter().ConvertToPartView(customerOrderPart);
                }

                parts.Add(customerOrderPartModel);
            }

            model.CustomerOrderParts = parts.OrderByDescending(y => y.EstArrivalDate).ThenBy(y => y.PartNumber).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get foundry order parts
        /// </summary>
        /// <param name="foundryOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPartsByFoundryOrder(Guid foundryOrderId)
        {
            var foundryOrderParts = new List<FoundryOrderPartViewModel>();

            var tempParts = _foundryOrderRepository.GetFoundryOrderParts().Where(x => x.FoundryOrderId == foundryOrderId).ToList();

            if (tempParts != null && tempParts.Count() > 0)
            {
                foreach (var tempPart in tempParts)
                {
                    FoundryOrderPartViewModel convertedModel = new FoundryOrderPartConverter().ConvertToPartView(tempPart);

                    foundryOrderParts.Add(convertedModel);
                }
            }

            return Json(foundryOrderParts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get customer orders by project with a filter
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOrdersByProject(Guid projectId, DateTime? fromDate, DateTime? toDate, string orderType)
        {
            var model = new CustomerOrderViewModel();

            var customerOrders = new List<CustomerOrderViewModel>();

            var tempCustomerOrders = _customerOrderRepository.GetCustomerOrders().Where(x => x.IsOpen && x.ProjectId == projectId).ToList();

            if (tempCustomerOrders != null && tempCustomerOrders.Count > 0)
            {
                foreach (var customerOrder in tempCustomerOrders)
                {
                    CustomerOrderViewModel customerOrderModel = new CustomerOrderConverter().ConvertToView(customerOrder);
                    customerOrders.Add(customerOrderModel);
                }
            }

            if (orderType.Equals("Sample"))
            {
                customerOrders = customerOrders.Where(x => x.IsSample).ToList();
            }
            else if (orderType.Equals("Tooling"))
            {
                customerOrders = customerOrders.Where(x => x.IsTooling).ToList();
            }
            else if (orderType.Equals("Production"))
            {
                customerOrders = customerOrders.Where(x => x.IsProduction).ToList();
            }

            if (fromDate != null)
            {
                customerOrders = customerOrders.Where(x => x.DueDate >= fromDate).ToList();
            }

            if (toDate != null)
            {
                customerOrders = customerOrders.Where(x => x.DueDate <= toDate).ToList();
            }

            model.CustomerOrders = customerOrders;

            return Json(model.CustomerOrders, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get price sheet parts
        /// </summary>
        /// <param name="priceSheetId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPartsByPriceSheet(Guid priceSheetId)
        {
            var model = new PriceSheetPartViewModel();

            var parts = new List<PriceSheetPartViewModel>();

            var priceSheet = _priceSheetRepository.GetPriceSheet(priceSheetId);

            if (priceSheet.IsQuote)
            {
                var priceSheetParts = _priceSheetRepository.GetPriceSheetParts(priceSheetId).Where(x => x.AvailableQuantity > 0).ToList();

                if (priceSheetParts != null && priceSheetParts.Count > 0)
                {
                    foreach (var priceSheetPart in priceSheetParts)
                    {
                        var priceSheetPartModel = new PriceSheetPartConverter().ConvertToProjectPartView(priceSheetPart);
                        parts.Add(priceSheetPartModel);
                    }
                }

                parts = parts.OrderBy(y => y.PartNumber).ToList();
            }
            else
            {
                var priceSheetParts = _priceSheetRepository.GetPriceSheetParts(priceSheetId).Where(x => x.AvailableQuantity > 0).ToList();

                if (priceSheetParts != null && priceSheetParts.Count > 0)
                {
                    foreach (var priceSheetPart in priceSheetParts)
                    {
                        var priceSheetPartModel = new PriceSheetPartConverter().ConvertToPartView(priceSheetPart);
                        parts.Add(priceSheetPartModel);
                    }
                }

                parts = parts.OrderBy(y => y.PartNumber).ToList();
            }

            model.OrderParts = parts;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get selectable customer orders by part
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSelectableCustomerOrderByPart(Guid partId)
        {
            var selectablePONumbers = _customerOrderRepository.GetSelectableCustomerOrdersByPart(partId);

            return Json(selectablePONumbers, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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

                if (_priceSheetRepository != null)
                {
                    _priceSheetRepository.Dispose();
                    _priceSheetRepository = null;
                }

                if (_siteDynamicsRepository != null)
                {
                    _siteDynamicsRepository.Dispose();
                    _siteDynamicsRepository = null;
                }

                if (_stateRepository != null)
                {
                    _stateRepository.Dispose();
                    _stateRepository = null;
                }

                if (_projectRepository != null)
                {
                    _projectRepository.Dispose();
                    _projectRepository = null;
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

                if (_orderTermRepository != null)
                {
                    _orderTermRepository.Dispose();
                    _orderTermRepository = null;
                }

                if (_projectPartRepository != null)
                {
                    _projectPartRepository.Dispose();
                    _projectPartRepository = null;
                }

                if (_purchaseOrderDynamicsRepository != null)
                {
                    _purchaseOrderDynamicsRepository.Dispose();
                    _purchaseOrderDynamicsRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}