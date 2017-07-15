using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class AccountCodeConverter
    {
        /// <summary>
        /// convert accountCode to view model
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public AccountCodeViewModel ConvertToView(AccountCode code)
        {
            AccountCodeViewModel model = new AccountCodeViewModel();

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _bucketRepository = new BucketRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(code.CustomerId);
            var bucket = _bucketRepository.GetBucket(code.AccountCodeId);

            model.AccountCodeId = code.AccountCodeId;
            model.Description = (!string.IsNullOrEmpty(code.Description)) ? code.Description : "N/A";
            model.CustomerId = code.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.BucketName = (bucket != null && !string.IsNullOrEmpty(bucket.Name)) ? bucket.Name : "N/A";

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }
            if (_bucketRepository != null)
            {
                _bucketRepository.Dispose();
                _bucketRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert AccountCode view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AccountCode ConvertToDomain(AccountCodeViewModel model)
        {
            AccountCode code = new AccountCode();

            code.AccountCodeId = model.AccountCodeId;
            code.Description = model.Description;
            code.CustomerId = model.CustomerId;

            return code;
        }
    }
}