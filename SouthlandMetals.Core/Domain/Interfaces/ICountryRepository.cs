using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface ICountryRepository : IDisposable
    {
        List<SelectListItem> GetSelectableCountries();

        List<Country> GetCountries();

        Country GetCountry(Guid countryId);

        Country GetCountry(Guid? countryId);

        Country GetCountry(string countryName);

        OperationResult UpdateCountry(Country country);
    }
}
