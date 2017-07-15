using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Administration.Models
{
    public class FoundryViewModel
    {
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "VendorID")]
        public string VendorId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryName { get; set; }
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
        [Display(Name = "Contact")]
        public string ContactName { get; set; }
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }
        [Display(Name = "Fax")]
        public string FaxNumber { get; set; }
        [Display(Name = "Address1")]
        public string Address1 { get; set; }
        [Display(Name = "Address2")]
        public string Address2 { get; set; }
        [Display(Name = "Address3")]
        public string Address3 { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "State")]
        public string StateName { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "Country")]
        public Guid? CountryId { get; set; }
        [Display(Name = "Country")]
        public string CountryName { get; set; }
        [Display(Name = "Payment Term")]
        public string PaymentTermDescription { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }

        public List<FoundryViewModel> Foundries { get; set; }
    }
}