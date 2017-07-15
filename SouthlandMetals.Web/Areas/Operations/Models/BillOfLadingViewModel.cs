using SouthlandMetals.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class BillOfLadingViewModel : ContainerPartViewModel
    {
        [Display(Name = "BillOfLading")]
        public Guid BillOfLadingId { get; set; }

        [Display(Name = "Shipment")]
        public Guid ShipmentId { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryName { get; set; }

        [Display(Name = "Date")]
        public DateTime BolDate { get; set; }

        [Display(Name = "Date")]
        public string BolDateStr { get; set; }

        [Display(Name = "BOL Number")]
        public string BolNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Lcl")]
        public bool HasLcl { get; set; }

        [Display(Name = "Door Move")]
        public bool HasDoorMove { get; set; }

        [Display(Name = "Arrival Notice")]
        public bool HasArrivalNotice { get; set; }

        [Display(Name = "Originals")]
        public bool HasOriginalDocuments { get; set; }

        [Display(Name = "Notes")]
        public string BolNotes { get; set; }

        [Display(Name = "Wire Instructions")]
        public string WireInstructions { get; set; }

        [Display(Name = "Customs Number")]
        public string CustomsNumber { get; set; }

        [Display(Name = "Customs Liquidated")]
        public bool IsCustomsLiquidated { get; set; }

        [Display(Name = "Analyzed?")]
        public bool HasBeenAnalyzed { get; set; }

        [Display(Name = "Pallet Count")]
        public int PalletCount { get; set; }

        [Display(Name = "Gross Weight")]
        public decimal GrossWeight { get; set; }

        [Display(Name = "Net Weight")]
        public decimal NetWeight { get; set; }

        [Display(Name = "Invoice")]
        public Guid FoundryInvoiceId { get; set; }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Invoice Total")]
        public decimal InvoiceTotal { get; set; }

        [Display(Name = "Scheduled Date")]
        public DateTime? ScheduledDate { get; set; }

        [Display(Name = "Scheduled Date")]
        public string ScheduledDateStr { get; set; }

        [Display(Name = "Actual Date")]
        public DateTime? ActualDate { get; set; }

        [Display(Name = "Actual Date")]
        public string ActualDateStr { get; set; }

        [Display(Name = "Notes")]
        public string InvoiceNotes { get; set; }

        [Display(Name = "BucketName")]
        public string BucketName { get; set; }

        [Display(Name = "BucketValue")]
        public string BucketValue { get; set; }

        [Display(Name = "Vessel")]
        public string VesselName { get; set; }

        [Display(Name = "Port")]
        public string PortName { get; set; }

        [Display(Name = "Departure Date")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "Departure Date")]
        public string DepartureDateStr { get; set; }

        [Display(Name = "Est. Arrival Date")]
        public DateTime? EstArrivalDate { get; set; }

        [Display(Name = "Est. Arrival Date")]
        public string EstArrivalDateStr { get; set; }

        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        public FoundryInvoiceViewModel FoundryInvoice { get; set; }

        public List<SelectListItem> SelectableContainers { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }
        public List<SelectListItem> SelectablePurchaseOrders { get; set; }

        public List<PalletViewModel> Pallets { get; set; }

        public List<ContainerViewModel> Containers { get; set; }
        public List<ContainerPartViewModel> ContainerParts { get; set; }
        public List<FoundryOrderViewModel> PurchaseOrders { get; set; }
        public List<BillOfLadingViewModel> BillsOfLading { get; set; }
    }
}