using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Helpers;

namespace SouthlandMetals.Web.Converters
{
    public class CustomerAddressConverter
    {
        /// <summary>
        /// convert customer address to customer view model
        /// </summary>
        /// <param name="customerAddress"></param>
        /// <returns></returns>
        public CustomerViewModel ConvertToView(RM00102_CustomerAddress customerAddress)
        {
            CustomerViewModel model = new CustomerViewModel();

            var _customerDynamicsRepository = new CustomerDynamicsRepository();
            var _stateRepository = new StateRepository();
            var _countryRepository = new CountryRepository();
            var _siteDynamicsRepository = new SiteDynamicsRepository();

            var dynamicsCustomer = _customerDynamicsRepository.GetCustomer(customerAddress.CUSTNMBR);
            var state = _stateRepository.GetState(customerAddress.STATE);
            var country = _countryRepository.GetCountry(customerAddress.COUNTRY);
            var dynamicsSite = _siteDynamicsRepository.GetSite(customerAddress.LOCNCODE);

            var type = (dynamicsCustomer != null && customerAddress.ADRSCODE.Replace(" ", string.Empty).Equals(dynamicsCustomer.PRBTADCD.Replace(" ", string.Empty))) ?
                        "Bill To Address" : (dynamicsCustomer != null && customerAddress.ADRSCODE.Replace(" ", string.Empty).Equals(dynamicsCustomer.PRSTADCD.Replace(" ", string.Empty))) ?
                        "Ship To Address" : (dynamicsCustomer != null && customerAddress.ADRSCODE.Replace(" ", string.Empty).Equals(dynamicsCustomer.STADDRCD.Replace(" ", string.Empty))) ?
                        "Standard Address" : "N/A";

            model.CustomerAddressId = customerAddress.ADRSCODE;
            model.CustomerNumber = customerAddress.CUSTNMBR;
            model.ShortName = (dynamicsCustomer != null && !string.IsNullOrEmpty(dynamicsCustomer.SHRTNAME)) ? dynamicsCustomer.SHRTNAME : "N/A";
            model.AddressCode = customerAddress.ADRSCODE;
            model.AddressType = type;
            model.ContactName = (!string.IsNullOrEmpty(customerAddress.CNTCPRSN.Replace(" ", string.Empty))) ? customerAddress.CNTCPRSN : "N/A";
            model.ContactPhone = FormattingManager.FormatPhone(customerAddress.PHONE1);
            model.FaxNumber = FormattingManager.FormatPhone(customerAddress.FAX);
            model.Address1 = (!string.IsNullOrEmpty(customerAddress.ADDRESS1.Replace(" ", string.Empty))) ? customerAddress.ADDRESS1 : "N/A";
            model.Address2 = (!string.IsNullOrEmpty(customerAddress.ADDRESS2.Replace(" ", string.Empty))) ? customerAddress.ADDRESS2 : "N/A";
            model.Address3 = (!string.IsNullOrEmpty(customerAddress.ADDRESS3.Replace(" ", string.Empty))) ? customerAddress.ADDRESS3 : "N/A";
            model.City = (!string.IsNullOrEmpty(customerAddress.CITY.Replace(" ", string.Empty))) ? customerAddress.CITY : "N/A";
            model.StateName = (state != null && !string.IsNullOrEmpty(state.Name)) ? state.Name : "N/A";
            model.PostalCode = (!string.IsNullOrEmpty(customerAddress.ZIP.Replace(" ", string.Empty))) ? customerAddress.ZIP : "N/A";
            model.CountryName = (country != null && !string.IsNullOrEmpty(country.Name)) ? country.Name : "N/A";
            model.ShippingMethod = (!string.IsNullOrEmpty(customerAddress.SHIPMTHD.Replace(" ", string.Empty))) ? customerAddress.SHIPMTHD : "N/A";
            model.SiteDescription = (dynamicsSite != null && !string.IsNullOrEmpty(dynamicsSite.LOCNDSCR.Replace(" ", string.Empty))) ? dynamicsSite.LOCNDSCR : "N/A";

            if (_customerDynamicsRepository != null)
            {
                _customerDynamicsRepository.Dispose();
                _customerDynamicsRepository = null;
            }

            if (_stateRepository != null)
            {
                _stateRepository.Dispose();
                _stateRepository = null;
            }

            if (_countryRepository != null)
            {
                _countryRepository.Dispose();
                _countryRepository = null;
            }

            if (_siteDynamicsRepository != null)
            {
                _siteDynamicsRepository.Dispose();
                _siteDynamicsRepository = null;
            }

            return model;
        }
    }
}