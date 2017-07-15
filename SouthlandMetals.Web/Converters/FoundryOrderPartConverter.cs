using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;

namespace SouthlandMetals.Web.Converters
{
    public class FoundryOrderPartConverter
    {
        /// <summary>
        /// convert foundry order part to project part view model
        /// </summary>
        /// <param name="foundryOrderPart"></param>
        /// <returns></returns>
        public FoundryOrderPartViewModel ConvertToProjectPartView(FoundryOrderPart foundryOrderPart)
        {
            FoundryOrderPartViewModel model = new FoundryOrderPartViewModel();

            var _foundryOrderRepository = new FoundryOrderRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _projectPartRepository = new ProjectPartRepository();
            var _dynamicsReceiptRepository = new ReceiptDynamicsRepository();

            var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart(foundryOrderPart.CustomerOrderPartId);
            var projectPart = _projectPartRepository.GetProjectPart(foundryOrderPart.ProjectPartId);
            var customerOrder = _customerOrderRepository.GetCustomerOrder((customerOrderPart != null) ? customerOrderPart.CustomerOrderId : Guid.Empty);

            model.FoundryOrderPartId = foundryOrderPart.FoundryOrderPartId;
            model.CustomerOrderPartId = foundryOrderPart.CustomerOrderPartId;
            model.FoundryOrderId = foundryOrderPart.FoundryOrderId;
            model.ProjectPartId = foundryOrderPart.ProjectPartId;
            model.CustomerOrderId = (customerOrderPart != null) ? customerOrderPart.CustomerOrderId : Guid.Empty;
            model.AvailableQuantity = (customerOrderPart != null) ? customerOrderPart.AvailableQuantity : 0;
            model.FoundryOrderQuantity = foundryOrderPart.Quantity;
            model.PartNumber = (projectPart != null && !string.IsNullOrEmpty(projectPart.Number)) ? projectPart.Number : "N/A";
            model.PartDescription = (projectPart != null && !string.IsNullOrEmpty(projectPart.Description)) ? projectPart.Description : "N/A";
            model.PONumber = (customerOrder != null && !string.IsNullOrEmpty(customerOrder.PONumber)) ? customerOrder.PONumber : "N/A";
            model.ShipCode = (!string.IsNullOrEmpty(foundryOrderPart.ShipCode)) ? foundryOrderPart.ShipCode : "N/A";
            model.ShipCodeNotes = (!string.IsNullOrEmpty(foundryOrderPart.ShipCodeNotes)) ? foundryOrderPart.ShipCodeNotes : "N/A";
            model.EstArrivalDate = (foundryOrderPart.EstArrivalDate != null) ? foundryOrderPart.EstArrivalDate : DateTime.MinValue;
            model.EstArrivalDateStr = (foundryOrderPart.EstArrivalDate != null) ? foundryOrderPart.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.ShipDate = (foundryOrderPart.ShipDate != null) ? foundryOrderPart.ShipDate : DateTime.MinValue;
            model.ShipDateStr = (foundryOrderPart.ShipDate != null) ? foundryOrderPart.ShipDate.Value.ToShortDateString() : "N/A";
            model.Cost = foundryOrderPart.Cost;
            model.Price = foundryOrderPart.Price;
            model.IsScheduled = foundryOrderPart.IsScheduled;
            model.HasBeenReceived = foundryOrderPart.HasBeenReceived;
            model.ReceiptDate = (foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate : DateTime.MinValue;
            model.ReceiptDateStr = (foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate.Value.ToShortDateString() : "N/A";
            model.ReceiptQuantity = foundryOrderPart.ReceiptQuantity;

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
            if (_projectPartRepository != null)
            {
                _projectPartRepository.Dispose();
                _projectPartRepository = null;
            }
            if (_dynamicsReceiptRepository != null)
            {
                _dynamicsReceiptRepository.Dispose();
                _dynamicsReceiptRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert foundry order part to part view model
        /// </summary>
        /// <param name="foundryOrderPart"></param>
        /// <returns></returns>
        public FoundryOrderPartViewModel ConvertToPartView(FoundryOrderPart foundryOrderPart)
        {
            FoundryOrderPartViewModel model = new FoundryOrderPartViewModel();

            var _customerOrderPartRepository = new CustomerOrderRepository();
            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();

            var customerOrderPart = _customerOrderPartRepository.GetCustomerOrderPart(foundryOrderPart.CustomerOrderPartId);
            var part = _partRepository.GetPart(foundryOrderPart.PartId);
            var dynamicsPart = _dynamicsPartRepository.GetPartMaster((part != null) ? part.Number : string.Empty);
            var customerOrder = _customerOrderRepository.GetCustomerOrder((customerOrderPart != null) ? customerOrderPart.CustomerOrderId : Guid.Empty);
            var foundryOrder = _foundryOrderRepository.GetFoundryOrder(foundryOrderPart.FoundryOrderId);

            model.FoundryOrderPartId = foundryOrderPart.FoundryOrderPartId;
            model.CustomerOrderPartId = foundryOrderPart.CustomerOrderPartId;
            model.CustomerOrderId = (customerOrder != null) ? customerOrder.CustomerOrderId : Guid.Empty;
            model.PartId = foundryOrderPart.PartId;
            model.Weight = (dynamicsPart != null) ? (dynamicsPart.ITEMSHWT / 100.00m) : 0.00m;
            model.FoundryOrderId = foundryOrderPart.FoundryOrderId;
            model.OrderNumber = (foundryOrder != null && !string.IsNullOrEmpty(foundryOrder.Number)) ? foundryOrder.Number : "N/A";
            model.AvailableQuantity = foundryOrderPart.AvailableQuantity;
            model.Quantity = foundryOrderPart.Quantity;
            model.MaxQuantity = foundryOrderPart.Quantity;
            model.FoundryOrderQuantity = foundryOrderPart.Quantity;
            model.PartNumber = (part != null && !string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartDescription = (dynamicsPart != null && !string.IsNullOrEmpty(dynamicsPart.ITEMDESC)) ? dynamicsPart.ITEMDESC : "N/A";
            model.PONumber = (customerOrder != null && !string.IsNullOrEmpty(customerOrder.PONumber)) ? customerOrder.PONumber : "N/A";
            model.ShipCode = (!string.IsNullOrEmpty(foundryOrderPart.ShipCode)) ? foundryOrderPart.ShipCode : "N/A";
            model.ShipCodeNotes = (!string.IsNullOrEmpty(foundryOrderPart.ShipCodeNotes)) ? foundryOrderPart.ShipCodeNotes : "N/A";
            model.EstArrivalDate = (foundryOrderPart.EstArrivalDate != null) ? foundryOrderPart.EstArrivalDate : DateTime.MinValue;
            model.EstArrivalDateStr = (foundryOrderPart.EstArrivalDate != null) ? foundryOrderPart.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.ShipDate = (foundryOrderPart.ShipDate != null) ? foundryOrderPart.ShipDate : DateTime.MinValue;
            model.ShipDateStr = (foundryOrderPart.ShipDate != null) ? foundryOrderPart.ShipDate.Value.ToShortDateString() : "N/A";
            model.ReceiptDate = (foundryOrderPart != null && foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate : DateTime.MinValue;
            model.ReceiptDateStr = (foundryOrderPart != null && foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate.Value.ToShortDateString() : "N/A";
            model.Cost = foundryOrderPart.Cost;
            model.Price = foundryOrderPart.Price;
            model.IsScheduled = foundryOrderPart.IsScheduled;
            model.HasBeenReceived = foundryOrderPart.HasBeenReceived;
            model.ReceiptDate = (foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate : DateTime.MinValue;
            model.ReceiptDateStr = (foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate.Value.ToShortDateString() : "N/A";
            model.ReceiptQuantity = foundryOrderPart.ReceiptQuantity;

            if (_customerOrderPartRepository != null)
            {
                _customerOrderPartRepository.Dispose();
                _customerOrderPartRepository = null;
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

            if (_foundryOrderRepository != null)
            {
                _foundryOrderRepository.Dispose();
                _foundryOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert foundry order part view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FoundryOrderPart ConvertToDomain(FoundryOrderPartViewModel model)
        {
            FoundryOrderPart part = new FoundryOrderPart();

            var _customerOrderRepository = new CustomerOrderRepository();

            var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart(model.CustomerOrderPartId);

            part.FoundryOrderPartId = model.FoundryOrderPartId;
            part.ProjectPartId = (customerOrderPart != null) ? customerOrderPart.ProjectPartId : null;
            part.PartId = (customerOrderPart != null) ? customerOrderPart.PartId : null;
            part.CustomerOrderPartId = model.CustomerOrderPartId;
            part.AvailableQuantity = model.AvailableQuantity;
            part.Quantity = model.Quantity;
            part.EstArrivalDate = model.EstArrivalDate;
            part.ShipDate = model.ShipDate;
            part.ShipCode = model.ShipCode;
            part.ShipCodeNotes = model.ShipCodeNotes;
            part.Price = model.Price;
            part.Cost = model.Cost;
            part.IsScheduled = model.IsScheduled;
            part.HasBeenReceived = model.HasBeenReceived;
            part.ReceiptDate = model.ReceiptDate;
            part.ReceiptQuantity = model.ReceiptQuantity;

            if (_customerOrderRepository != null)
            {
                _customerOrderRepository.Dispose();
                _customerOrderRepository = null;
            }

            return part;
        }
    }
}