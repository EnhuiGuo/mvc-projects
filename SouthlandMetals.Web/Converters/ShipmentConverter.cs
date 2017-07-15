using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class ShipmentConverter
    {
        /// <summary>
        /// convert shipment to view model
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public ShipmentViewModel ConvertToView(Shipment shipment)
        {
            ShipmentViewModel model = new ShipmentViewModel();

            var _vesselRepository = new VesselRepository();
            var _portRepository = new PortRepository();
            var _bolRepository = new BillOfLadingRepository();

            var vessel = _vesselRepository.GetVessel(shipment.VesselId);
            var port = _portRepository.GetPort(shipment.PortId);
            var bols = _bolRepository.GetBillOfLadings().Where(x => x.ShipmentId == shipment.ShipmentId).ToList();

            model.ShipmentId = shipment.ShipmentId;
            model.CarrierId = shipment.CarrierId;
            model.VesselId = shipment.VesselId;
            model.PortId = shipment.PortId;
            model.DepartureDate = shipment.DepartureDate;
            model.EstArrivalDate = (shipment.EstArrivalDate != null) ? shipment.EstArrivalDate : DateTime.MinValue;
            model.ShipmentNotes = (!string.IsNullOrEmpty(shipment.Notes)) ? shipment.Notes : "N/A";
            model.VesselName = (vessel != null && !string.IsNullOrEmpty(vessel.Name)) ? vessel.Name : "N/A";
            model.PortName = (port != null && !string.IsNullOrEmpty(port.Name)) ? port.Name : "N/A";
            model.DepartureDateStr =  shipment.DepartureDate.ToShortDateString();
            model.EstArrivalDateStr = (shipment.EstArrivalDate != null) ? shipment.EstArrivalDate.Value.ToShortDateString() : "N/A";
            model.IsComplete = shipment.IsComplete;
            model.CompletedDate = (shipment.CompletedDate != null) ? shipment.CompletedDate : DateTime.MinValue;
            model.CompletedDateStr = (shipment.CompletedDate != null) ? shipment.CompletedDate.Value.ToShortDateString() : "N/A";
            model.CreatedDate = (shipment.CreatedDate != null) ? shipment.CreatedDate : DateTime.MinValue;

            if (bols != null && bols.Count > 0)
            {
                model.BillsOfLading = new List<BillOfLadingViewModel>();
                foreach (var bol in bols)
                {
                    BillOfLadingViewModel convertedModel = new BillOfLadingConverter().ConvertToView(bol);

                    model.BillsOfLading.Add(convertedModel);
                }
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
            if (_bolRepository != null)
            {
                _bolRepository.Dispose();
                _bolRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert shipment view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Shipment ConvertToDomain(ShipmentViewModel model)
        {
            Shipment shipment = new Shipment();

            shipment.ShipmentId = model.ShipmentId;
            shipment.CarrierId = model.CarrierId;
            shipment.VesselId = model.VesselId;
            shipment.PortId = model.PortId;
            shipment.DepartureDate = model.DepartureDate;
            shipment.EstArrivalDate = model.EstArrivalDate;
            shipment.Notes = model.ShipmentNotes;
            shipment.IsComplete = model.IsComplete;
            shipment.CompletedDate = model.CompletedDate;

            return shipment;
        }
    }
}