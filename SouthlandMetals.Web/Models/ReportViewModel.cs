using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Models
{
    public class ReportViewModel
    {
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "Ship Code")]
        public string ShipCode { get; set; }
        [Display(Name = "Condition")]
        public string Condition { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Start Date")]
        public string StartDateStr { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "End Date")]
        public string EndDateStr { get; set; }

        public List<SelectListItem> SelectableCountries { get; set; }
    }
}