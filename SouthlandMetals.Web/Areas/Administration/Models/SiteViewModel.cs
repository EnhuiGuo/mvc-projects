using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Administration.Models
{
    public class SiteViewModel
    {
        [Display(Name = "Site")]
        public string SiteId { get; set; }
        [Display(Name = "Location Code")]
        public string LocationCode { get; set; }
        [Display(Name = "Description")]
        public string SiteDescription { get; set; }
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
        public string CountryName { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }

        public List<SiteViewModel> Sites { get; set; }
    }
}