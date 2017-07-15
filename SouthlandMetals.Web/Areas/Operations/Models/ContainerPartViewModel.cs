using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class ContainerPartViewModel
    {
        [Display(Name = "Container Part")]
        public Guid ContainerPartId { get; set; }

        [Display(Name = "Part")]
        public Guid PartId { get; set; }

        [Display(Name = "Part")]
        public string PartNumber { get; set; }

        [Display(Name = "Container")]
        public Guid? ContainerId { get; set; }

        [Display(Name = "Container")]
        public string ContainerNumber { get; set; }

        [Display(Name = "Order")]
        public Guid FoundryOrderId { get; set; }

        [Display(Name = "Foundry Order")]
        public string OrderNumber { get; set; }

        [Display(Name = "Pallet")]
        public string PalletNumber { get; set; }

        [Display(Name = "Available Quantity")]
        public int AvailableQuantity { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Ship Code")]
        public string ShipCode { get; set; }

        [Display(Name = "Ship Code Notes")]
        public string ShipCodeNotes { get; set; }

        [Display(Name = "Cost")]
        public decimal Cost { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Weight")]
        public decimal Weight { get; set; }

        [Display(Name = "Foundry Order Part")]
        public Guid FoundryOrderPartId { get; set; }
    }
}