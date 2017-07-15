using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Helpers;

namespace SouthlandMetals.Web.Converters
{
    public class FoundryConverter
    {
        /// <summary>
        /// convert foundry to list model
        /// </summary>
        /// <param name="foundry"></param>
        /// <returns></returns>
        public FoundryViewModel ConvertToListView(PM00200_Foundry foundry)
        {
            FoundryViewModel model = new FoundryViewModel();

            model.FoundryId = foundry.VENDORID;
            model.VendorId = foundry.VENDORID;
            model.ShortName = (!string.IsNullOrEmpty(foundry.VENDSHNM.Replace(" ", string.Empty))) ? foundry.VENDSHNM : "N/A";
            model.ContactName = (!string.IsNullOrEmpty(foundry.VNDCNTCT.Replace(" ", string.Empty))) ? foundry.VNDCNTCT : "N/A";
            model.IsActive = foundry.VENDSTTS != 1 ? false : true;

            return model;
        }

        /// <summary>
        /// convert foundry to view model
        /// </summary>
        /// <param name="foundry"></param>
        /// <returns></returns>
        public FoundryViewModel ConvertToView(PM00200_Foundry foundry)
        {
            FoundryViewModel model = new FoundryViewModel();

            var _paymentTermRepository = new PaymentTermRepository();
            var _shipmentTermRepository = new ShipmentTermRepository();
            var _stateRepository = new StateRepository();
            var _countryRepository = new CountryRepository();

            var state = _stateRepository.GetState(foundry.STATE);
            var country = _countryRepository.GetCountry(foundry.COUNTRY);
            var paymentTerm = _paymentTermRepository.GetPaymentTerm(foundry.PYMTRMID);

            model.FoundryId = foundry.VENDORID;
            model.VendorId = foundry.VENDORID;
            model.ShortName = (!string.IsNullOrEmpty(foundry.VENDSHNM.Replace(" ", string.Empty))) ? foundry.VENDSHNM : "N/A";
            model.ContactName = (!string.IsNullOrEmpty(foundry.VNDCNTCT.Replace(" ", string.Empty))) ? foundry.VNDCNTCT : "N/A";
            model.ContactPhone = FormattingManager.FormatPhone(foundry.PHNUMBR1);
            model.FaxNumber = FormattingManager.FormatPhone(foundry.FAXNUMBR);
            model.Address1 = (!string.IsNullOrEmpty(foundry.ADDRESS1.Replace(" ", string.Empty))) ? foundry.ADDRESS1 : "N/A";
            model.Address2 = (!string.IsNullOrEmpty(foundry.ADDRESS2.Replace(" ", string.Empty))) ? foundry.ADDRESS2 : "N/A";
            model.Address3 = (!string.IsNullOrEmpty(foundry.ADDRESS3.Replace(" ", string.Empty))) ? foundry.ADDRESS3 : "N/A";
            model.City = (!string.IsNullOrEmpty(foundry.CITY.Replace(" ", string.Empty))) ? foundry.CITY : "N/A";
            model.StateName = (state != null && !string.IsNullOrEmpty(state.Name)) ? state.Name : "N/A";
            model.PostalCode = (!string.IsNullOrEmpty(foundry.ZIPCODE.Replace(" ", string.Empty))) ? foundry.ZIPCODE : "N/A";
            model.CountryName = (country != null && !string.IsNullOrEmpty(country.Name)) ? country.Name : "N/A";
            model.PaymentTermDescription = (paymentTerm != null && !string.IsNullOrEmpty(paymentTerm.Description)) ? paymentTerm.Description : "N/A";
            model.IsActive = foundry.VENDSTTS != 1 ? false : true;

            if (_paymentTermRepository != null)
            {
                _paymentTermRepository.Dispose();
                _paymentTermRepository = null;
            }

            if (_shipmentTermRepository != null)
            {
                _shipmentTermRepository.Dispose();
                _shipmentTermRepository = null;
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

            return model;
        }
    }
}