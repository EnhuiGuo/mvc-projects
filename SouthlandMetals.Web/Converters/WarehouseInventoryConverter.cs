using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;

namespace SouthlandMetals.Web.Converters
{
    public class WarehouseInventoryConverter
    {
        /// <summary>
        /// convert container part to warehouse inventory list model
        /// </summary>
        /// <param name="containerPart"></param>
        /// <returns></returns>
        public WarehouseInventoryViewModel ConvertToListView(ContainerPart containerPart)
        {
            WarehouseInventoryViewModel model = new WarehouseInventoryViewModel();

            var _foundryOrderRepository = new FoundryOrderRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _containerRepository = new ContainerRepository();

            var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPart(containerPart.FoundryOrderPartId);
            var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart((foundryOrderPart != null) ? foundryOrderPart.CustomerOrderPartId : Guid.Empty);
            var part = _partRepository.GetPart((foundryOrderPart != null) ? foundryOrderPart.PartId : Guid.Empty);
            var dynamicsPart = _dynamicsPartRepository.GetPartMaster((part != null) ? part.Number : string.Empty);
            var container = _containerRepository.GetContainer(containerPart.ContainerId);

            model.CustomerId = (part != null) ? part.CustomerId : "N/A";
            model.ShipCode = (foundryOrderPart != null) ? foundryOrderPart.ShipCode : "N/A";
            model.PartNumber = (part != null && !string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartWeight = (dynamicsPart != null) ? (dynamicsPart.ITEMSHWT / 100.00m) : 0.00m;
            model.PalletNumber = (!string.IsNullOrEmpty(part.Number)) ? containerPart.PalletNumber : "N/A";
            model.PalletQuantity = containerPart.Quantity;
            model.TotalQuantity = containerPart.Quantity;
            model.ContainerNumber = (!string.IsNullOrEmpty(part.Number)) ? container.Number : "N/A";
            model.PONumber = (customerOrderPart != null && !string.IsNullOrEmpty(customerOrderPart.CustomerOrder.PONumber)) ? customerOrderPart.CustomerOrder.PONumber : "N/A";
            model.WarehouseDate = (foundryOrderPart != null) ? foundryOrderPart.ReceiptDate : DateTime.MinValue;
            model.WarehouseDateStr = (foundryOrderPart != null && foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate.Value.ToShortDateString() : "N/A";
            model.SixtyDaysDate = (foundryOrderPart != null && foundryOrderPart.ReceiptDate != null) ? foundryOrderPart.ReceiptDate.Value.AddDays(60) : DateTime.MinValue;
            model.SixtyDaysDateStr = (model.SixtyDaysDate != null) ? model.SixtyDaysDate.Value.ToShortDateString() : "N/A";

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

            if (_containerRepository != null)
            {
                _containerRepository.Dispose();
                _containerRepository = null;
            }

            return model;
        }
    }
}