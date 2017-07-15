using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class WarehouseViewModel
    {
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }
        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }

        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableCountries { get; set; }
    }
}