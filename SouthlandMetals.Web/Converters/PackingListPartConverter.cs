using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;

namespace SouthlandMetals.Web.Converters
{
    public class PackingListPartConverter
    {
        /// <summary>
        /// convert packlingList part to view model
        /// </summary>
        /// <param name="packingListPart"></param>
        /// <returns></returns>
        public PackingListPartViewModel ConvertToView(PackingListPart packingListPart)
        {
            PackingListPartViewModel model = new PackingListPartViewModel();

            var _partRepository = new PartRepository();
            var _customerOrderRepository = new CustomerOrderRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();

            var part = _partRepository.GetPart(packingListPart.PartNumber);
            var dynamicsPart = _dynamicsPartRepository.GetPartMaster((part != null) ? part.Number : null);
            var customerOrder = _customerOrderRepository.GetCustomerOrder(packingListPart.PONumber);

            model.PackingListPartId = packingListPart.PackingListPartId;
            model.PackingListId = packingListPart.PackingListId;
            model.PartId = (part != null) ? part.PartId : Guid.Empty;
            model.ShipCode = (!string.IsNullOrEmpty(packingListPart.ShipCode)) ? packingListPart.ShipCode : "N/A";
            model.PartNumber = (!string.IsNullOrEmpty(packingListPart.PartNumber)) ? packingListPart.PartNumber : "N/A";
            model.PartWeight = (dynamicsPart != null) ? (dynamicsPart.ITEMSHWT / 100.00m) : 0.00m;
            model.PartQuantity = packingListPart.PalletQuantity;
            model.PalletNumber = (!string.IsNullOrEmpty(packingListPart.PalletNumber)) ? packingListPart.PalletNumber : "N/A";
            model.PalletQuantity = packingListPart.PalletQuantity;
            model.PalletWeight = packingListPart.PalletWeight;
            model.PalletTotal = packingListPart.PalletTotal;
            model.TotalPalletQuantity = packingListPart.TotalPalletQuantity;
            model.PONumber = (!string.IsNullOrEmpty(packingListPart.PONumber)) ? packingListPart.PONumber : "N/A";
            model.CustomerOrderId = (customerOrder != null) ? customerOrder.CustomerOrderId : Guid.Empty;
            model.InvoiceNumber = (!string.IsNullOrEmpty(packingListPart.InvoiceNumber)) ? packingListPart.InvoiceNumber : "N/A";

            if (_partRepository != null)
            {
                _partRepository.Dispose();
                _partRepository = null;
            }

            if (_customerOrderRepository != null)
            {
                _customerOrderRepository.Dispose();
                _customerOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert container part to packlingList part view model
        /// </summary>
        /// <param name="containerPart"></param>
        /// <returns></returns>
        public PackingListPartViewModel ConvertToView(ContainerPart containerPart)
        {
            PackingListPartViewModel model = new PackingListPartViewModel();

            var _foundryOrderRepository = new FoundryOrderRepository();
            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _customerOrderRepository = new CustomerOrderRepository();

            var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderPart(containerPart.FoundryOrderPartId);
            var part = _partRepository.GetPart(foundryOrderPart.PartId);
            var dynamicsPart = _dynamicsPartRepository.GetPartMaster((part != null) ? part.Number : null);
            var customerOrderPart = _customerOrderRepository.GetCustomerOrderPart(foundryOrderPart.CustomerOrderPartId);
            var customerOrder = _customerOrderRepository.GetCustomerOrder(customerOrderPart.CustomerOrderId);

            model.ShipCode = foundryOrderPart.ShipCode;
            model.PartNumber = (part != null && !string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartWeight = (dynamicsPart != null) ? (dynamicsPart.ITEMSHWT / 100.00m) : 0.00m;
            model.PartQuantity = containerPart.Quantity;
            model.PalletNumber = (!string.IsNullOrEmpty(containerPart.PalletNumber)) ? containerPart.PalletNumber : "N/A";
            model.PONumber = (customerOrder != null && !string.IsNullOrEmpty(customerOrder.PONumber)) ? customerOrder.PONumber : "N/A";
            model.PalletQuantity = containerPart.Quantity;
            model.InvoiceNumber = "Will fill in later";

            if (_foundryOrderRepository != null)
            {
                _foundryOrderRepository.Dispose();
                _foundryOrderRepository = null;
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
        /// convert packlingList part view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PackingListPart ConvertToDomain(PackingListPartViewModel model)
        {
            PackingListPart part = new PackingListPart();

            part.PackingListPartId = model.PackingListPartId;
            part.PackingListId = model.PackingListId;
            part.ShipCode = model.ShipCode;
            part.PartNumber = model.PartNumber;
            part.PalletNumber = model.PalletNumber;
            part.PalletQuantity = model.PalletQuantity;
            part.PalletWeight = model.PalletWeight;
            part.PalletTotal = model.PalletTotal;
            part.TotalPalletQuantity = model.TotalPalletQuantity;
            part.PONumber = model.PONumber;
            part.InvoiceNumber = model.InvoiceNumber;

            return part;
        }
    }
}