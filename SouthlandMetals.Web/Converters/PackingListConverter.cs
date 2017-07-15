using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class PackingListConverter
    {
        /// <summary>
        /// convert packlingList to list model
        /// </summary>
        /// <param name="packingList"></param>
        /// <returns></returns>
        public PackingListViewModel ConvertToListView(PackingList packingList)
        {
            PackingListViewModel model = new PackingListViewModel();

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            var _stateRepository = new StateRepository();
            var _carrierRepository = new CarrierRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(packingList.CustomerId);
            var dynamicsCustomerAddress = _customerAddressDynamicsRepository.GetCustomerAddress(packingList.CustomerAddressId);
            var state = _stateRepository.GetState((dynamicsCustomerAddress != null && !string.IsNullOrEmpty(dynamicsCustomerAddress.STATE)) ? dynamicsCustomerAddress.STATE : string.Empty);
            var stateName = (state != null && !string.IsNullOrEmpty(state.Name)) ? ", " + state.Name : string.Empty;
            var carrier = _carrierRepository.GetCarrier(packingList.CarrierId);

            model.PackingListId = packingList.PackingListId;
            model.CreatedDate = (packingList.CreatedDate != null) ? packingList.CreatedDate : DateTime.MinValue;
            model.CreatedDateStr = (packingList.CreatedDate != null) ? packingList.CreatedDate.Value.ToShortDateString() : "N/A";
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.CustomerAddress = (dynamicsCustomerAddress != null) ? dynamicsCustomerAddress.ADDRESS1 + " " + 
                                                                        dynamicsCustomerAddress.CITY + ", " + stateName : "N/A";
            model.ShipDate = packingList.ShipDate;
            model.ShipDateStr = packingList.ShipDate.ToShortDateString();
            model.CarrierName = (carrier != null && !string.IsNullOrEmpty(carrier.Name)) ? carrier.Name : "N/A";
            model.TrackingNumber = (!string.IsNullOrEmpty(packingList.TrackingNumber)) ? packingList.TrackingNumber : "N/A";
            model.Notes = (!string.IsNullOrEmpty(packingList.TrackingNumber)) ? packingList.Notes : "N/A";
            model.IsClosed = packingList.IsClosed;
            model.ClosedDate = (packingList.ClosedDate != null) ? packingList.ClosedDate : DateTime.MinValue;
            model.ClosedDateStr = (packingList.ClosedDate != null) ? packingList.ClosedDate.Value.ToShortDateString() : "N/A";

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

            if (_carrierRepository != null)
            {
                _carrierRepository.Dispose();
                _carrierRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert packing list to view model
        /// </summary>
        /// <param name="packingList"></param>
        /// <returns></returns>
        public PackingListViewModel ConvertToView(PackingList packingList)
        {
            PackingListViewModel model = new PackingListViewModel();

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _customerAddressDynamicsRepository = new CustomerAddressDynamicsRepository();
            var _stateRepository = new StateRepository();
            var _carrierRepository = new CarrierRepository();
            var _packingListRepository = new PackingListRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(packingList.CustomerId);
            var dynamicsCustomerAddress = _customerAddressDynamicsRepository.GetCustomerAddress(packingList.CustomerAddressId);
            var state = _stateRepository.GetState((dynamicsCustomerAddress != null && !string.IsNullOrEmpty(dynamicsCustomerAddress.STATE)) ? dynamicsCustomerAddress.STATE : string.Empty);
            var stateName = (state != null && !string.IsNullOrEmpty(state.Name)) ? ", " + state.Name : string.Empty;
            var carrier = _carrierRepository.GetCarrier(packingList.CarrierId);

            model.PackingListId = packingList.PackingListId;
            model.CustomerId = packingList.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.CustomerAddressId = packingList.CustomerAddressId;
            model.CustomerAddress = (dynamicsCustomerAddress != null) ? dynamicsCustomerAddress.ADDRESS1 + " " +
                                                                       dynamicsCustomerAddress.CITY + ", " + stateName : "N/A";
            model.ShipDate = packingList.ShipDate;
            model.ShipDateStr = packingList.ShipDate.ToShortDateString();
            model.Freight = packingList.Freight;
            model.CarrierId = packingList.CarrierId;
            model.CarrierName = (carrier != null && !string.IsNullOrEmpty(carrier.Name)) ? carrier.Name : "N/A";
            model.TrailerNumber = (!string.IsNullOrEmpty(packingList.TrailerNumber)) ? packingList.TrailerNumber : "N/A";
            model.TrackingNumber = (!string.IsNullOrEmpty(packingList.TrackingNumber)) ? packingList.TrackingNumber : "N/A";
            model.Notes = (!string.IsNullOrEmpty(packingList.Notes)) ? packingList.Notes : "N/A";
            model.NetWeight = packingList.NetWeight;
            model.GrossWeight = packingList.GrossWeight;
            model.DeliveryDate = (packingList.DeliveryDate != null) ? packingList.DeliveryDate : DateTime.MinValue; ;
            model.DeliveryDateStr = (packingList.DeliveryDate != null) ? packingList.DeliveryDate.Value.ToShortDateString() : "N/A";
            model.IsClosed = packingList.IsClosed;
            model.ClosedDate = (packingList.ClosedDate != null) ? packingList.ClosedDate : DateTime.MinValue;
            model.ClosedDateStr = (packingList.ClosedDate != null) ? packingList.ClosedDate.Value.ToShortDateString() : "N/A";
            model.CreatedDate = (packingList.CreatedDate != null) ? packingList.CreatedDate : DateTime.MinValue;
            model.CreatedDateStr = (packingList.CreatedDate != null) ? packingList.CreatedDate.Value.ToShortDateString() : "N/A";

            var parts = _packingListRepository.GetPackingListPartsByPackingList(packingList.PackingListId);

            if (parts != null && parts.Count > 0)
            {
                var packingListParts = new List<PackingListPartViewModel>();

                foreach (var part in parts)
                {
                    PackingListPartViewModel packingListPart = new PackingListPartConverter().ConvertToView(part);
                    packingListParts.Add(packingListPart);
                }

                model.PackingListParts = packingListParts.OrderBy(x => x.ShipCode).ThenBy(y => y.PartNumber).ThenBy(z => z.PalletNumber).ToList();
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

            if (_carrierRepository != null)
            {
                _carrierRepository.Dispose();
                _carrierRepository = null;
            }

            if (_packingListRepository != null)
            {
                _packingListRepository.Dispose();
                _packingListRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert packlingList view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PackingList ConvertToDomain(PackingListViewModel model)
        {
            PackingList packingList = new PackingList();

            packingList.PackingListId = model.PackingListId;
            packingList.CustomerId = model.CustomerId;
            packingList.CustomerAddressId = model.CustomerAddressId;
            packingList.ShipDate = model.ShipDate;
            packingList.Freight = model.Freight;
            packingList.CarrierId = model.CarrierId;
            packingList.NetWeight = model.NetWeight;
            packingList.GrossWeight = model.GrossWeight;
            packingList.TrailerNumber = model.TrailerNumber;
            packingList.TrackingNumber = model.TrackingNumber;
            packingList.Notes = model.Notes;
            packingList.DeliveryDate = model.DeliveryDate;
            packingList.IsClosed = model.IsClosed;
            packingList.ClosedDate = (model.IsClosed) ? DateTime.Now : model.ClosedDate;

            if (model.PackingListParts != null && model.PackingListParts.Count() > 0)
            {
                var packingListParts = new List<PackingListPart>();

                foreach (var part in model.PackingListParts)
                {
                    PackingListPart packingListPart = new PackingListPartConverter().ConvertToDomain(part);

                    packingListParts.Add(packingListPart);
                }

                packingList.PackingListParts = packingListParts;
            }

            return packingList;
        }
    }
}