using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;

namespace SouthlandMetals.Web.Converters
{
    public class CountryConverter
    {
        /// <summary>
        /// convert country to view model
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public CountryViewModel ConvertToView(Country country)
        {
            CountryViewModel model = new CountryViewModel();

            model.CountryId = country.CountryId;
            model.CountryName = (!string.IsNullOrEmpty(country.Name)) ? country.Name : "N/A";
            model.ShipmentTerms = (!string.IsNullOrEmpty(country.ShipmentTerms)) ? country.ShipmentTerms : "N/A";
            model.PrintTerms = country.PrintTerms;

            return model;
        }

        /// <summary>
        /// convert country view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Country ConvertToDomain(CountryViewModel model)
        {
            Country country = new Country();

            country.CountryId = model.CountryId;
            country.Name = model.CountryName;
            country.ShipmentTerms = model.ShipmentTerms;
            country.PrintTerms = model.PrintTerms;

            return country;
        }
    }
}