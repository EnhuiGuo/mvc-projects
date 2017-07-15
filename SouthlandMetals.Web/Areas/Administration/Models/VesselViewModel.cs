using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Administration.Models
{
    public class VesselViewModel
    {
        [Display(Name = "Vessel")]
        public Guid VesselId { get; set; }
        [Display(Name = "Vessel")]
        public string VesselName { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public List<VesselViewModel> Vessels { get; set; }
    }
}