using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Interfaces;
using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Reporting.Domain.Interfaces;
using SouthlandMetals.Reporting.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using SouthlandMetals.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Controllers
{
    public class ShipmentController : ApplicationBaseController
    {
        private ICustomerDynamicsRepository _customerDynamicsRepository;
        private ISalespersonDynamicsRepository _salespersonDynamicsRepository;
        private IPartRepository _partRepository;
        private IFoundryDynamicsRepository _foundryDynamicsRepository;
        private ICarrierRepository _carrierRepository;
        private IVesselRepository _vesselRepository;
        private IPortRepository _portRepository;
        private IShipmentRepository _shipmentRepository;
        private IContainerRepository _containerRepository;
        private IBillOfLadingRepository _bolRepository;
        private IFoundryInvoiceRepository _foundryInvoiceRepository;
        private IFoundryOrderRepository _foundryOrderRepository;
        private IBucketRepository _buckRepository;
        private IAccountCodeRepository _accountCodeRepository;
        private IDebitMemoRepository _debitMemoRepository;
        private ICreditMemoRepository _creditMemoRepository;
        private IPayablesDynamicsRepository _payablesDynamicsRepository;
        private IReceivablesDynamicsRepository _receivablesDynamicsRepository;
        private ICustomerOrderRepository _customerOrderRepository;
        private IReceiptDynamicsRepository _receiptDynamicsRepository;
        private IReportRepository _reportRepository;

        public ShipmentController()
        {
            _customerDynamicsRepository = new CustomerDynamicsRepository();
            _salespersonDynamicsRepository = new SalespersonDynamicsRepository();
            _partRepository = new PartRepository();
            _foundryDynamicsRepository = new FoundryDynamicsRepository();
            _carrierRepository = new CarrierRepository();
            _vesselRepository = new VesselRepository();
            _portRepository = new PortRepository();
            _shipmentRepository = new ShipmentRepository();
            _containerRepository = new ContainerRepository();
            _bolRepository = new BillOfLadingRepository();
            _foundryInvoiceRepository = new FoundryInvoiceRepository();
            _foundryOrderRepository = new FoundryOrderRepository();
            _buckRepository = new BucketRepository();
            _accountCodeRepository = new AccountCodeRepository();
            _debitMemoRepository = new DebitMemoRepository();
            _creditMemoRepository = new CreditMemoRepository();
            _payablesDynamicsRepository = new PayablesDynamicsRepository();
            _receivablesDynamicsRepository = new ReceivablesDynamicsRepository();
            _customerOrderRepository = new CustomerOrderRepository();
            _receiptDynamicsRepository = new ReceiptDynamicsRepository();
            _reportRepository = new ReportRepository();
        }

        public ShipmentController(ICustomerDynamicsRepository customerDynamicsRepository,
                                  ISalespersonDynamicsRepository salespersonDynamicsRepository,
                                  IPartRepository partRepository,
                                  IFoundryDynamicsRepository foundryDynamicsRepository,
                                  ICarrierRepository carrierRepository,
                                  IVesselRepository vesselRepository,
                                  IPortRepository portRepository,
                                  IShipmentRepository shipmentRepository,
                                  IContainerRepository containerRepository,
                                  IBillOfLadingRepository bolRepository,
                                  IFoundryInvoiceRepository foundryInvoiceRepository,
                                  IFoundryOrderRepository foundryOrderRepository,
                                  IBucketRepository buckRepository,
                                  IAccountCodeRepository accountCodeRepository,
                                  IDebitMemoRepository debitMemoRepository,
                                  ICreditMemoRepository creditMemoRepository,
                                  IPayablesDynamicsRepository payablesDynamicsRepository,
                                  IReceivablesDynamicsRepository receivablesDynamicsRepository,
                                  ICustomerOrderRepository customerOrderRepository,
                                  IReceiptDynamicsRepository receiptDynamicsRepository,
                                  IReportRepository reportRepository)
        {
            _customerDynamicsRepository = customerDynamicsRepository;
            _salespersonDynamicsRepository = salespersonDynamicsRepository;
            _partRepository = partRepository;
            _foundryDynamicsRepository = foundryDynamicsRepository;
            _carrierRepository = carrierRepository;
            _vesselRepository = vesselRepository;
            _portRepository = portRepository;
            _shipmentRepository = shipmentRepository;
            _containerRepository = containerRepository;
            _bolRepository = bolRepository;
            _foundryInvoiceRepository = foundryInvoiceRepository;
            _foundryOrderRepository = foundryOrderRepository;
            _buckRepository = buckRepository;
            _accountCodeRepository = accountCodeRepository;
            _debitMemoRepository = debitMemoRepository;
            _creditMemoRepository = creditMemoRepository;
            _payablesDynamicsRepository = payablesDynamicsRepository;
            _receivablesDynamicsRepository = receivablesDynamicsRepository;
            _customerOrderRepository = customerOrderRepository;
            _receiptDynamicsRepository = receiptDynamicsRepository;
            _reportRepository = reportRepository;
        }

        private string DebitMemoNumber()
        {
            var debitMemoNumber = _debitMemoRepository.DebitMemoNumber();
            return debitMemoNumber;
        }

        private List<BillOfLadingViewModel> GetBillsOfLadingPendingPayment()
        {
            var billsOfLading = new List<BillOfLadingViewModel>();

            var tempBillsOfLading = _bolRepository.GetBillOfLadings().ToList();

            if (tempBillsOfLading != null && tempBillsOfLading.Count > 0)
            {
                foreach (var tempBillOfLading in tempBillsOfLading)
                {
                    var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoiceByBillOfLading(tempBillOfLading.BillOfLadingId);

                    if (foundryInvoice.ScheduledPaymentDate == null)
                    {
                        BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToListView(tempBillOfLading);

                        billsOfLading.Add(convertedModel);
                    }
                }
            }

            return billsOfLading;
        }

        private List<BillOfLadingViewModel> GetBillsOfLadingPendingAnalysis()
        {
            var billsOfLading = new List<BillOfLadingViewModel>();

            var tempBillsOfLading = _bolRepository.GetBillOfLadings().Where(x => x.HasBeenAnalyzed == false).ToList();

            if (tempBillsOfLading != null && tempBillsOfLading.Count > 0)
            {
                foreach (var tempBillOfLading in tempBillsOfLading)
                {
                    BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToListView(tempBillOfLading);

                    billsOfLading.Add(convertedModel);
                }
            }

            return billsOfLading;
        }

        private List<BillOfLadingViewModel> GetBillsOfWithoutArrivalNotices()
        {
            var billsOfLading = new List<BillOfLadingViewModel>();

            var tempBillsOfLading = _bolRepository.GetBillOfLadings().Where(x => x.HasArrivalNotice == false).ToList();

            if (tempBillsOfLading != null && tempBillsOfLading.Count > 0)
            {
                foreach (var tempBillOfLading in tempBillsOfLading)
                {
                    BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToListView(tempBillOfLading);

                    billsOfLading.Add(convertedModel);
                }
            }

            return billsOfLading;
        }

        private List<BillOfLadingViewModel> GetBillsOfLadingWithoutCustomsNumbers()
        {
            var billsOfLading = new List<BillOfLadingViewModel>();

            var tempBillsOfLading = _bolRepository.GetBillOfLadings().Where(x => x.CustomsNumber.Equals(string.Empty) || x.CustomsNumber == null).ToList();

            if (tempBillsOfLading != null && tempBillsOfLading.Count > 0)
            {
                foreach (var tempBillOfLading in tempBillsOfLading)
                {
                    BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToListView(tempBillOfLading);

                    billsOfLading.Add(convertedModel);
                }
            }

            return billsOfLading;
        }

        private List<BillOfLadingViewModel> GetBillsOfLadingWithoutOriginalDocuments()
        {
            var billsOfLading = new List<BillOfLadingViewModel>();

            var tempBillsOfLading = _bolRepository.GetBillOfLadings().Where(x => x.HasOriginalDocuments == false).ToList();

            if (tempBillsOfLading != null && tempBillsOfLading.Count > 0)
            {
                foreach (var tempBillOfLading in tempBillsOfLading)
                {
                    BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToListView(tempBillOfLading);

                    billsOfLading.Add(convertedModel);
                }
            }

            return billsOfLading;
        }

        private List<BillOfLadingViewModel> GetBillsOfLadingWithoutWireInstructions()
        {
            var billsOfLading = new List<BillOfLadingViewModel>();

            var tempBillsOfLading = _bolRepository.GetBillOfLadings().Where(x => x.WireInstructions == null).ToList();

            if (tempBillsOfLading != null && tempBillsOfLading.Count > 0)
            {
                foreach (var tempBillOfLading in tempBillsOfLading)
                {
                    BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToListView(tempBillOfLading);

                    billsOfLading.Add(convertedModel);
                }
            }

            return billsOfLading;
        }

        /// <summary>
        /// GET: Operations/Shipment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Tracking()
        {
            var model = new ShipmentViewModel();

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

            model.SelectableCarriers = _carrierRepository.GetSelectableCarriers();

            var defaultCarrier = new SelectListItem()
            {
                Text = "--Select Carrier--",
                Value = null
            };

            model.SelectableCarriers.Insert(0, defaultCarrier);

            model.SelectableVessels = _vesselRepository.GetSelectableVessels();

            var defaultVessel = new SelectListItem()
            {
                Text = "--Select Vessel--",
                Value = null
            };

            model.SelectableVessels.Insert(0, defaultVessel);

            model.SelectablePorts = _portRepository.GetSelectablePorts();

            var defaultPort = new SelectListItem()
            {
                Text = "--Select Port--",
                Value = null
            };

            model.SelectablePorts.Insert(0, defaultPort);

            var tempShipments = _shipmentRepository.GetShipments().Where(x => !x.IsComplete).ToList();

            var shipments = new List<ShipmentViewModel>();

            if (tempShipments != null && tempShipments.Count > 0)
            {
                foreach (var shipment in tempShipments)
                {
                    ShipmentViewModel convertedModel = new ShipmentConverter().ConvertToView(shipment);

                    shipments.Add(convertedModel);
                }
            }

            model.Shipments = shipments.OrderBy(x => x.DepartureDateStr).ToList();

            model.ReceiptDateStr = DateTime.Now.ToShortDateString();

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Shipment/SearchShipments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SearchShipments()
        {
            var shipments = new List<ShipmentViewModel>();

            var tempShipments = _shipmentRepository.GetShipments();

            if (tempShipments != null && tempShipments.Count > 0)
            {
                foreach (var tempShipment in tempShipments)
                {
                    ShipmentViewModel convertedModel = new ShipmentConverter().ConvertToView(tempShipment);

                    shipments.Add(convertedModel);
                }
            }

            return Json(shipments, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add shipment modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddShipment()
        {
            var model = new ShipmentViewModel();

            model.SelectableCarriers = _carrierRepository.GetSelectableCarriers();

            var defaultCarrier = new SelectListItem()
            {
                Text = "--Select Carrier--",
                Value = null
            };

            model.SelectableCarriers.Insert(0, defaultCarrier);

            model.SelectableVessels = _vesselRepository.GetSelectableVessels();

            var defaultVessel = new SelectListItem()
            {
                Text = "--Select Vessel--",
                Value = null
            };

            model.SelectableVessels.Insert(0, defaultVessel);

            model.SelectablePorts = _portRepository.GetSelectablePorts();

            var defaultPort = new SelectListItem()
            {
                Text = "--Select Port--",
                Value = null
            };

            model.SelectablePorts.Insert(0, defaultPort);

            return PartialView(model);
        }

        /// <summary>
        /// add shipment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult AddShipment(ShipmentViewModel model)
        {
            var operationResult = new OperationResult();

            Shipment newShipment = new ShipmentConverter().ConvertToDomain(model);

            operationResult = _shipmentRepository.SaveShipment(newShipment);

            var tempShipments = _shipmentRepository.GetShipments();

            var shipments = new List<ShipmentViewModel>();

            if (tempShipments != null && tempShipments.Count > 0)
            {
                foreach (var shipment in tempShipments)
                {
                    ShipmentViewModel convertedModel = new ShipmentConverter().ConvertToView(shipment);

                    shipments.Add(convertedModel);
                }
            }

            model.Shipments = shipments.OrderBy(x => x.DepartureDateStr).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// edit shipment modal
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditShipment(Guid shipmentId)
        {
            var shipment = _shipmentRepository.GetShipment(shipmentId);

            ShipmentViewModel model = new ShipmentConverter().ConvertToView(shipment);

            model.SelectableCarriers = _carrierRepository.GetSelectableCarriers();

            model.SelectableVessels = _vesselRepository.GetSelectableVessels();

            model.SelectablePorts = _portRepository.GetSelectablePorts();

            return PartialView(model);
        }

        /// <summary>
        /// edit shipment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditShipment(ShipmentViewModel model)
        {
            var operationResult = new OperationResult();

            Shipment shipmentToUpdate = new ShipmentConverter().ConvertToDomain(model);

            operationResult = _shipmentRepository.UpdateShipment(shipmentToUpdate);

            var tempShipments = _shipmentRepository.GetShipments();

            var shipments = new List<ShipmentViewModel>();

            if (tempShipments != null && tempShipments.Count > 0)
            {
                foreach (var shipment in tempShipments)
                {
                    ShipmentViewModel convertedModel = new ShipmentConverter().ConvertToView(shipment);

                    shipments.Add(convertedModel);
                }
            }

            model.Shipments = shipments.OrderBy(x => x.DepartureDateStr).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete shipment
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteShipment(Guid shipmentId)
        {
            var operationResult = new OperationResult();

            operationResult = _shipmentRepository.DeleteShipment(shipmentId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// receive shipment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult ReceiveShipment(ShipmentViewModel model)
        {
            var operationResult = new OperationResult();

            if (model.PurchaseOrders != null && model.PurchaseOrders.Count > 0)
            {
                foreach (var order in model.PurchaseOrders)
                {
                    var orderToUpdate = _foundryOrderRepository.GetFoundryOrder(order.OrderNumber);

                    orderToUpdate.IsComplete = true;

                    var partsReceived = _foundryOrderRepository.GetFoundryOrderParts().Where(x => x.FoundryOrderId == orderToUpdate.FoundryOrderId).ToList();

                    if (partsReceived != null && partsReceived.Count > 0)
                    {
                        foreach (var partReceived in partsReceived)
                        {
                            partReceived.EstArrivalDate = DateTime.Now;
                            partReceived.HasBeenReceived = true;
                            partReceived.ReceiptDate = DateTime.Now;
                            partReceived.ReceiptQuantity = partReceived.Quantity;

                            operationResult = _foundryOrderRepository.UpdateFoundryOrderPart(partReceived);

                            if (operationResult.Success)
                            {
                                var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart(partReceived.CustomerOrderPartId);

                                customerOrderPart.EstArrivalDate = DateTime.Now;

                                operationResult = _customerOrderRepository.UpdateCustomerOrderPart(customerOrderPart);
                            }
                        }
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Shipment/CreateBillofLading
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreateBillofLading(Guid shipmentId)
        {
            var model = new BillOfLadingViewModel();

            model.ShipmentId = shipmentId;

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            model.SelectableContainers = _containerRepository.GetSelectableContainers();

            model.SelectablePurchaseOrders = _foundryOrderRepository.GetSelectableFoundryOrders();

            return View(model);
        }

        /// <summary>
        /// create bill of lading
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult CreateBillofLading(BillOfLadingViewModel model)
        {
            var operationResult = new OperationResult();

            if (model.Containers != null && model.Containers.Count > 0)
            {
                foreach (var container in model.Containers)
                {
                    if (model.ContainerParts != null && model.ContainerParts.Count > 0)
                    {
                        container.ContainerParts = model.ContainerParts.Where(x => x.ContainerNumber == container.ContainerNumber).ToList();
                    }
                }
            }

            BillOfLading newBol = new BillOfLadingConverter().ConvertToDomain(model);

            operationResult = _bolRepository.SaveBillOfLading(newBol);

            if (model.ContainerParts != null && model.ContainerParts.Count > 0)
            {
                foreach (var containerPart in model.ContainerParts)
                {
                    var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPart(containerPart.FoundryOrderPartId);
                    if (foundryOrderPart != null)
                    {
                        foundryOrderPart.AvailableQuantity = foundryOrderPart.AvailableQuantity - containerPart.Quantity;
                        operationResult = _foundryOrderRepository.UpdateFoundryOrderPart(foundryOrderPart);
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Shipment/BillofladingDetail
        /// </summary>
        /// <param name="bolId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult BillofLadingDetail(Guid bolId)
        {
            var selectedBol = _bolRepository.GetBillOfLading(bolId);

            BillOfLadingViewModel model = new BillOfLadingConverter().ConvertToView(selectedBol);

            model.SelectableContainers = _containerRepository.GetContainers().Where(x => x.BillOfLadingId == selectedBol.BillOfLadingId)
                                                   .Select(y => new SelectListItem()
                                                   {
                                                       Text = y.Number,
                                                       Value = y.ContainerId.ToString()
                                                   }).ToList();

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Shipment/EditBillOfLading
        /// </summary>
        /// <param name="bolId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult EditBillofLading(Guid bolId)
        {
            var billOfLading = _bolRepository.GetBillOfLading(bolId);

            BillOfLadingViewModel model = new BillOfLadingConverter().ConvertToView(billOfLading);

            model.SelectableContainers = _containerRepository.GetContainers().Where(x => x.BillOfLadingId == billOfLading.BillOfLadingId)
                                            .Select(y => new SelectListItem()
                                            {
                                                Text = y.Number,
                                                Value = y.ContainerId.ToString()
                                            }).ToList();

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            model.SelectablePurchaseOrders = _foundryOrderRepository.GetSelectableFoundryOrders();

            return View(model);
        }

        /// <summary>
        /// edit bill of lading
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditBillofLading(BillOfLadingViewModel model)
        {
            var operationResult = new OperationResult();

            if (model.Containers != null && model.Containers.Count > 0)
            {
                foreach (var container in model.Containers)
                {
                    if (model.ContainerParts != null && model.ContainerParts.Count > 0)
                    {
                        container.ContainerParts = model.ContainerParts.Where(x => x.ContainerNumber == container.ContainerNumber).ToList();
                    }
                }
            }

            BillOfLading billOfLading = new BillOfLadingConverter().ConvertToDomain(model);

            operationResult = _bolRepository.UpdateBillOfLading(billOfLading);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete bill of lading
        /// </summary>
        /// <param name="billOfLadingId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteBillOfLading(Guid billOfLadingId)
        {
            var operationResult = new OperationResult();

            operationResult = _bolRepository.DeleteBillOfLading(billOfLadingId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Shipment/Progress
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult Progress()
        {
            var model = new ShipmentViewModel();

            var progressSelections = new List<SelectListItem>();

            var selection1 = new SelectListItem() { Text = "Pending Payments", Value = "1" };
            progressSelections.Insert(0, selection1);
            var selection2 = new SelectListItem() { Text = "Pending Shipment Analysis", Value = "2" };
            progressSelections.Insert(0, selection2);
            var selection3 = new SelectListItem() { Text = "Without Arrival Notices", Value = "3" };
            progressSelections.Insert(0, selection3);
            var selection4 = new SelectListItem() { Text = "Without Entry Numbers", Value = "4" };
            progressSelections.Insert(0, selection4);
            var selection5 = new SelectListItem() { Text = "Without Original Documents", Value = "5" };
            progressSelections.Insert(0, selection5);
            var selection6 = new SelectListItem() { Text = "Without Payment Instructions", Value = "6" };
            progressSelections.Insert(0, selection6);

            var defaultSelection = new SelectListItem() { Text = "--Select Progress--", Value = null };

            progressSelections.Insert(0, defaultSelection);

            model.SelectableProgressSelection = progressSelections;

            return View(model);
        }

        /// <summary>
        /// add container modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddContainer()
        {
            return PartialView();
        }

        /// <summary>
        /// add parts modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddParts()
        {
            return PartialView();
        }

        /// <summary>
        /// edit purchase order modal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditPurchaseOrder(BillOfLadingViewModel model)
        {
            return PartialView(model);
        }

        /// <summary>
        /// edit container part modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditContainerPart()
        {
            return PartialView();
        }

        /// <summary>
        /// view ship code notes modal
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewShipCodeNotes(string notes)
        {
            var model = new ContainerPartViewModel();

            model.ShipCodeNotes = notes;

            return PartialView(model);
        }

        /// <summary>
        /// view container part details modal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewContainerPartDetails(ContainerPartViewModel model)
        {
            return PartialView(model);
        }

        /// <summary>
        /// receiving modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _Receiving()
        {
            return PartialView();
        }

        /// <summary>
        /// process dynamics receipts
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderParts"></param>
        /// <returns></returns>
        private OperationResult ProcessDynamicsReceipts(FoundryOrderPartViewModel order, List<FoundryOrderPart> orderParts)
        {
            var operationResult = new OperationResult();

            var receiptLines = new List<POP10310>();

            var newReceiptHeader = new POP10300_Receipt_Work()
            {
                POPRCTNM = "PHANTOM201",
                POPTYPE = 1,
                receiptdate = DateTime.Now.ToShortDateString(),
                BACHNUMB = "COMPSYS",
                VENDORID = "SUO"
            };

            foreach (var orderPart in orderParts)
            {
                var newReceiptLine = new POP10310()
                {
                    POPTYPE = 1,
                    POPRCTNM = "PHANTOM201",
                    SERLTNUM = "B99999",
                    ITEMNMBR = "083198-101A",
                    VENDORID = "SUO",
                    PONUMBER = "A014007",
                    VNDITNUM = "083198-101A",
                    QTYSHPPD = 5
                };

                receiptLines.Add(newReceiptLine);
            }

            operationResult = _receiptDynamicsRepository.SaveReceipt(newReceiptHeader, receiptLines);

            return operationResult;
        }

        /// <summary>
        /// process dynamics payables
        /// </summary>
        /// <param name="billOfLadingId"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult ProcessDynamicsPayables(Guid billOfLadingId)
        {
            var operationResult = new OperationResult();

            //var billofLading = _bolRepository.GetBillOfLading(billOfLadingId);

            //if (billofLading.HasBeenAnalyzed)
            //{
            //    var parts = _reportRepository.GetShipmentAnalysisData(billOfLadingId);

            //    foreach (var groupedPart in groupedParts)
            //    {
            //        var foundryOrder = _foundryOrderRepository.GetFoundryOrder(groupedPart.First().FoundryOrderId);

            //        var newPayable = new PM10000_Payables_Work()
            //        {
            //            BACHNUMB = groupedPart.Key, //ShipCode
            //            VCHNUMWK = "99999999999999",
            //            VENDORID = foundryOrder.FoundryId,
            //            DOCNUMBR = "DOC999999",
            //            DOCTYPE = 1,
            //            DOCAMNT = 1000.00m,
            //            DOCDATE = DateTime.Now,
            //            MSCCHAMT = 0.00m,
            //            PRCHAMNT = 1000.00m,
            //            CHRGAMNT = 1000.00m,
            //            TAXAMNT = 0.00m,
            //            FRTAMNT = 0.00m,
            //            TRDISAMT = 0.00m,
            //            CASHAMNT = 0.00m,
            //            CHEKAMNT = 0.00m,
            //            CRCRDAMT = 0.00m,
            //            DISTKNAM = 0.00m
            //        };

            //        operationResult = _payablesDynamicsRepository.SavePayableTransaction(newPayable);

            //        if (operationResult.Success)
            //        {
            //            var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoiceByBillOfLading(billOfLadingId);

            //            foundryInvoice.HasBeenProcessed = true;

            //            operationResult = _foundryInvoiceRepository.UpdateFoundryInvoice(foundryInvoice);
            //        }
            //    }
            //}
            //else
            //{
            //    operationResult.Success = false;
            //    operationResult.Message = "Unable to process payment, shipment analysis has not been done.";
            //}

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Shipment/FoundryInvoices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
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
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
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
        /// GET: Operations/Shipment/FoundryInvoice
        /// </summary>
        /// <param name="foundryInvoiceId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult FoundryInvoice(Guid foundryInvoiceId)
        {
            var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoice(foundryInvoiceId);

            FoundryInvoiceViewModel model = new FoundryInvoiceConverter().ConvertToView(foundryInvoice);

            var debitMemos = new List<DebitMemoViewModel>();

            var tempDebitMemos = _debitMemoRepository.GetDebitMemos().Where(x => x.FoundryInvoiceId == foundryInvoice.FoundryInvoiceId).ToList();

            if (tempDebitMemos != null && tempDebitMemos.Count > 0)
            {
                foreach (var tempDebitMemo in tempDebitMemos)
                {
                    DebitMemoViewModel convertedModel = new DebitMemoConverter().ConvertToView(tempDebitMemo);

                    debitMemos.Add(convertedModel);
                }
            }

            model.DebitMemos = debitMemos.OrderBy(x => x.DebitMemoNumber).ToList();

            return View(model);
        }
        
        /// <summary>
        /// add attachment
        /// </summary>
        /// <param name="formatDataModel"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult AddAttachment(AttachmentModel formatDataModel)
        {
            var operationResult = new OperationResult();

            var model = new NewAttachmentModel();

            if (formatDataModel.Attachment != null && formatDataModel.Attachment.ContentLength > 0)
            {
                byte[] tempFile = new byte[formatDataModel.Attachment.ContentLength];
                formatDataModel.Attachment.InputStream.Read(tempFile, 0, formatDataModel.Attachment.ContentLength);

                var newDebitMemoAttachment = new DebitMemoAttachmentConverter().ConvertToDomain(formatDataModel);

                operationResult = _debitMemoRepository.SaveDebitMemoAttachment(newDebitMemoAttachment);

                if (operationResult.Success)
                {
                    model.Success = true;
                    model.DebitMemoAttachmentId = operationResult.ReferenceId;
                    model.AttachmentName = operationResult.Name;
                }
                else
                {
                    model.Success = false;
                    model.Message = "Unable to add attachment";
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete attachment
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteAttachment(Guid attachmentId)
        {
            var operationResult = new OperationResult();

            operationResult = _debitMemoRepository.DeleteDebitMemoAttachment(attachmentId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Shipment/debitMemos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult DebitMemos()
        {
            var model = new DebitMemoViewModel();

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

            model.FromDateStr = DateTime.Now.ToShortDateString();
            model.ToDateStr = DateTime.Now.ToShortDateString();

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Shipment/CreateDebitMemo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreateDebitMemo()
        {
            var model = new DebitMemoViewModel();

            model.DebitMemoNumber = DebitMemoNumber();

            _debitMemoRepository.RemoveDebitMemoNumber(model.DebitMemoNumber);

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            model.SelectableFoundryInvoices = _foundryInvoiceRepository.GetSelectableFoundryInvoices();

            var defaultFoundryInvoice = new SelectListItem()
            {
                Text = "--Select Foundry Invoice--",
                Value = null
            };

            model.SelectableFoundryInvoices.Insert(0, defaultFoundryInvoice);

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableSalespersons = _salespersonDynamicsRepository.GetSelectableSalespersons();

            return View(model);
        }

        /// <summary>
        /// Create debitMemo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult CreateDebitMemo(DebitMemoViewModel model)
        {
            var operationResult = new OperationResult();

            DebitMemo newDebitMemo = new DebitMemoConverter().ConvertToDomain(model);

            operationResult = _debitMemoRepository.SaveDebitMemo(newDebitMemo);

            //CreditMemo newCreditMemo = new CreditMemoConverter().ConvertFromDebitMemo(model);

            //operationResult = _creditMemoRepository.SaveCreditMemo(newCreditMemo);

            if (operationResult.Success)
            {
                //var newPayables = new PM10000_Payables_Work()
                //{
                //    BACHNUMB = newDebitMemo.BACHNUMB,
                //    VCHNUMWK = newDebitMemo.VCHNUMWK,
                //    VENDORID = newDebitMemo.FoundryId,
                //    DOCNUMBR = newDebitMemo.Number,
                //    DOCTYPE = 5,
                //    DOCAMNT = newDebitMemo.Amount,
                //    DOCDATE = DateTime.Now,
                //    MSCCHAMT = newDebitMemo.MSCCHAMT,
                //    PRCHAMNT = newDebitMemo.PRCHAMNT,
                //    CHRGAMNT = newDebitMemo.CHRGAMNT,
                //    TAXAMNT = newDebitMemo.TAXAMNT,
                //    FRTAMNT = newDebitMemo.FRTAMNT,
                //    TRDISAMT = newDebitMemo.TRDISAMT,
                //    CASHAMNT = newDebitMemo.CASHAMNT,
                //    CHEKAMNT = newDebitMemo.CHEKAMNT,
                //    CRCRDAMT = newDebitMemo.CRCRDAMT,
                //    DISTKNAM = newDebitMemo.DISTKNAM
                //};

                //operationResult = _payablesDynamicsRepository.SavePayableTransaction(newPayables);

                CreditMemo newCreditMemo = new CreditMemoConverter().ConvertFromDebitMemo(model);

                operationResult = _creditMemoRepository.SaveCreditMemo(newCreditMemo);

                //if (operationResult.Success)
                //{
                //    var newReceivables = new RM20101()
                //    {
                //        RMDTYPAL = 7,
                //        DOCNUMBR = newCreditMemo.Number,
                //        DOCDATE = DateTime.Now,
                //        BACHNUMB = receivable.BACHNUMB,
                //        CUSTNMBR = newCreditMemo.CustomerId,
                //        DOCAMNT = newCreditMemo.Amount,
                //        SLSAMNT = receivable.SLSAMNT,
                //    };

                //    operationResult = _receivablesDynamicsRepository.SaveReceivableTransaction(newReceivables);
                //}
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Operations/Shipment/DebitMemoDetail
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult DebitMemoDetail(Guid debitMemoId)
        {
            var debitMemo = _debitMemoRepository.GetDebitMemo(debitMemoId);

            DebitMemoViewModel model = new DebitMemoConverter().ConvertToView(debitMemo);

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Shipment/EditDebitMemo
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult EditDebitMemo(Guid debitMemoId)
        {
            var debitMemo = _debitMemoRepository.GetDebitMemo(debitMemoId);

            DebitMemoViewModel model = new DebitMemoConverter().ConvertToView(debitMemo);

            model.SelectableFoundries = _foundryDynamicsRepository.GetSelectableFoundries();

            var defaultFoundry = new SelectListItem()
            {
                Text = "--Select Foundry--",
                Value = null
            };

            model.SelectableFoundries.Insert(0, defaultFoundry);

            model.SelectableFoundryInvoices = _foundryInvoiceRepository.GetSelectableFoundryInvoices();

            var defaultFoundryInvoice = new SelectListItem()
            {
                Text = "--Select Foundry Invoice--",
                Value = null
            };

            model.SelectableFoundryInvoices.Insert(0, defaultFoundryInvoice);

            model.SelectableCustomers = _customerDynamicsRepository.GetSelectableCustomers();

            var defaultCustomer = new SelectListItem()
            {
                Text = "--Select Customer--",
                Value = null
            };

            model.SelectableCustomers.Insert(0, defaultCustomer);

            model.SelectableSalespersons = _salespersonDynamicsRepository.GetSelectableSalespersons();

            return View(model);
        }

        /// <summary>
        /// edit debitMemo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult EditDebitMemo(DebitMemoViewModel model)
        {
            var operationResult = new OperationResult();

            var debitMemo = new DebitMemoConverter().ConvertToDomain(model);

            operationResult = _debitMemoRepository.UpdateDebitMemo(debitMemo);

            if (operationResult.Success)
            {
                CreditMemo newCreditMemo = new CreditMemoConverter().ConvertFromDebitMemo(model);

                operationResult = _creditMemoRepository.UpdateCreditMemo(newCreditMemo);
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// delete debitMemo
        /// </summary>
        /// <param name="debitMemoId"></param>
        /// <returns></returns>
        [HttpDelete]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult DeleteDebitMemo(Guid debitMemoId)
        {
            var operationResult = new OperationResult();

            operationResult = _debitMemoRepository.DeleteDebitMemo(debitMemoId);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// search debitMemos with filter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SearchDebitMemos(DebitMemoViewModel model)
        {
            var debitMemos = new List<DebitMemoViewModel>();

            var tempDebitMemos = _debitMemoRepository.GetDebitMemos();

            if (tempDebitMemos != null && tempDebitMemos.Count > 0)
            {
                foreach (var tempDebitMemo in tempDebitMemos)
                {
                    DebitMemoViewModel convertedModel = new DebitMemoConverter().ConvertToListView(tempDebitMemo);

                    debitMemos.Add(convertedModel);
                }
            }

            if (model.DebitMemoNumber != null && model.DebitMemoNumber != string.Empty)
            {
                debitMemos = debitMemos.Where(x => x.DebitMemoNumber.ToLower() == model.DebitMemoNumber.ToLower()).ToList();
            }

            if (model.InvoiceNumber != null && model.InvoiceNumber != string.Empty)
            {
                debitMemos = debitMemos.Where(x => x.InvoiceNumber.ToLower() == model.InvoiceNumber.ToLower()).ToList();
            }

            if (model.RmaNumber != null && model.RmaNumber != string.Empty)
            {
                debitMemos = debitMemos.Where(x => x.RmaNumber.ToLower() == model.RmaNumber.ToLower()).ToList();
            }

            if (model.CustomerId != null && model.CustomerId != string.Empty && model.CustomerId != "--Select Customer--")
            {
                debitMemos = debitMemos.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.FoundryId != null && model.FoundryId != string.Empty && model.FoundryId != "--Select Foundry--")
            {
                debitMemos = debitMemos.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.FromDate != null && model.ToDate != null)
            {
                var fromDate = model.FromDate;
                var toDate = model.ToDate.AddDays(1);

                debitMemos = debitMemos.Where(x => x.DebitMemoDate >= fromDate && x.DebitMemoDate < toDate).ToList();
            }

            if (model.PartNumber != null && model.PartNumber != string.Empty)
            {
                if (debitMemos != null && debitMemos.Count > 0)
                {
                    for (var i = debitMemos.Count - 1; i >= 0; i--)
                    {
                        var debitMemoItems = _debitMemoRepository.GetDebitMemoItems(debitMemos[i].DebitMemoId).ToList();

                        if (debitMemoItems != null && debitMemoItems.Count > 0)
                        {
                            var debitMemoItem = debitMemoItems.FirstOrDefault(x => x.PartNumber.ToLower() == model.PartNumber.ToLower());

                            if (debitMemoItem == null)
                            {
                                debitMemos.RemoveAt(i);
                            }
                        }
                    }
                }
            }

            model.DebitMemos = debitMemos.OrderBy(x => x.DebitMemoNumber).ToList(); 

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add debitMemo item modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddDebitMemoItem()
        {
            DebitMemoViewModel model = new DebitMemoViewModel();

            return PartialView(model);
        }

        /// <summary>
        /// edit debitMemo item modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _EditDebitMemoItem()
        {
            DebitMemoViewModel model = new DebitMemoViewModel();

            return PartialView(model);
        }

        /// <summary>
        /// GET: Operations/Shipment/CreditMemos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreditMemos()
        {
            var model = new CreditMemoViewModel();

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

            model.FromDateStr = DateTime.Now.ToShortDateString();
            model.ToDateStr = DateTime.Now.ToShortDateString();

            return View(model);
        }

        /// <summary>
        /// GET: Operations/Shipment/CreditMemo
        /// </summary>
        /// <param name="creditMemoId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult CreditMemo(Guid creditMemoId)
        {
            var creditMemo = _creditMemoRepository.GetCreditMemo(creditMemoId);

            CreditMemoViewModel model = new CreditMemoConverter().ConvertToView(creditMemo);

            return View(model);
        }

        /// <summary>
        /// search creditMemos with filter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult SearchCreditMemos(CreditMemoViewModel model)
        {
            var creditMemos = new List<CreditMemoViewModel>();

            var tempCreditMemos = _creditMemoRepository.GetCreditMemos();

            if (tempCreditMemos != null && tempCreditMemos.Count > 0)
            {
                foreach (var tempCreditMemo in tempCreditMemos)
                {
                    CreditMemoViewModel convertedModel = new CreditMemoConverter().ConvertToListView(tempCreditMemo);

                    creditMemos.Add(convertedModel);
                }
            }

            if (model.CreditMemoNumber != null && model.CreditMemoNumber != string.Empty)
            {
                creditMemos = creditMemos.Where(x => x.CreditMemoNumber.ToLower() == model.CreditMemoNumber.ToLower()).ToList();
            }

            if (model.RmaNumber != null && model.RmaNumber != string.Empty)
            {
                creditMemos = creditMemos.Where(x => x.RmaNumber.ToLower() == model.RmaNumber.ToLower()).ToList();
            }

            if (model.CustomerId != null && model.CustomerId != string.Empty && model.CustomerId != "--Select Customer--")
            {
                creditMemos = creditMemos.Where(x => x.CustomerId.Replace(" ", string.Empty).ToLower() == model.CustomerId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.FoundryId != null && model.FoundryId != string.Empty && model.FoundryId != "--Select Foundry--")
            {
                creditMemos = creditMemos.Where(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == model.FoundryId.Replace(" ", string.Empty).ToLower()).ToList();
            }

            if (model.PartNumber != null && model.PartNumber != string.Empty)
            {
                if (creditMemos != null && creditMemos.Count > 0)
                {
                    for (var i = creditMemos.Count - 1; i >= 0; i--)
                    {
                        var creditMemoItems = _creditMemoRepository.GetCreditMemoItems(creditMemos[i].CreditMemoId).ToList();

                        if (creditMemoItems != null && creditMemoItems.Count > 0)
                        {
                            var creditMemoItem = creditMemoItems.FirstOrDefault(x => x.PartNumber.ToLower() == model.PartNumber.ToLower());

                            if (creditMemoItem == null)
                            {
                                creditMemos.RemoveAt(i);
                            }
                        }
                    }
                }
            }

            if (model.FromDate != null && model.ToDate != null)
            {
                var fromDate = model.FromDate;
                var toDate = model.ToDate.AddDays(1);

                creditMemos = creditMemos.Where(x => x.CreditMemoDate >= fromDate && x.CreditMemoDate < toDate).ToList();
            }

            model.CreditMemos = creditMemos.OrderBy(x => x.CreditMemoNumber).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// check if shipment can be delete
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ValidateShipmentDelete(Guid shipmentId)
        {
            var operationResult = new OperationResult();

            operationResult.Success = true;

            var bols = _bolRepository.GetBillOfLadings().Where(x => x.ShipmentId == shipmentId).ToList();

            if (bols != null && bols.Count() > 0)
            {
                foreach (var bol in bols)
                {
                    var containers = _containerRepository.GetContainers().Where(x => x.BillOfLadingId == bol.BillOfLadingId);
                    if (containers != null && containers.Count() > 0)
                    {
                        foreach (var container in containers)
                        {
                            var containerParts = _containerRepository.GetContainerParts().Where(x => x.ContainerId == container.ContainerId);
                            if (containerParts != null && containerParts.Count() > 0)
                            {
                                foreach (var containerPart in containerParts)
                                {
                                    var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPart(containerPart.FoundryOrderPartId);
                                    if (foundryOrderPart != null && foundryOrderPart.HasBeenReceived)
                                    {
                                        operationResult.Success = false;
                                        operationResult.Message = "Unable to Delete, There are parts included in this shipment that have been received!";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// check if bill of lading can be delete 
        /// </summary>
        /// <param name="bolId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ValidateBillofLadingDelete(Guid bolId)
        {
            var operationResult = new OperationResult();

            operationResult.Success = true;

            var containers = _containerRepository.GetContainers().Where(x => x.BillOfLadingId == bolId);
            if (containers != null && containers.Count() > 0)
            {
                foreach (var container in containers)
                {
                    var containerParts = _containerRepository.GetContainerParts().Where(x => x.ContainerId == container.ContainerId);
                    if (containerParts != null && containerParts.Count() > 0)
                    {
                        foreach (var containerPart in containerParts)
                        {
                            var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPart(containerPart.FoundryOrderPartId);
                            if (foundryOrderPart != null && foundryOrderPart.HasBeenReceived)
                            {
                                operationResult.Success = false;
                                operationResult.Message = "Unable to Delete, There are parts included in this bol that have been received!";
                                break;
                            }
                        }
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// check shipment can be analysis
        /// </summary>
        /// <param name="bolId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ValidateShipmentAnalysis(Guid bolId)
        {
            var operationResult = new OperationResult();

            operationResult.Success = false;
            operationResult.Message = "Unable to Analysis, There are no parts included in this bol that have been received!";

            var containers = _containerRepository.GetContainers().Where(x => x.BillOfLadingId == bolId);
            if (containers != null && containers.Count() > 0)
            {
                foreach (var container in containers)
                {
                    var containerParts = _containerRepository.GetContainerParts().Where(x => x.ContainerId == container.ContainerId);
                    if (containerParts != null && containerParts.Count() > 0)
                    {
                        foreach (var containerPart in containerParts)
                        {
                            var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPart(containerPart.FoundryOrderPartId);
                            if (foundryOrderPart != null && foundryOrderPart.HasBeenReceived)
                            {
                                operationResult.Success = true;
                                break;
                            }
                        }
                    }
                }
            }

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get bill of lading by progress
        /// </summary>
        /// <param name="progressId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetBillsOfLadingByProgress(string progressId)
        {
            var model = new ShipmentViewModel();

            var billsOfLading = new List<BillOfLadingViewModel>();

            switch (progressId)
            {
                case "1":
                    billsOfLading = GetBillsOfLadingPendingPayment();
                    break;
                case "2":
                    billsOfLading = GetBillsOfLadingPendingAnalysis();
                    break;
                case "3":
                    billsOfLading = GetBillsOfWithoutArrivalNotices();
                    break;
                case "4":
                    billsOfLading = GetBillsOfLadingWithoutCustomsNumbers();
                    break;
                case "5":
                    billsOfLading = GetBillsOfLadingWithoutOriginalDocuments();
                    break;
                case "6":
                    billsOfLading = GetBillsOfLadingWithoutWireInstructions();
                    break;
            }

            model.BillsOfLading = billsOfLading;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get image data
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetImageData(HttpPostedFileBase attachment)
        {
            var model = new NewAttachmentModel();

            if (attachment == null && attachment.ContentLength < 1)
            {
                model.Success = false;
                model.Message = "Error occurred...image file not found or contains no data.";
            }
            else
            {
                if (!attachment.FileName.EndsWith(".png") && !attachment.FileName.EndsWith(".jpg") && !attachment.FileName.EndsWith(".pdf"))
                {
                    model.Success = false;
                    model.Message = "Error occurred...image file not in correct format.";
                }
                else
                {
                    string trimmedFileName = string.Empty;

                    byte[] tempFile = new byte[attachment.ContentLength];
                    attachment.InputStream.Read(tempFile, 0, attachment.ContentLength);

                    if (attachment.FileName.EndsWith("png"))
                    {
                        trimmedFileName = attachment.FileName.Replace(".png", "");
                    }
                    else if (attachment.FileName.EndsWith("jpg"))
                    {
                        trimmedFileName = attachment.FileName.Replace(".jpg", "");
                    }
                    else if (attachment.FileName.EndsWith("pdf"))
                    {
                        trimmedFileName = attachment.FileName.Replace(".pdf", "");
                    }

                    model.Success = true;
                    model.AttachmentName = trimmedFileName;
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get attachment by id
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult GetAttachment(Guid attachmentId)
        {
            var attachment = new NewAttachmentModel();

            attachment = _debitMemoRepository.GetDebitMemoAttachments().Where(x => x.DebitMemoAttachmentId == attachmentId)
                                               .Select(y => new NewAttachmentModel()
                                               {
                                                   Content = y.Content,
                                                   Type = y.Type,
                                                   AttachmentName = y.Name
                                               }).FirstOrDefault();

            MemoryStream ms = new MemoryStream(attachment.Content, 0, 0, true, true);
            Response.ContentType = attachment.Type;
            Response.AddHeader("content-disposition", "attachment;filename=" + attachment.AttachmentName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, attachment.Type);
        }

        /// <summary>
        /// get all debitMemos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetAllDebitMemos()
        {
            var model = new ShipmentViewModel();

            var debitMemos = new List<DebitMemoViewModel>();

            var tempDebitMemos = _debitMemoRepository.GetDebitMemos();

            if (tempDebitMemos != null && tempDebitMemos.Count > 0)
            {
                foreach (var tempDebitMemo in tempDebitMemos)
                {
                    DebitMemoViewModel convertedModel = new DebitMemoConverter().ConvertToListView(tempDebitMemo);

                    debitMemos.Add(convertedModel);
                }
            }

            model.DebitMemos = debitMemos.OrderBy(x => x.DebitMemoNumber).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get open debitMemos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetOpenDebitMemos()
        {
            var model = new ShipmentViewModel();

            var debitMemos = new List<DebitMemoViewModel>();

            var tempDebitMemos = _debitMemoRepository.GetDebitMemos().Where(x => x.IsOpen).ToList();

            if (tempDebitMemos != null && tempDebitMemos.Count > 0)
            {
                foreach (var tempDebitMemo in tempDebitMemos)
                {
                    DebitMemoViewModel convertedModel = new DebitMemoConverter().ConvertToListView(tempDebitMemo);

                    debitMemos.Add(convertedModel);
                }
            }

            model.DebitMemos = debitMemos.OrderBy(x => x.DebitMemoNumber).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get closed debitMemos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetClosedDebitMemos()
        {
            var model = new ShipmentViewModel();

            var debitMemos = new List<DebitMemoViewModel>();

            var tempDebitMemos = _debitMemoRepository.GetDebitMemos().Where(x => x.IsClosed).ToList();

            if (tempDebitMemos != null && tempDebitMemos.Count > 0)
            {
                foreach (var tempDebitMemo in tempDebitMemos)
                {
                    DebitMemoViewModel convertedModel = new DebitMemoConverter().ConvertToListView(tempDebitMemo);

                    debitMemos.Add(convertedModel);
                }
            }

            model.DebitMemos = debitMemos.OrderBy(x => x.DebitMemoNumber).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get purchase orders by shipment
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetPurchaseOrdersByShipmentId(Guid shipmentId)
        {
            var model = new FoundryOrderViewModel();

            var purchaseOrders = new List<FoundryOrderViewModel>();

            var billsOfLading = _bolRepository.GetBillOfLadings().Where(x => x.ShipmentId == shipmentId).ToList();

            if (billsOfLading != null && billsOfLading.Count > 0)
            {
                foreach (var billOfLading in billsOfLading)
                {
                    var containers = _containerRepository.GetContainers().Where(x => x.BillOfLadingId == billOfLading.BillOfLadingId).ToList();

                    if (containers != null && containers.Count > 0)
                    {
                        foreach (var container in containers)
                        {
                            var containerParts = _containerRepository.GetContainerParts().Where(x => x.ContainerId == container.ContainerId).ToList();

                            if (containerParts != null && containerParts.Count > 0)
                            {
                                foreach (var containerPart in containerParts)
                                {
                                    var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPart(containerPart.FoundryOrderPartId);
                                    var foundryOrder = _foundryOrderRepository.GetFoundryOrder((foundryOrderPart != null) ? foundryOrderPart.FoundryOrderId : Guid.Empty);

                                    if (foundryOrder != null)
                                    {
                                        FoundryOrderViewModel convertedModel = new FoundryOrderConverter().ConvertToView(foundryOrder);

                                        purchaseOrders.Add(convertedModel);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            model.PurchaseOrders = purchaseOrders.OrderBy(z => z.OrderNumber).ThenBy(s => s.ShipCode).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get bill of ladings by shipment
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetBolsByShipmentId(Guid shipmentId)
        {
            var model = new BillOfLadingViewModel();

            var billsOfLading = new List<BillOfLadingViewModel>();

            var tempBillsOfLading = _bolRepository.GetBillOfLadings().Where(x => x.ShipmentId == shipmentId).ToList();

            if (tempBillsOfLading != null && tempBillsOfLading.Count > 0)
            {
                foreach (var billOfLading in tempBillsOfLading)
                {
                    BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToListView(billOfLading);

                    billsOfLading.Add(convertedModel);
                }
            }

            model.BillsOfLading = billsOfLading;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get foundry invoices by foundry
        /// </summary>
        /// <param name="foundryId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public JsonResult GetSelectableFoundryInvoicesByFoundry(string foundryId)
        {
            var model = new DebitMemoViewModel();

            model.SelectableFoundryInvoices = _foundryInvoiceRepository.GetSelectableFoundryInvoicesByFoundry(foundryId);

            return Json(model, JsonRequestBehavior.AllowGet);
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

                if (_salespersonDynamicsRepository != null)
                {
                    _salespersonDynamicsRepository.Dispose();
                    _salespersonDynamicsRepository = null;
                }

                if (_partRepository != null)
                {
                    _partRepository.Dispose();
                    _partRepository = null;
                }

                if (_foundryDynamicsRepository != null)
                {
                    _foundryDynamicsRepository.Dispose();
                    _foundryDynamicsRepository = null;
                }

                if (_carrierRepository != null)
                {
                    _carrierRepository.Dispose();
                    _carrierRepository = null;
                }

                if (_vesselRepository != null)
                {
                    _vesselRepository.Dispose();
                    _vesselRepository = null;
                }

                if (_portRepository != null)
                {
                    _portRepository.Dispose();
                    _portRepository = null;
                }

                if (_shipmentRepository != null)
                {
                    _shipmentRepository.Dispose();
                    _shipmentRepository = null;
                }

                if (_containerRepository != null)
                {
                    _containerRepository.Dispose();
                    _containerRepository = null;
                }

                if (_bolRepository != null)
                {
                    _bolRepository.Dispose();
                    _bolRepository = null;
                }

                if (_foundryInvoiceRepository != null)
                {
                    _foundryInvoiceRepository.Dispose();
                    _foundryInvoiceRepository = null;
                }

                if (_foundryOrderRepository != null)
                {
                    _foundryOrderRepository.Dispose();
                    _foundryOrderRepository = null;
                }

                if (_buckRepository != null)
                {
                    _buckRepository.Dispose();
                    _buckRepository = null;
                }

                if (_accountCodeRepository != null)
                {
                    _accountCodeRepository.Dispose();
                    _accountCodeRepository = null;
                }

                if (_debitMemoRepository != null)
                {
                    _debitMemoRepository.Dispose();
                    _debitMemoRepository = null;
                }

                if (_creditMemoRepository != null)
                {
                    _creditMemoRepository.Dispose();
                    _creditMemoRepository = null;
                }

                if (_payablesDynamicsRepository != null)
                {
                    _payablesDynamicsRepository.Dispose();
                    _payablesDynamicsRepository = null;
                }

                if (_receivablesDynamicsRepository != null)
                {
                    _receivablesDynamicsRepository.Dispose();
                    _receivablesDynamicsRepository = null;
                }

                if (_customerOrderRepository != null)
                {
                    _customerOrderRepository.Dispose();
                    _customerOrderRepository = null;
                }

                if (_receiptDynamicsRepository != null)
                {
                    _receiptDynamicsRepository.Dispose();
                    _receiptDynamicsRepository = null;
                }

                if (_reportRepository != null)
                {
                    _reportRepository.Dispose();
                    _reportRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}