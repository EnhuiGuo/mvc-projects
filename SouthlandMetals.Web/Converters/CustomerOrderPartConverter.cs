using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class CustomerOrderPartConverter
    {
        /// <summary>
        /// convert customer order part to project part view model
        /// </summary>
        /// <param name="customerOrderPart"></param>
        /// <returns></returns>
        public CustomerOrderPartViewModel ConvertToProjectPartView(CustomerOrderPart customerOrderPart)
        {
            CustomerOrderPartViewModel model = new CustomerOrderPartViewModel();

            var _priceSheetRepository = new PriceSheetRepository();
            var _projectPartRepository = new ProjectPartRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();

            var priceSheetPart = _priceSheetRepository.GetPriceSheetPart(customerOrderPart.PriceSheetPartId);
            var projectPart = _projectPartRepository.GetProjectPart((customerOrderPart.ProjectPartId != null) ? customerOrderPart.ProjectPartId : Guid.Empty);
            var priceSheet = _priceSheetRepository.GetPriceSheet((priceSheetPart != null) ? priceSheetPart.PriceSheetId : Guid.Empty);
            var customerOrder = _customerOrderRepository.GetCustomerOrder(customerOrderPart.CustomerOrderId);
            var receivedQuantity = _foundryOrderRepository.GetFoundryOrderParts().Where(x => x.CustomerOrderPartId == customerOrderPart.CustomerOrderPartId &&
                                                                                              x.HasBeenReceived).Select(y => y.ReceiptQuantity).Sum();

            model.CustomerOrderPartId = customerOrderPart.CustomerOrderPartId;
            model.CustomerOrderId = customerOrderPart.CustomerOrderId;
            model.PONumber = (!string.IsNullOrEmpty(customerOrder.PONumber)) ? customerOrder.PONumber : "N/A";
            model.ProjectPartId = customerOrderPart.ProjectPartId;
            model.PriceSheetPartId = customerOrderPart.PriceSheetPartId;
            model.PriceSheetId = (priceSheet != null) ? priceSheet.PriceSheetId : Guid.Empty;
            model.AvailableQuantity = customerOrderPart.AvailableQuantity;
            model.CustomerOrderQuantity = customerOrderPart.Quantity;
            model.PartNumber = (projectPart != null && !string.IsNullOrEmpty(projectPart.Number)) ? projectPart.Number : "N/A";
            model.PartDescription = (projectPart != null && !string.IsNullOrEmpty(projectPart.Description)) ? projectPart.Description : "N/A";
            model.PriceSheetNumber = (priceSheet != null && !string.IsNullOrEmpty(priceSheet.Number)) ? priceSheet.Number : "N/A";
            model.EstArrivalDate = (customerOrderPart.EstArrivalDate != null) ? customerOrderPart.EstArrivalDate : DateTime.MinValue;
            model.EstArrivalDateStr = (customerOrderPart.EstArrivalDate != null) ? customerOrderPart.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.ReceiptQuantity = receivedQuantity;  
            model.Cost = customerOrderPart.Cost;
            model.Price = customerOrderPart.Price;

            if (_priceSheetRepository != null)
            {
                _priceSheetRepository.Dispose();
                _priceSheetRepository = null;
            }

            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }

            if (_customerOrderRepository != null)
            {
                _customerOrderRepository.Dispose();
                _customerOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert customer order part to part view model
        /// </summary>
        /// <param name="customerOrderPart"></param>
        /// <returns></returns>
        public CustomerOrderPartViewModel ConvertToPartView(CustomerOrderPart customerOrderPart)
        {
            CustomerOrderPartViewModel model = new CustomerOrderPartViewModel();

            var _priceSheetRepository = new PriceSheetRepository();
            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();

           
            var priceSheetPart = _priceSheetRepository.GetPriceSheetPart(customerOrderPart.PriceSheetPartId);
            var part = _partRepository.GetPart((customerOrderPart.PartId != null) ? customerOrderPart.PartId : Guid.Empty);
            var dynamicsPart = _dynamicsPartRepository.GetPartMaster((part != null) ? part.Number : string.Empty);
            var priceSheet = _priceSheetRepository.GetPriceSheet((priceSheetPart != null) ? priceSheetPart.PriceSheetId : Guid.Empty);
            var customerOrder = _customerOrderRepository.GetCustomerOrder(customerOrderPart.CustomerOrderId);
            var receivedQuantity = _foundryOrderRepository.GetFoundryOrderParts().Where(x => x.CustomerOrderPartId == customerOrderPart.CustomerOrderPartId &&
                                                                                              x.HasBeenReceived).Select(y => y.ReceiptQuantity).Sum();

            model.CustomerOrderId = customerOrderPart.CustomerOrderId;
            model.CustomerOrderPartId = customerOrderPart.CustomerOrderPartId;
            model.PONumber = (customerOrder != null && !string.IsNullOrEmpty(customerOrder.PONumber)) ? customerOrder.PONumber : "N/A";
            model.PartId = customerOrderPart.PartId;
            model.PriceSheetPartId = customerOrderPart.PriceSheetPartId;
            model.PriceSheetId = (priceSheet != null) ? priceSheet.PriceSheetId : Guid.Empty;
            model.AvailableQuantity = customerOrderPart.AvailableQuantity;
            model.CustomerOrderQuantity = customerOrderPart.Quantity;
            model.PartNumber = (part != null && !string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartDescription = (dynamicsPart != null && !string.IsNullOrEmpty(dynamicsPart.ITEMDESC)) ? dynamicsPart.ITEMDESC : "N/A";
            model.PriceSheetNumber = (priceSheet != null && !string.IsNullOrEmpty(priceSheet.Number)) ? priceSheet.Number : "N/A";
            model.EstArrivalDate = (customerOrderPart.EstArrivalDate != null) ? customerOrderPart.EstArrivalDate : DateTime.MinValue;
            model.EstArrivalDateStr = (customerOrderPart.EstArrivalDate != null) ? customerOrderPart.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.ShipDate = (customerOrderPart.ShipDate != null) ? customerOrderPart.ShipDate : DateTime.MinValue; ;
            model.ShipDateStr = (customerOrderPart.ShipDate != null) ? customerOrderPart.ShipDate.Value.ToShortDateString() : "N/A";
            model.Cost = customerOrderPart.Cost;
            model.Price = customerOrderPart.Price;
            model.ReceiptQuantity = receivedQuantity;

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

            if (_dynamicsPartRepository != null)
            {
                _dynamicsPartRepository.Dispose();
                _dynamicsPartRepository = null;
            }

            if (_customerOrderRepository != null)
            {
                _customerOrderRepository.Dispose();
                _customerOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert customer order part view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CustomerOrderPart ConvertToDomain(CustomerOrderPartViewModel model)
        {
            CustomerOrderPart part = new CustomerOrderPart();

            part.CustomerOrderPartId = model.CustomerOrderPartId;
            part.ProjectPartId = model.ProjectPartId;
            part.PartId = model.PartId;
            part.PriceSheetPartId = model.PriceSheetPartId;
            part.CustomerOrderId = model.CustomerOrderId;
            part.AvailableQuantity = model.AvailableQuantity;
            part.Quantity = model.CustomerOrderQuantity;
            part.EstArrivalDate = model.EstArrivalDate;
            part.ShipDate = model.ShipDate;
            part.Price = model.Price;
            part.Cost = model.Cost;

            return part;
        }
    }
}