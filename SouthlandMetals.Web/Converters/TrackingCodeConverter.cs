using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class TrackingCodeConverter
    {
        /// <summary>
        /// convert tracking code to view model
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public TrackingCodeViewModel ConvertToView(TrackingCode code)
        {
            TrackingCodeViewModel model = new TrackingCodeViewModel();
       
            var _customerDynamicsRepository = new CustomerDynamicsRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(code.CustomerId);

            model.TrackingCodeId = code.TrackingCodeId;
            model.CustomerId = code.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.TrackingCodeDescription = (!string.IsNullOrEmpty(code.Code)) ? code.Code : "N/A";
            model.IsActive = code.IsActive;

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert tracking code view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TrackingCode ConvertToDomain(TrackingCodeViewModel model)
        {
            TrackingCode code = new TrackingCode();

            code.TrackingCodeId = model.TrackingCodeId;
            code.CustomerId = model.CustomerId;
            code.Code = model.TrackingCodeDescription;
            code.IsActive = model.IsActive;

            return code;
        }
    }
}