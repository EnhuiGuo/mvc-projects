using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class CustomerOrderPartViewModel
    {
        [Display(Name = "Customer Order Part")]
        public Guid CustomerOrderPartId { get; set; }

        [Display(Name = "Customer Order")]
        public Guid CustomerOrderId { get; set; }

        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Display(Name = "Price Sheet")]
        public Guid PriceSheetId { get; set; }

        [Display(Name = "Price Sheet Number")]
        public string PriceSheetNumber { get; set; }

        [Display(Name = "Price Sheet Part")]
        public Guid PriceSheetPartId { get; set; }

        [Display(Name = "Part")]
        public Guid? ProjectPartId { get; set; }

        [Display(Name = "Part")]
        public Guid? PartId { get; set; }

        [Display(Name = "Number")]
        public string PartNumber { get; set; }

        [Display(Name = "Description")]
        public string PartDescription { get; set; }

        [Display(Name = "Quantity Ordered")]
        public int CustomerOrderQuantity { get; set; }

        [Display(Name = "Available Quantity")]
        public int AvailableQuantity { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Cost")]
        public decimal Cost { get; set; }

        [Display(Name = "Est Arrival Date")]
        public DateTime? EstArrivalDate { get; set; }

        [Display(Name = "Est Arrival Date")]
        public string EstArrivalDateStr { get; set; }

        [Display(Name = "Ship Date")]
        public DateTime? ShipDate { get; set; }

        [Display(Name = "Ship Date")]
        public string ShipDateStr { get; set; }

        [Display(Name = "Receipt Quantity")]
        public int ReceiptQuantity { get; set; }
    }
}