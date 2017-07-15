using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class FoundryOrderConverter
    {
        /// <summary>
        /// convert foudry order to list model
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public FoundryOrderViewModel ConvertToListView(FoundryOrder order)
        {
            FoundryOrderViewModel model = new FoundryOrderViewModel();
   
            var _customerDynamicsRepository = new CustomerDynamicsRepository();      
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(order.CustomerId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(order.FoundryId);

            model.FoundryOrderId = order.FoundryOrderId;
            model.OrderNumber = (!string.IsNullOrEmpty(order.Number)) ? order.Number : "N/A";
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.DueDate = (order.DueDate != null) ? order.DueDate : DateTime.MinValue;
            model.DueDateStr = (order.DueDate != null) ? order.DueDate.Value.ToShortDateString() : "N/A";
            model.ShipDate = (order.ShipDate != null) ? order.ShipDate : DateTime.MinValue;
            model.ShipDateStr = (order.ShipDate != null) ? order.ShipDate.Value.ToShortDateString() : "N/A";
            model.IsConfirmed = order.IsConfirmed;
            model.IsOpen = order.IsOpen ? true : false;
            model.IsHold = order.IsHold;
            model.IsCanceled = order.IsCanceled;
            model.IsComplete = order.IsComplete;
            model.Status = order.IsOpen ? "Open" : order.IsCanceled ? "Canceled" : order.IsComplete ? "Completed" : order.IsHold ? "On Hold" : "N/A";          
            model.IsSample = order.IsSample;
            model.IsTooling = order.IsTooling;
            model.IsProduction = order.IsProduction;
            model.OrderTypeDescription = order.IsSample ? "Sample" : order.IsTooling ? "Tooling" : order.IsProduction ? "Production" : "N/A";
            model.CreatedDate = (order.CreatedDate != null) ? order.CreatedDate : DateTime.MinValue;
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
        /// convert foundry order to view model
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public FoundryOrderViewModel ConvertToView(FoundryOrder order)
        {
            FoundryOrderViewModel model = new FoundryOrderViewModel();

            var _projectRepository = new ProjectRepository(); 
            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            var _stateRepository = new StateRepository();  
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _siteDynamicsRepository = new SiteDynamicsRepository();
            var _orderTermRepository = new OrderTermRepository();
            var _billOfLadingRepository = new BillOfLadingRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();

            var project = _projectRepository.GetProject(order.ProjectId);
            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(order.CustomerId);
            var dynamicsCustomerAddress = _customerAddressDynamicsRepository.GetCustomerAddress(order.CustomerAddressId);
            var state = _stateRepository.GetState((dynamicsCustomerAddress != null && !string.IsNullOrEmpty(dynamicsCustomerAddress.STATE)) ? dynamicsCustomerAddress.STATE : string.Empty);
            var stateName = (state != null && !string.IsNullOrEmpty(state.Name)) ? ", " + state.Name : string.Empty;
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(order.FoundryId);
            var dynamicsSite = _siteDynamicsRepository.GetSite((dynamicsCustomerAddress != null && !string.IsNullOrEmpty(dynamicsCustomerAddress.LOCNCODE)) ? dynamicsCustomerAddress.LOCNCODE : string.Empty);
            var orderTerm = _orderTermRepository.GetOrderTerm(order.ShipmentTermsId);
            var billOfLading = _billOfLadingRepository.GetBillOfLadings().FirstOrDefault(x => x.FoundryId.Replace(" ", string.Empty).ToLower() == order.FoundryId.Replace(" ", string.Empty).ToLower());
            var parts = _foundryOrderRepository.GetFoundryOrderParts().Where(x => x.FoundryOrderId == order.FoundryOrderId).ToList();

            model.FoundryOrderId = order.FoundryOrderId;
            model.BillOfLadingId = (billOfLading != null) ? billOfLading.BillOfLadingId : Guid.Empty;
            model.OrderNumber = (!string.IsNullOrEmpty(order.Number)) ? order.Number : "N/A";
            model.CustomerId = order.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.CustomerAddressId = order.CustomerAddressId;
            model.CustomerAddress = (dynamicsCustomerAddress != null) ? dynamicsCustomerAddress.ADDRESS1 + " " + dynamicsCustomerAddress.CITY + ", " + stateName : "N/A";
            model.ProjectId = order.ProjectId;
            model.ProjectName = (project != null) ? project.Name : "N/A";
            model.FoundryId = order.FoundryId;
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.OrderNotes = (!string.IsNullOrEmpty(order.Notes)) ? order.Notes : "N/A";
            model.ShipmentTermsId = order.ShipmentTermsId;
            model.ShipmentTerms = (orderTerm != null) ? orderTerm.Description : "N/A";
            model.OrderDate = order.OrderDate;
            model.PODate = (order.OrderDate != null) ? order.OrderDate : DateTime.MinValue;
            model.PODateStr = (order.OrderDate != null) ? order.OrderDate.ToShortDateString() : "N/A";
            model.DueDate = (order.DueDate != null) ? order.DueDate : DateTime.MinValue;
            model.DueDateStr = (order.DueDate != null) ? order.DueDate.Value.ToShortDateString() : "N/A";
            model.PortDate = (order.PortDate != null) ? order.PortDate : DateTime.MinValue;
            model.PortDateStr = (order.PortDate != null) ? order.PortDate.Value.ToShortDateString() : "N/A";
            model.ShipDate = (order.ShipDate != null) ? order.ShipDate : DateTime.MinValue;
            model.ShipDateStr = (order.ShipDate != null) ? order.ShipDate.Value.ToShortDateString() : "N/A";
            model.EstArrivalDate = (order.EstArrivalDate != null) ? order.EstArrivalDate : DateTime.MinValue;
            model.EstArrivalDateStr = (order.EstArrivalDate != null) ? order.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.ShipVia = (!string.IsNullOrEmpty(order.ShipVia)) ? order.ShipVia : "N/A";
            model.TransitDays = order.TransitDays;
            model.SiteId = order.SiteId;
            model.SiteDescription = (dynamicsSite != null && !string.IsNullOrEmpty(dynamicsSite.LOCNDSCR)) ? dynamicsSite.LOCNDSCR : "N/A";
            model.IsOpen = order.IsOpen ? true : false;
            model.IsConfirmed = order.IsConfirmed;
            model.ConfirmedDate = (order.ConfirmedDate != null) ? order.ConfirmedDate : DateTime.MinValue;
            model.ConfirmedDateStr = (order.ConfirmedDate != null) ? order.ConfirmedDate.Value.ToShortDateString() : "N/A";
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

            if (parts != null && parts.Count > 0)
            {
                var foundryOrderParts = new List<FoundryOrderPartViewModel>();

                var totalPrice = 0.00m;

                foreach (var part in parts)
                {
                    FoundryOrderPartViewModel foundryOrderPart = new FoundryOrderPartViewModel();

                    if (model.OrderTypeDescription.Equals("Sample") || model.OrderTypeDescription.Equals("Tooling"))
                    {
                        foundryOrderPart = new FoundryOrderPartConverter().ConvertToProjectPartView(part);
                    }
                    else
                    {
                        foundryOrderPart = new FoundryOrderPartConverter().ConvertToPartView(part);
                    }

                    foundryOrderParts.Add(foundryOrderPart);

                    totalPrice += (foundryOrderPart.Price * foundryOrderPart.FoundryOrderQuantity);
                }

                model.TotalPrice = Math.Round(totalPrice, 2);
                model.FoundryOrderParts = foundryOrderParts;
            }

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

            if (_stateRepository != null)
            {
                _stateRepository.Dispose();
                _stateRepository = null;
            }

            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
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

            if (_billOfLadingRepository != null)
            {
                _billOfLadingRepository.Dispose();
                _billOfLadingRepository = null;
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

            return model;
        }

        /// <summary>
        /// convert foundry order view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FoundryOrder ConvertToDomain(FoundryOrderViewModel model)
        {
            var _customerOrderRepository = new CustomerOrderRepository();

            FoundryOrder order = new FoundryOrder();


            order.FoundryOrderId = model.FoundryOrderId;
            order.Number = model.OrderNumber;
            order.CustomerId = model.CustomerId;
            order.ProjectId = model.ProjectId;
            order.CustomerAddressId = model.CustomerAddressId;
            order.FoundryId = model.FoundryId;
            order.SiteId = model.SiteId;
            order.ShipmentTermsId = model.ShipmentTermsId;
            order.OrderDate = model.OrderDate ?? DateTime.Now;
            order.ShipDate = model.ShipDate;
            order.EstArrivalDate = model.EstArrivalDate;
            order.DueDate = model.DueDate;
            order.PortDate = model.PortDate;
            order.Notes = model.OrderNotes;
            order.ShipVia = model.ShipVia;
            order.TransitDays = model.TransitDays;
            order.IsConfirmed = model.IsConfirmed;
            order.ConfirmedDate = model.ConfirmedDate;
            order.IsOpen = model.IsOpen;
            order.IsHold = model.IsHold;
            order.HoldExpirationDate = (model.IsHold) ? model.HoldExpirationDate : null;
            order.HoldNotes = (model.IsHold) ? model.HoldNotes : null;
            order.IsCanceled = model.IsCanceled;
            order.CanceledDate = (model.IsCanceled) ? model.CanceledDate : null;
            order.CancelNotes = (model.IsCanceled) ? model.CancelNotes : null;
            order.IsComplete = model.IsComplete;
            order.CompletedDate = model.CompletedDate;
            order.IsSample = model.IsSample;
            order.IsTooling = model.IsTooling;
            order.IsProduction = model.IsProduction;

            if (model.FoundryOrderParts != null && model.FoundryOrderParts.Count > 0)
            {
                var foundryOrderParts = new List<FoundryOrderPart>();

                foreach (var foundryOrderPart in model.FoundryOrderParts)
                {
                    var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart(foundryOrderPart.CustomerOrderPartId);

                    if (model.IsSample || model.IsTooling)
                    { 
                        foundryOrderPart.ProjectPartId = (customerOrderPart != null) ? customerOrderPart.ProjectPartId : null;
                        foundryOrderPart.PartId = null;
                    }
                    else
                    {
                        foundryOrderPart.ProjectPartId = null;
                        foundryOrderPart.PartId = (customerOrderPart != null) ? customerOrderPart.PartId : null;
                    }

                    foundryOrderPart.FoundryOrderId = model.FoundryOrderId;

                    FoundryOrderPart orderPart = new FoundryOrderPartConverter().ConvertToDomain(foundryOrderPart);

                    foundryOrderParts.Add(orderPart);
                }

                order.FoundryOrderParts = foundryOrderParts;
            }

            if (_customerOrderRepository != null)
            {
                _customerOrderRepository.Dispose();
                _customerOrderRepository = null;
            }

            return order;
        }
    }
}