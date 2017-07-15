using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class CarrierConverter
    {
        /// <summary>
        /// convert carrier to view model
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public CarrierViewModel ConvertToView(Carrier carrier)
        {
            CarrierViewModel model = new CarrierViewModel();

            model.CarrierId = carrier.CarrierId;
            model.CarrierName = (!string.IsNullOrEmpty(carrier.Name)) ? carrier.Name : "N/A";
            model.IsActive = carrier.IsActive;

            return model;
        }

        /// <summary>
        /// convert carrier view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Carrier ConvertToDomain(CarrierViewModel model)
        {
            Carrier carrier = new Carrier();

            carrier.CarrierId = model.CarrierId;
            carrier.Name = model.CarrierName;
            carrier.IsActive = model.IsActive;

            return carrier;
        }
    }
}