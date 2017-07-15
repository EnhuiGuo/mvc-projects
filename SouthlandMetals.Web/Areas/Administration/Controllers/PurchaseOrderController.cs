using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Interfaces;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Web.Areas.Administration.Models;
using SouthlandMetals.Web.Controllers;
using SouthlandMetals.Web.Converters;
using SouthlandMetals.Web.Helpers;
using System;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Administration.Controllers
{
    public class PurchaseOrderController : ApplicationBaseController
    {
        private ICountryRepository _countryRepository;

        public PurchaseOrderController()
        {
            _countryRepository = new CountryRepository();
        }

        public PurchaseOrderController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        /// <summary>
        /// GET: Administration/PurchaseOrder/ShipmentTerms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ShipmentTerms()
        {
            var model = new CountryViewModel();

            var countries = _countryRepository.GetSelectableCountries();

            var defaultCountry = new SelectListItem()
            {
                Text = "--Select Country--",
                Value = null
            };

            countries.Insert(0, defaultCountry);

            model.SelectableCountries = countries;

            return View(model);
        }

        /// <summary>
        /// edit shipment terms
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult EditShipmentTerms(CountryViewModel model)
        {
            var operationResult = new OperationResult();

            Country country = new CountryConverter().ConvertToDomain(model);

            operationResult = _countryRepository.UpdateCountry(country);

            return Json(operationResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get shipment terms by country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public JsonResult GetShipmentTermsByCountry(Guid countryId)
        {
            var country = _countryRepository.GetCountry(countryId);

            CountryViewModel convertedModel = new CountryConverter().ConvertToView(country);

            return Json(convertedModel, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_countryRepository != null)
                {
                    _countryRepository.Dispose();
                    _countryRepository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}