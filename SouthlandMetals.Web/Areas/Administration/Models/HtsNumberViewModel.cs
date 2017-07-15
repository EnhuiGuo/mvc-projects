using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Administration.Models
{
    public class HtsNumberViewModel
    {
        [Display(Name = "Hts Number")]
        public Guid HtsNumberId { get; set; }
        [Display(Name = "Description")]
        public string HtsNumberDescription { get; set; }
        [Display(Name = "Duty Rate")]
        public string HtsNumberDutyRate { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public List<HtsNumberViewModel> HtsNumbers { get; set; }
    }
}