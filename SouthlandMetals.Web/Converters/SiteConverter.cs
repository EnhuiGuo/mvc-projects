using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Models;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class SiteConverter
    {
        /// <summary>
        /// convert site to view model
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public SiteViewModel ConvertToView(IV40700_Site site)
        {
            SiteViewModel model = new SiteViewModel();

            var _stateRepository = new StateRepository();
            var _countryRepository = new CountryRepository();

            var state = _stateRepository.GetState(site.STATE);
            var country = _countryRepository.GetCountry(site.COUNTRY);

            model.SiteId = site.LOCNCODE;
            model.LocationCode = site.LOCNCODE;
            model.SiteDescription = (!string.IsNullOrEmpty(site.LOCNDSCR.Replace(" ", string.Empty))) ? site.LOCNDSCR : "N/A";
            model.Address1 = (!string.IsNullOrEmpty(site.ADDRESS1.Replace(" ", string.Empty))) ? site.ADDRESS1 : "N/A";
            model.Address2 = (!string.IsNullOrEmpty(site.ADDRESS2.Replace(" ", string.Empty))) ? site.ADDRESS2 : "N/A";
            model.Address3 = (!string.IsNullOrEmpty(site.ADDRESS3.Replace(" ", string.Empty))) ? site.ADDRESS3 : "N/A";
            model.City = (!string.IsNullOrEmpty(site.CITY.Replace(" ", string.Empty))) ? site.CITY : "N/A";
            model.StateName = (state != null && !string.IsNullOrEmpty(state.Name)) ? state.Name : "N/A";
            model.PostalCode = (!string.IsNullOrEmpty(site.ZIPCODE.Replace(" ", string.Empty))) ? site.ZIPCODE : "N/A";
            model.CountryName = (country != null && !string.IsNullOrEmpty(country.Name)) ? country.Name : "N/A";
            model.IsActive = site.INACTIVE != 1 ? true : false;

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