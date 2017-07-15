using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class CustomerOrderConverter
    {
        /// <summary>
        /// convert customer order to list model
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public CustomerOrderViewModel ConvertToListView(CustomerOrder order)
        {
            CustomerOrderViewModel model = new CustomerOrderViewModel();

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(order.CustomerId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(order.FoundryId);

            model.CustomerOrderId = order.CustomerOrderId;
            model.CustomerName = (dynamicsCustomer != null) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.PONumber = (!string.IsNullOrEmpty(order.PONumber)) ? order.PONumber : "N/A";
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.PODate = order.PODate;
            model.PODateStr = (order.PODate != null) ? order.PODate.ToShortDateString() : "N/A";
            model.DueDate = (order.DueDate != null) ? order.DueDate : DateTime.MinValue;
            model.DueDateStr = (order.DueDate != null) ? order.DueDate.Value.ToShortDateString() : "N/A";
            model.IsOpen = order.IsOpen;
            model.IsHold = order.IsHold;
            model.IsCanceled = order.IsCanceled;
            model.IsComplete = order.IsComplete;
            model.Status = order.IsOpen ? "Open" : order.IsCanceled ? "Canceled" : order.IsComplete ? "Completed" : order.IsHold ? "Hold" : "N/A";
            model.IsSample = order.IsSample;
            model.IsTooling = order.IsTooling;
            model.IsProduction = order.IsProduction;
            model.OrderTypeDescription = order.IsSample ? "Sample" : order.IsTooling ? "Tooling" : order.IsProduction ? "Production" : "N/A";
            model.CreatedDate = order.CreatedDate;
            model.HoldExpirationDate = order.HoldExpirationDate;
            model.HoldNotes = order.HoldNotes;

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

            return model;
        }

        /// <summary>
        /// convert customer order to view model
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public CustomerOrderViewModel ConvertToView(CustomerOrder order)
        {
            CustomerOrderViewModel model = new CustomerOrderViewModel();

            var _projectRepository = new ProjectRepository();
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            var _stateRepository = new StateRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _siteDynamicsRepository = new SiteDynamicsRepository();
            var _orderTermRepository = new OrderTermRepository();
            var _priceSheetRepository = new PriceSheetRepository();
            var _customerOrderRepository = new CustomerOrderRepository();

            var project = _projectRepository.GetProject(order.ProjectId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(order.CustomerId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(order.FoundryId);
            var dynamicsAddress = _customerAddressDynamicsRepository.GetCustomerAddress(order.CustomerAddressId);
            var state = _stateRepository.GetState((dynamicsAddress != null && !string.IsNullOrEmpty(dynamicsAddress.STATE)) ? dynamicsAddress.STATE : string.Empty);
            var stateName = (state != null && !string.IsNullOrEmpty(state.Name)) ? ", " + state.Name : string.Empty;
            var dynamicsSite = _siteDynamicsRepository.GetSite((dynamicsAddress != null && !string.IsNullOrEmpty(dynamicsAddress.LOCNCODE)) ? dynamicsAddress.LOCNCODE : string.Empty);
            var orderTerm = _orderTermRepository.GetOrderTerm(order.ShipmentTermsId);
            var customerOrderParts = _customerOrderRepository.GetCustomerOrderParts().Where(x => x.CustomerOrderId == order.CustomerOrderId).ToList();

            model.CustomerOrderId = order.CustomerOrderId;
            model.OrderDate = order.PODate;
            model.PONumber = (!string.IsNullOrEmpty(order.PONumber)) ? order.PONumber : "N/A";
            model.PODate = order.PODate;
            model.PODateStr = (order.PODate != null) ? order.PODate.ToShortDateString() : "N/A";
            model.ProjectId = order.ProjectId;
            model.ProjectName = (project != null && !string.IsNullOrEmpty(project.Name)) ? project.Name : "N/A";
            model.CustomerId = order.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.CustomerAddressId = order.CustomerAddressId;
            model.CustomerAddress = (dynamicsAddress != null) ? dynamicsAddress.ADDRESS1 + " " + dynamicsAddress.CITY + stateName : "N/A";
            model.FoundryId = order.FoundryId;
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.SiteId = order.SiteId;
            model.SiteDescription = (dynamicsSite != null && !string.IsNullOrEmpty(dynamicsSite.LOCNDSCR)) ? dynamicsSite.LOCNDSCR : "N/A";
            model.ShipmentTermsId = order.ShipmentTermsId;
            model.ShipmentTerms = (orderTerm != null && !string.IsNullOrEmpty(orderTerm.Description)) ? orderTerm.Description : "N/A";
            model.PortDate = (order.PortDate != null) ? order.PortDate : DateTime.MinValue;
            model.PortDateStr = (order.PortDate != null) ? order.PortDate.Value.ToShortDateString() : "N/A";
            model.ShipDate = (order.ShipDate != null) ? order.ShipDate : DateTime.MinValue;
            model.ShipDateStr = (order.ShipDate != null) ? order.ShipDate.Value.ToShortDateString() : "N/A";
            model.DueDate = (order.DueDate != null) ? order.DueDate : DateTime.MinValue;
            model.DueDateStr = (order.DueDate != null) ? order.DueDate.Value.ToShortDateString() : "N/A";
            model.EstArrivalDate = (order.EstArrivalDate != null) ? order.EstArrivalDate : DateTime.MinValue;
            model.EstArrivalDateStr = (order.EstArrivalDate != null) ? order.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.OrderNotes = (!string.IsNullOrEmpty(order.Notes)) ? order.Notes : "N/A";
            model.IsOpen = order.IsOpen;
            model.IsHold = order.IsHold;
            model.HoldExpirationDate = (order.HoldExpirationDate != null) ? order.HoldExpirationDate : DateTime.MinValue;
            model.HoldExpirationDateStr = (order.HoldExpirationDate != null) ? order.HoldExpirationDate.Value.ToShortDateString() : "N/A";
            model.HoldNotes = (!string.IsNullOrEmpty(order.HoldNotes)) ? order.HoldNotes : "N/A";
            model.IsCanceled = order.IsCanceled;
            model.CanceledDate = (order.CanceledDate != null) ? order.CanceledDate : DateTime.MinValue;
            model.CanceledDateStr = (order.CanceledDate != null) ? order.CanceledDate.Value.ToShortDateString() : "N/A";
            model.CancelNotes = (!string.IsNullOrEmpty(order.CancelNotes)) ? order.CancelNotes : "N/A";
            model.IsComplete = order.IsComplete;
            model.CompletedDate = (order.CompletedDate != null) ? order.CompletedDate : DateTime.MinValue;
            model.CompletedDateStr = (order.CompletedDate != null) ? order.CompletedDate.Value.ToShortDateString() : "N/A";
            model.Status = order.IsOpen ? "Open" : order.IsCanceled ? "Canceled" : order.IsComplete ? "Completed" : order.IsHold ? "On Hold" : "N/A";        
            model.IsSample = order.IsSample;
            model.IsTooling = order.IsTooling;
            model.IsProduction = order.IsProduction;
            model.OrderTypeDescription = order.IsSample ? "Sample" : order.IsTooling ? "Tooling" : order.IsProduction ? "Production" : "N/A";

            model.CustomerOrderParts = new List<CustomerOrderPartViewModel>();
  
            var totalPrice = 0.00m;

            if (customerOrderParts != null && customerOrderParts.Count > 0)
            {
                foreach (var customerOrderPart in customerOrderParts)
                {
                    CustomerOrderPartViewModel partModel = new CustomerOrderPartViewModel();
                    if (model.OrderTypeDescription.Equals("Sample") || model.OrderTypeDescription.Equals("Tooling"))
                    {
                        partModel = new CustomerOrderPartConverter().ConvertToProjectPartView(customerOrderPart);
                    }
                    else
                    {
                        partModel = new CustomerOrderPartConverter().ConvertToPartView(customerOrderPart);
                    }
                    model.CustomerOrderParts.Add(partModel);
                    totalPrice += (partModel.Price * partModel.CustomerOrderQuantity);
                }
            }

            model.TotalPrice = Math.Round(totalPrice, 2);

            model.CustomerOrderParts = model.CustomerOrderParts.OrderByDescending(y => y.EstArrivalDate).ThenBy(y => y.PartNumber).ToList();

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

            if (_stateRepository != null)
            {
                _stateRepository.Dispose();
                _stateRepository = null;
            }

            if (_siteDynamicsRepository != null)
            {
                _siteDynamicsRepository.Dispose();
                _siteDynamicsRepository = null;
            }

            if (_orderTermRepository != null)
            {
                _orderTermRepository.Dispose();
                _orderTermRepository = null;
            }

            if (_priceSheetRepository != null)
            {
                _priceSheetRepository.Dispose();
                _priceSheetRepository = null;
            }

            if (_customerOrderRepository != null)
            {
                _customerOrderRepository.Dispose();
                _customerOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert customer order view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CustomerOrder ConvertToDomain(CustomerOrderViewModel model)
        {
            CustomerOrder customerOrder = new CustomerOrder();

            customerOrder.CustomerOrderId = model.CustomerOrderId;
            customerOrder.ProjectId = model.ProjectId;
            customerOrder.PONumber = model.PONumber;
            customerOrder.PODate = model.PODate ?? DateTime.Now;
            customerOrder.DueDate = model.DueDate;
            customerOrder.PortDate = model.PortDate;
            customerOrder.ShipDate = model.ShipDate;
            customerOrder.EstArrivalDate = model.EstArrivalDate;
            customerOrder.CustomerId = model.CustomerId;
            customerOrder.FoundryId = model.FoundryId;
            customerOrder.CustomerAddressId = model.CustomerAddressId;
            customerOrder.SiteId = model.SiteId;
            customerOrder.Notes = model.OrderNotes;
            customerOrder.ShipmentTermsId = model.ShipmentTermsId;
            customerOrder.IsOpen = model.IsOpen;
            customerOrder.IsHold = model.IsHold;
            customerOrder.HoldExpirationDate = (model.IsHold) ? model.HoldExpirationDate : null;
            customerOrder.HoldNotes = (model.IsHold) ? model.HoldNotes : null;
            customerOrder.IsCanceled = model.IsCanceled;       
            customerOrder.CanceledDate = (model.IsCanceled) ? model.CanceledDate : null;
            customerOrder.CancelNotes = (model.IsCanceled) ? model.CancelNotes : null;
            customerOrder.IsComplete = model.IsComplete;
            customerOrder.CompletedDate = model.CompletedDate;
            customerOrder.IsSample = model.IsSample;
            customerOrder.IsTooling = model.IsTooling;
            customerOrder.IsProduction = model.IsProduction;

            if (model.CustomerOrderParts != null && model.CustomerOrderParts.Count > 0)
            {
                var customerOrderParts = new List<CustomerOrderPart>();

                foreach (var customerOrderPart in model.CustomerOrderParts)
                {
                    customerOrderPart.CustomerOrderId = model.CustomerOrderId;

                    CustomerOrderPart orderPart = new CustomerOrderPartConverter().ConvertToDomain(customerOrderPart);

                    customerOrderParts.Add(orderPart);
                }

                customerOrder.CustomerOrderParts = customerOrderParts;
            }

            return customerOrder;
        }
    }
}