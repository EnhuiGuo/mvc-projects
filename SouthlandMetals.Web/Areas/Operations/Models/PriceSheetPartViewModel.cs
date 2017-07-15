using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class PriceSheetPartViewModel
    {
        [Display(Name = "Price Sheet Part")]
        public Guid PriceSheetPartId { get; set; }
        [Display(Name = "Project Part")]
        public Guid ProjectPartId { get; set; }
        [Display(Name = "Est. Annual Usage")]
        public decimal AnnualUsage { get; set; }
        [Display(Name = "Part")]
        public Guid PartId { get; set; }
        [Display(Name = "Part")]
        public string PartNumber { get; set; }
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Price Sheet")]
        public Guid PriceSheetId { get; set; }
        [Display(Name = "Price Sheet")]
        public string PriceSheetNumber { get; set; }
        [Display(Name = "Description")]
        public string PartDescription { get; set; }
        [Display(Name = "Available Quantity")]
        public int AvailableQuantity { get; set; }
        [Display(Name = "Customer Order Quantity")]
        public int CustomerOrderQuantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }

        public List<PriceSheetPartViewModel> OrderParts { get; set; }
    }
}