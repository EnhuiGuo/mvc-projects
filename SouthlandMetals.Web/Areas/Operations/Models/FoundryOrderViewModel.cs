using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class FoundryOrderViewModel
    {
        [Display(Name = "Success")]
        public bool Success { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Foundry Order")]
        public Guid FoundryOrderId { get; set; }

        [Display(Name = "Bill of Lading")]
        public Guid BillOfLadingId { get; set; }

        [Display(Name = "Order Type")]
        public string OrderTypeId { get; set; }

        [Display(Name = "Order Type")]
        public string OrderTypeDescription { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Customer")]
        [Required]
        public string CustomerId { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Ship To")]
        public string CustomerAddressId { get; set; }

        [Display(Name = "Ship To")]
        public string CustomerAddress { get; set; }

        [Display(Name = "Project")]
        [Required]
        public Guid ProjectId { get; set; }

        [Display(Name = "Project")]
        public string ProjectName { get; set; }

        [Display(Name = "Foundry")]
        [Required]
        public string FoundryId { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryName { get; set; }

        [Display(Name = "Notes")]
        public string OrderNotes { get; set; }

        [Display(Name = "Terms")]
        public Guid ShipmentTermsId { get; set; }

        [Display(Name = "Terms")]
        public string ShipmentTerms { get; set; }

        [Display(Name = "Order Date")]
        [Required]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "PO Date")]
        [Required]
        public DateTime? PODate { get; set; }

        [Display(Name = "PO Date")]
        [Required]
        public string PODateStr { get; set; }

        [Display(Name = "Due Date")]
        [Required]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Due Date")]
        [Required]
        public string DueDateStr { get; set; }

        [Display(Name = "Port Date")]
        [Required]
        public DateTime? PortDate { get; set; }

        [Display(Name = "Port Date")]
        public string PortDateStr { get; set; }

        [Display(Name = "Ship Date")]
        [Required]
        public DateTime? ShipDate { get; set; }

        [Display(Name = "Ship Date")]
        public string ShipDateStr { get; set; }

        [Display(Name = "Est Arrival Date")]
        [Required]
        public DateTime? EstArrivalDate { get; set; }

        [Display(Name = "Est Arrival Date")]
        public string EstArrivalDateStr { get; set; }

        [Display(Name = "Ship Via")]
        public string ShipVia { get; set; }

        [Display(Name = "Transit Days")]
        public int TransitDays { get; set; }

        [Display(Name = "Site")]
        public string SiteId { get; set; }

        [Display(Name = "Site Description")]
        public string SiteDescription { get; set; }

        [Display(Name = "Open?")]
        public bool IsOpen { get; set; }

        [Display(Name = "Canceled?")]
        public bool IsCanceled { get; set; }

        [Display(Name = "Canceled Date")]
        public DateTime? CanceledDate { get; set; }

        [Display(Name = "Canceled Date")]
        public string CanceledDateStr { get; set; }

        [Display(Name = "Complete?")]
        public bool IsComplete { get; set; }

        [Display(Name = "Completed Date")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "Completed Date")]
        public string CompletedDateStr { get; set; }

        [Display(Name = "Hold?")]
        public bool IsHold { get; set; }

        [Display(Name = "Confirmed?")]
        public bool IsConfirmed { get; set; }

        [Display(Name = "Confirmed Date")]
        public DateTime? ConfirmedDate { get; set; }

        [Display(Name = "Confirmed Date")]
        public string ConfirmedDateStr { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Cancel Notes")]
        public string CancelNotes { get; set; }

        [Display(Name = "Hold Notes")]
        public string HoldNotes { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Sample")]
        public bool IsSample { get; set; }

        [Display(Name = "Tooling")]
        public bool IsTooling { get; set; }

        [Display(Name = "Production")]
        public bool IsProduction { get; set; }

        [Display(Name = "Hold Expiration Date")]
        public DateTime? HoldExpirationDate { get; set; }

        [Display(Name = "Hold Expiration Date")]
        public string HoldExpirationDateStr { get; set; }

        [Display(Name = "User")]
        public string CurrentUser { get; set; }

        [Display(Name = "Ship Code List")]
        public string ShipCodeList { get; set; }

        [Display(Name = "Ship Code")]
        public string ShipCode { get; set; }

        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        public List<FoundryOrderViewModel> PurchaseOrders { get; set; }
        public List<FoundryOrderViewModel> FoundryOrders { get; set; }
        public List<FoundryOrderPartViewModel> FoundryOrderParts { get; set; }
        public List<FoundryOrderViewModel> FoundryOrderDifferents { get; set; }

        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }
        public List<SelectListItem> SelectableOrderTypes { get; set; }
        public List<SelectListItem> SelectableCustomerAddresses { get; set; }
        public List<SelectListItem> SelectableCustomerOrders { get; set; }
        public List<SelectListItem> SelectableSites { get; set; }
    }
}