using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class OrderTermConverter
    {
        /// <summary>
        /// convert order term to customer view model
        /// </summary>
        /// <param name="orderTerm"></param>
        /// <returns></returns>
        public CustomerViewModel ConvertToCustomerView(OrderTerm orderTerm)
        {
            CustomerViewModel model = new CustomerViewModel();
     
            var _customerDynamicsRepository = new CustomerDynamicsRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(orderTerm.CustomerId);

            model.OrderTermId = orderTerm.OrderTermId;
            model.CustomerId = orderTerm.CustomerId;
            model.CustomerName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.OrderTermsDescription = (!string.IsNullOrEmpty(orderTerm.Description)) ? orderTerm.Description : "N/A";

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert order term to view model
        /// </summary>
        /// <param name="orderTerm"></param>
        /// <returns></returns>
        public OrderTermViewModel ConvertToView(OrderTerm orderTerm)
        {
            OrderTermViewModel model = new OrderTermViewModel();

            model.OrderTermId = orderTerm.OrderTermId;
            model.CustomerId = orderTerm.CustomerId;
            model.OrderTermsDescription = (!string.IsNullOrEmpty(orderTerm.Description)) ? orderTerm.Description : "N/A";

            return model;
        }

        /// <summary>
        /// convert order term view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OrderTerm ConvertToDomain(OrderTermViewModel model)
        {
            OrderTerm orderTerm = new OrderTerm();

            orderTerm.OrderTermId = model.OrderTermId;
            orderTerm.CustomerId = model.CustomerId;
            orderTerm.Description = model.OrderTermsDescription;

            return orderTerm;
        }
    }
}