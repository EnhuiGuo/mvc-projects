using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class PartInventoryViewModel
    {
        [Display(Name = "Part")]
        public Guid PartId { get; set; }
        [Display(Name = "Part")]
        public string PartNumber { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "Cost")]
        public decimal Cost { get; set; }
        [Display(Name = "Quantity On Hand")]
        public decimal QuantityOnHand { get; set; }
        [Display(Name = "On Order Quantity")]
        public decimal OnOrderQuantity { get; set; }
        [Display(Name = "Receipt Date")]
        public DateTime? ReceiptDate { get; set; }
        [Display(Name = "Receipt Date")]
        public string ReceiptDateStr { get; set; }
        [Display(Name = "Receipt Quantity")]
        public decimal ReceiptQuantity { get; set; }
        [Display(Name = "Sales Date")]
        public DateTime? SalesDate { get; set; }
        [Display(Name = "Sales Date")]
        public string SalesDateStr { get; set; }
        [Display(Name = "Year To Date Sales")]
        public decimal YearToDateSales { get; set; }
        [Display(Name = "Minimum Quantity")]
        public int MinimumQuantity { get; set; }
        [Display(Name = "Safety Quantity")]
        public int SafetyQuantity { get; set; }
        [Display(Name = "Order Cycle")]
        public int OrderCycle { get; set; }
        [Display(Name = "Lead Time")]
        public decimal LeadTime { get; set; }
        [Display(Name = "Transit Time")]
        public decimal TransitTime { get; set; }
        [Display(Name = "Reorder Point")]
        public int ReorderPoint { get; set; }
        [Display(Name = "Daily Sales")]
        public decimal AverageDailySales { get; set; }
    }
}