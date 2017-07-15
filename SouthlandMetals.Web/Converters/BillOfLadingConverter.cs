using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class BillOfLadingConverter
    {
        /// <summary>
        /// convert bol to list model
        /// </summary>
        /// <param name="bol"></param>
        /// <returns></returns>
        public BillOfLadingViewModel ConvertToListView(BillOfLading bol)
        {
            BillOfLadingViewModel model = new BillOfLadingViewModel();

            var _shipmentRepository = new ShipmentRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _vesselRepository = new VesselRepository();
            var _portRepository = new PortRepository();

            var shipment = _shipmentRepository.GetShipment(bol.ShipmentId);
            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(bol.FoundryId);
            var vessel = _vesselRepository.GetVessel((shipment != null) ? shipment.VesselId : Guid.Empty);
            var port = _portRepository.GetPort((shipment != null) ? shipment.PortId : Guid.Empty);

            model.BillOfLadingId = bol.BillOfLadingId;
            model.ShipmentId = bol.ShipmentId;
            model.BolNumber = (!string.IsNullOrEmpty(bol.Number)) ? bol.Number : "N/A";
            model.FoundryName = (dynamicsFoundry != null) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.Description = (!string.IsNullOrEmpty(bol.Description)) ? bol.Description : "N/A";
            model.VesselName = (vessel != null && !string.IsNullOrEmpty(vessel.Name)) ? vessel.Name : "N/A";
            model.PortName = (port != null && !string.IsNullOrEmpty(port.Name)) ? port.Name : "N/A";
            model.DepartureDate = (shipment != null && shipment.DepartureDate != null) ? shipment.DepartureDate : DateTime.MinValue ;
            model.DepartureDateStr = (shipment != null) ? shipment.DepartureDate.ToShortDateString() : "N/A";
            model.EstArrivalDate = (shipment != null && shipment.EstArrivalDate != null) ? shipment.EstArrivalDate : DateTime.MinValue;
            model.EstArrivalDateStr = (shipment != null) ? shipment.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.HasBeenAnalyzed = bol.HasBeenAnalyzed;
            model.CreatedDate = bol.CreatedDate;

            if (_shipmentRepository != null)
            {
                _shipmentRepository.Dispose();
                _shipmentRepository = null;
            }
            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
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

            return model;
        }

        /// <summary>
        /// convert bol to view model
        /// </summary>
        /// <param name="bol"></param>
        /// <returns></returns>
        public BillOfLadingViewModel ConvertToView(BillOfLading bol)
        {
            BillOfLadingViewModel model = new BillOfLadingViewModel();

            var _bolRepository = new BillOfLadingRepository();
            var _foundryDynamicsRepository = new FoundryDynamicsRepository();
            var _foundryInvoiceRepository = new FoundryInvoiceRepository();
            var _containerRepository = new ContainerRepository();
            var _foundryOrderRepository = new FoundryOrderRepository();

            var dynamicsFoundry = _foundryDynamicsRepository.GetFoundry(bol.FoundryId);
            var foundryInvoice = _foundryInvoiceRepository.GetFoundryInvoiceByBillOfLading(bol.BillOfLadingId);
            var containers = _containerRepository.GetContainers().Where(x => x.BillOfLadingId == bol.BillOfLadingId).ToList();

            model.BillOfLadingId = bol.BillOfLadingId;
            model.ShipmentId = bol.ShipmentId;
            model.FoundryId = bol.FoundryId;
            model.BolNumber = (!string.IsNullOrEmpty(bol.Number)) ? bol.Number : "N/A";
            model.BolDate = bol.BolDate;
            model.BolDateStr = (bol.BolDate != null) ? bol.BolDate.ToShortDateString() : "N/A";
            model.FoundryName = (dynamicsFoundry != null && !string.IsNullOrEmpty(dynamicsFoundry.VENDSHNM)) ? dynamicsFoundry.VENDSHNM : "N/A";
            model.Description = (!string.IsNullOrEmpty(bol.Description)) ? bol.Description : "N/A";
            model.HasLcl = bol.HasLcl;
            model.HasDoorMove = bol.HasDoorMove;
            model.HasArrivalNotice = bol.HasArrivalNotice;
            model.HasOriginalDocuments = bol.HasOriginalDocuments;
            model.PalletCount = bol.PalletCount;
            model.GrossWeight = bol.GrossWeight;
            model.NetWeight = bol.NetWeight;
            model.BolNotes = (!string.IsNullOrEmpty(bol.Notes)) ? bol.Notes : "N/A";
            model.WireInstructions = (!string.IsNullOrEmpty(bol.WireInstructions)) ? bol.WireInstructions : "N/A";
            model.CustomsNumber = (!string.IsNullOrEmpty(bol.CustomsNumber)) ? bol.CustomsNumber : "N/A";
            model.IsCustomsLiquidated = bol.IsCustomsLiquidated;
            model.FoundryInvoiceId = (foundryInvoice != null) ? foundryInvoice.FoundryInvoiceId : Guid.Empty;
            model.BillOfLadingId = (foundryInvoice != null) ? foundryInvoice.FoundryInvoiceId : Guid.Empty;
            model.InvoiceNumber = (foundryInvoice != null && !string.IsNullOrEmpty(foundryInvoice.Number)) ? foundryInvoice.Number : "N/A";
            model.InvoiceTotal = (foundryInvoice != null) ? foundryInvoice.Amount : 0.00m;
            model.ScheduledDate = (foundryInvoice != null && foundryInvoice.ScheduledPaymentDate != null) ? foundryInvoice.ScheduledPaymentDate : DateTime.MinValue;
            model.ScheduledDateStr = (foundryInvoice != null && foundryInvoice.ScheduledPaymentDate != null) ? foundryInvoice.ScheduledPaymentDate.Value.ToShortDateString() : "N/A";
            model.ActualDate = (foundryInvoice != null && foundryInvoice.ActualPaymentDate != null) ? foundryInvoice.ActualPaymentDate : DateTime.MinValue;
            model.ActualDateStr = (foundryInvoice != null && foundryInvoice.ActualPaymentDate != null) ? foundryInvoice.ActualPaymentDate.Value.ToShortDateString() : "N/A";
            model.InvoiceNotes = (foundryInvoice != null && !string.IsNullOrEmpty(foundryInvoice.Notes)) ? foundryInvoice.Notes : "N/A";
            model.HasBeenAnalyzed = bol.HasBeenAnalyzed;

            if (containers != null && containers.Count > 0)
            {
                model.Containers = new List<ContainerViewModel>();

                foreach (var container in containers)
                {
                    ContainerViewModel convertedModel = new ContainerConverter().ConvertToView(container);

                    model.Containers.Add(convertedModel);
                }

                model.ContainerParts = new List<ContainerPartViewModel>();
                model.Pallets = new List<PalletViewModel>();
                model.PurchaseOrders = new List<FoundryOrderViewModel>();

                foreach (var container in model.Containers)
                {
                    if (container.ContainerParts != null)
                    {
                        foreach (var containerPart in container.ContainerParts)
                        {
                            model.ContainerParts.Add(containerPart);
                            model.Pallets.Add(new PalletViewModel() { PalletNumber = containerPart.PalletNumber });

                            var foundryOrder = _foundryOrderRepository.GetFoundryOrder(containerPart.FoundryOrderId);

                            if (foundryOrder != null)
                            {
                                FoundryOrderViewModel convertedModel = new FoundryOrderConverter().ConvertToView(foundryOrder);

                                model.PurchaseOrders.Add(convertedModel);
                            }
                        }
                    }
                }

                if (model.PurchaseOrders != null && model.PurchaseOrders.Count > 0)
                {
                    model.PurchaseOrders = model.PurchaseOrders.GroupBy(x => x.FoundryOrderId).Select(y => y.First()).ToList();
                }

                if (model.Pallets != null && model.Pallets.Count > 0)
                {
                    model.Pallets = model.Pallets.Distinct().ToList();
                }
            }

            model.FoundryInvoice = new FoundryInvoiceConverter().ConvertToView(foundryInvoice);

            if (_bolRepository != null)
            {
                _bolRepository.Dispose();
                _bolRepository = null;
            }

            if (_foundryDynamicsRepository != null)
            {
                _foundryDynamicsRepository.Dispose();
                _foundryDynamicsRepository = null;
            }

            if (_foundryInvoiceRepository != null)
            {
                _foundryInvoiceRepository.Dispose();
                _foundryInvoiceRepository = null;
            }

            if (_containerRepository != null)
            {
                _containerRepository.Dispose();
                _containerRepository = null;
            }

            if (_foundryOrderRepository != null)
            {
                _foundryOrderRepository.Dispose();
                _foundryOrderRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert Bol view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BillOfLading ConvertToDomain(BillOfLadingViewModel model)
        {
            BillOfLading bol = new BillOfLading();

            bol.BillOfLadingId = model.BillOfLadingId;
            bol.ShipmentId = model.ShipmentId;
            bol.Number = model.BolNumber;
            bol.BolDate = model.BolDate;
            bol.FoundryId = model.FoundryId;
            bol.Description = model.Description;
            bol.HasLcl = model.HasLcl;
            bol.HasDoorMove = model.HasDoorMove;
            bol.HasArrivalNotice = model.HasArrivalNotice;
            bol.HasOriginalDocuments = model.HasOriginalDocuments;
            bol.PalletCount = model.PalletCount;
            bol.GrossWeight = model.GrossWeight;
            bol.NetWeight = model.NetWeight;
            bol.Notes = model.BolNotes;
            bol.WireInstructions = model.WireInstructions;
            bol.CustomsNumber = model.CustomsNumber;
            bol.IsCustomsLiquidated = model.IsCustomsLiquidated;
            bol.HasBeenAnalyzed = model.HasBeenAnalyzed;

            var containers = new List<Container>();

            if (model.Containers != null && model.Containers.Count > 0)
            {
                foreach (var container in model.Containers)
                {
                    Container convertedModel = new ContainerConverter().ConvertToDomain(container);
                    containers.Add(convertedModel);
                }

                bol.Containers = containers;
            }

            FoundryInvoice foundryInvoice = new FoundryInvoiceConverter().ConvertToDomain(model.FoundryInvoice);

            bol.FoundryInvoice = foundryInvoice;

            return bol;
        }
    }
}