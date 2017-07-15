using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class ContainerPartConverter
    {
        /// <summary>
        /// convert containerPart to view model
        /// </summary>
        /// <param name="containerPart"></param>
        /// <returns></returns>
        public ContainerPartViewModel ConvertToView(ContainerPart containerPart)
        {
            ContainerPartViewModel model = new ContainerPartViewModel();

            var _containerRepository = new ContainerRepository();
            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();

            var container = _containerRepository.GetContainer(containerPart.ContainerId);
            var foundryOrderPart = _foundryOrderRepository.GetFoundryOrderParts().FirstOrDefault(x => x.FoundryOrderPartId == containerPart.FoundryOrderPartId);
            var part = _partRepository.GetPart((foundryOrderPart != null) ? foundryOrderPart.PartId : Guid.Empty);
            var dynamicsPart = _dynamicsPartRepository.GetPartMaster(part != null && !string.IsNullOrEmpty(part.Number) ? part.Number : null);
            var foundryOrder = _foundryOrderRepository.GetFoundryOrder((foundryOrderPart != null) ? foundryOrderPart.FoundryOrderId : Guid.Empty);

            model.FoundryOrderPartId = containerPart.FoundryOrderPartId;
            model.ContainerPartId = containerPart.ContainerPartId;
            model.ContainerId = containerPart.ContainerId;
            model.PartId = (part != null) ? part.PartId : Guid.Empty;
            model.ContainerNumber = (container != null) ? container.Number : "N/A";
            model.PartNumber = (part != null && !string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.FoundryOrderId = (foundryOrder != null) ? foundryOrder.FoundryOrderId : Guid.Empty;
            model.OrderNumber = (foundryOrder != null && !string.IsNullOrEmpty(foundryOrder.Number)) ? foundryOrder.Number : "N/A";
            model.PalletNumber = (!string.IsNullOrEmpty(containerPart.PalletNumber)) ? containerPart.PalletNumber : "N/A";
            model.Quantity = containerPart.Quantity;
            model.Weight = (dynamicsPart != null) ? (dynamicsPart.ITEMSHWT / 100.00m) : 0.00m;
            model.Cost = foundryOrderPart.Cost;
            model.Price = foundryOrderPart.Price;
            model.AvailableQuantity = (foundryOrderPart != null) ? foundryOrderPart.Quantity : 0;
            model.ShipCode = (foundryOrderPart != null && !string.IsNullOrEmpty(foundryOrderPart.ShipCode)) ? foundryOrderPart.ShipCode : "N/A";

            if (_containerRepository != null)
            {
                _containerRepository.Dispose();
                _containerRepository = null;
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

            if (_foundryOrderRepository != null)
            {
                _foundryOrderRepository.Dispose();
                _foundryOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert containerPart view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ContainerPart ConvertToDomain(ContainerPartViewModel model)
        {
            ContainerPart containerPart = new ContainerPart();

            containerPart.ContainerPartId = model.ContainerPartId;
            containerPart.FoundryOrderPartId = model.FoundryOrderPartId;
            containerPart.ContainerId = model.ContainerId ?? Guid.Empty;
            containerPart.Quantity = model.Quantity;
            containerPart.PalletNumber = model.PalletNumber;

            return containerPart;
        }
    }
}