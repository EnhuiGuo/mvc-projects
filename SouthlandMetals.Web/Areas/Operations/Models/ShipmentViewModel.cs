using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class ShipmentViewModel
    {
        [Display(Name = "Shipment")]
        public Guid ShipmentId { get; set; }
        [Display(Name = "Carrier")]
        public Guid CarrierId { get; set; }
        [Display(Name = "Vessel")]
        public Guid VesselId { get; set; }
        [Display(Name = "Vessel")]
        public string VesselName { get; set; }
        [Display(Name = "Port")]
        public Guid PortId { get; set; }
        [Display(Name = "Port")]
        public string PortName { get; set; }
        [Display(Name = "Departure Date")]
        public DateTime DepartureDate { get; set; }
        [Display(Name = "Est. Arrival Date")]
        public DateTime? EstArrivalDate { get; set; }
        [Display(Name = "Departure Date")]
        public string DepartureDateStr { get; set; }
        [Display(Name = "Est. Arrival Date")]
        public string EstArrivalDateStr { get; set; }
        [Display(Name = "Notes")]
        public string ShipmentNotes { get; set; }
        [Display(Name = "BOL Number")]
        public string BolNumber { get; set; }
        [Display(Name = "Customs Number")]
        public string CustomsNumber { get; set; }
        [Display(Name = "Container")]
        public string ContainerNumber { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Receipt Date")]
        public string ReceiptDateStr { get; set; }
        [Display(Name = "Order")]
        public string OrderNumber { get; set; }
        [Display(Name = "ShipCode")]
        public string ShipCode { get; set; }
        [Display(Name = "Foundry")]
        public Guid FoundryId { get; set; }
        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }
        [Display(Name = "Complete?")]
        public bool IsComplete { get; set; }
        [Display(Name = "Completed Date")]
        public DateTime? CompletedDate { get; set; }
        [Display(Name = "Completed Date")]
        public string CompletedDateStr { get; set; }
        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Show BOLs that are:")]
        public string ProgressId { get; set; }

        public List<ShipmentViewModel> Shipments { get; set; }
        public List<BillOfLadingViewModel> BillsOfLading { get; set; }
        public List<FoundryOrderPartViewModel> PurchaseOrders { get; set; }
        public List<DebitMemoViewModel> DebitMemos { get; set; }
        public List<SelectListItem> SelectableCarriers { get; set; }
        public List<SelectListItem> SelectableVessels { get; set; }
        public List<SelectListItem> SelectablePorts { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableProgressSelection { get; set; }
    }
}
