using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class PackingListViewModel
    {
        public Guid PartId { get; set; }
        public string PalletNumber { get; set; }
        public int PalletQuantity { get; set; }
        public decimal PalletWeight { get; set; }
        public decimal PalletTotal { get; set; }
        public int TotalPalletQuantity { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }

        [Display(Name = "Packing List")]
        public Guid PackingListId { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Display(Name = "Address")]
        public string CustomerAddressId { get; set; }
        [Display(Name = "Address")]
        public string CustomerAddress { get; set; }
        [Display(Name = "Ship Date")]
        public DateTime ShipDate { get; set; }
        [Display(Name = "Ship Date")]
        public string ShipDateStr { get; set; }
        [Display(Name = "Freight")]
        public decimal Freight { get; set; }
        [Display(Name = "Carrier")]
        public Guid CarrierId { get; set; }
        [Display(Name = "Carrier")]
        public string CarrierName { get; set; }
        [Display(Name = "Trailer")]
        public string TrailerNumber { get; set; }
        [Display(Name = "Tracking#")]
        public string TrackingNumber { get; set; }
        [Display(Name = "Net Weight")]
        public decimal NetWeight { get; set; }
        [Display(Name = "Gross Weight")]
        public decimal GrossWeight { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Date Delivered")]
        public DateTime? DeliveryDate { get; set; }
        [Display(Name = "Date Delivered")]
        public string DeliveryDateStr { get; set; }
        [Display(Name = "Closed?")]
        public bool IsClosed { get; set; }
        [Display(Name = "Date Closed")]
        public DateTime? ClosedDate { get; set; }
        [Display(Name = "Date Closed")]
        public string ClosedDateStr { get; set; }
        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Created")]
        public string CreatedDateStr { get; set; }

        public PackingListPartViewModel PackingListPart { get; set; }
        public List<SelectListItem> SelectableParts { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableCustomerAddresses { get; set; }
        public List<SelectListItem> SelectableCarriers { get; set; }
        public List<SelectListItem> SelectablePONumbers { get; set; }
        public List<PackingListViewModel> PackingLists { get; set; }

        public IEnumerable<PackingListPartViewModel> PackingListParts { get; set; }
    }
}