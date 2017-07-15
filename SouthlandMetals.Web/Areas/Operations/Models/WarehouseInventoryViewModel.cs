using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class WarehouseInventoryViewModel
    {
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Ship Code")]
        public string ShipCode { get; set; }
        [Display(Name = "Part")]
        public string PartNumber { get; set; }
        [Display(Name = "Pallet")]
        public string PalletNumber { get; set; }
        [Display(Name = "Pallet Quantity")]
        public int PalletQuantity { get; set; }
        [Display(Name = "Total Quantity")]
        public int TotalQuantity { get; set; }
        [Display(Name = "Container")]
        public string ContainerNumber { get; set; }
        [Display(Name = "PO Number")]
        public string PONumber { get; set; }
        [Display(Name = "Warehouse Date")]
        public DateTime? WarehouseDate { get; set; }
        [Display(Name = "Warehouse Date")]
        public string WarehouseDateStr { get; set; }
        [Display(Name = "Sixty Days")]
        public DateTime? SixtyDaysDate { get; set; }
        [Display(Name = "Sixty Days")]
        public string SixtyDaysDateStr { get; set; }
        [Display(Name = "Part Weight")]
        public decimal PartWeight { get; set; }
    }
}