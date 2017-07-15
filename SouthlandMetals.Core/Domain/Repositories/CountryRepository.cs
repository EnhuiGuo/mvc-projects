using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Repositories
{
    public class CountryRepository : ICountryRepository, IDisposable
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SouthlandDb _db;
        private bool disposed = false;

        public CountryRepository()
        {
            _db = new SouthlandDb();
        }

        /// <summary>
        /// get all selectable countries
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSelectableCountries()
        {
            var countries = new List<SelectListItem>();

            try
            {
                countries = _db.Country.Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.CountryId.ToString()
                }).OrderBy(z => z.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting countries: {0} ", ex.ToString());
            }

            return countries;
        }

        /// <summary>
        /// get countires
        /// </summary>
        /// <returns></returns>
        public List<Country> GetCountries()
        {
            var countries = new List<Country>();

            try
            {
                countries = _db.Country.ToList();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting countries: {0} ", ex.ToString());
            }

            return countries;
        }

        /// <summary>
        /// get country by id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public Country GetCountry(Guid countryId)
        {
            var country = new Country();

            try
            {
                country = _db.Country.FirstOrDefault(x => x.CountryId == countryId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting country: {0} ", ex.ToString());
            }

            return country;
        }

        /// <summary>
        /// get country by nullable id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public Country GetCountry(Guid? countryId)
        {
            var country = new Country();

            try
            {
                country = _db.Country.FirstOrDefault(x => x.CountryId == countryId);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting country: {0} ", ex.ToString());
            }

            return country;
        }

        /// <summary>
        /// get country by name
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public Country GetCountry(string countryName)
        {
            var country = new Country();

            try
            {
                if (countryName.Replace(" ", string.Empty).ToLower() == "usa" || countryName.Replace(" ", string.Empty).ToLower() == "u.s.a" ||
                               countryName.Replace(" ", string.Empty).ToLower() == "us" || countryName.Replace(" ", string.Empty).ToLower() == "u.s." ||
                               countryName.Replace(" ", string.Empty).ToLower() == "unitedstates")
                {
                    country = _db.Country.FirstOrDefault(x => x.Name.Replace(" ", string.Empty).ToLower() == "unitedstates");
                }
                else
                {
                    country = _db.Country.FirstOrDefault(x => x.Name.Replace(" ", string.Empty).ToLower() == countryName.Replace(" ", string.Empty).ToLower());
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Error getting country: {0} ", ex.ToString());
            }

            return country;
        }

        /// <summary>
        /// update country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public OperationResult UpdateCountry(Country country)
        {
            var operationResult = new OperationResult();

            var existingCountry = GetCountry(country.CountryId);

            if (existingCountry != null)
            {
                logger.Debug("Country is being updated.");

                try
                {
                    _db.Country.Attach(existingCountry);

                    existingCountry.CountryId = country.CountryId;
                    existingCountry.Name = country.Name;
                    existingCountry.ShipmentTerms = country.ShipmentTerms;
                    existingCountry.PrintTerms = country.PrintTerms;

                    _db.SaveChanges();

                    operationResult.Success = true;
                    operationResult.Message = "Success";
                }
                catch (Exception ex)
                {
                    operationResult.Success = false;
                    operationResult.Message = "Error";
                    logger.ErrorFormat("Error while updating country: { 0} ", ex.ToString());
                }
            }
            else
            {
                operationResult.Success = false;
                operationResult.Message = "Unable to find selected country.";
            }

            return operationResult;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
