using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class QuoteViewModel
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        [Display(Name = "Quote")]
        public Guid QuoteId { get; set; }
        [Display(Name = "Project")]
        public Guid? ProjectId { get; set; }
        [Display(Name = "RFQ")]
        public Guid? RfqId { get; set; }
        [Display(Name = "Price Sheet")]
        public Guid? PriceSheetId { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Address")]
        public string CustomerAddressId { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        [Display(Name = "Quote Number")]
        public string QuoteNumber { get; set; }
        [Display(Name = "RFQ Number")]
        public string RfqNumber { get; set; }
        [Display(Name = "Quote Date")]
        public DateTime QuoteDate { get; set; }
        [Display(Name = "Quote Date")]
        public string QuoteDateStr { get; set; }
        [Display(Name = "Validity")]
        public int Validity { get; set; }
        [Display(Name = "Contact")]
        public string ContactName { get; set; }
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Display(Name = "Address")]
        public string CustomerAddress { get; set; }
        [Display(Name = "CC: ")]
        public string ContactCopy { get; set; }
        [Display(Name = "Shipment Term")]
        public Guid? ShipmentTermId { get; set; }     
        [Display(Name = "Shipment Term")]
        public string ShipmentTermDescription { get; set; }
        [Display(Name = "Payment TermId")]
        public Guid? PaymentTermId { get; set; }
        [Display(Name = "Payment Term")]
        public string PaymentTermDescription { get; set; }
        [Display(Name = "Minimum Shipment")]
        public string MinimumShipment { get; set; }
        [Display(Name = "Tooling Term")]
        public string ToolingTermDescription { get; set; }
        [Display(Name = "Sample Lead Time")]
        public string SampleLeadTime { get; set; }
        [Display(Name = "Production Lead Time")]
        public string ProductionLeadTime { get; set; }
        [Display(Name = "Material")]
        public Guid? MaterialId { get; set; }
        [Display(Name = "Material")]
        public string MaterialDescription { get; set; }
        [Display(Name = "Coating")]
        public Guid? CoatingTypeId { get; set; }     
        [Display(Name = "Coating Type")]
        public string CoatingTypeDescription { get; set; }
        [Display(Name = "Hts Number")]
        public Guid? HtsNumberId { get; set; }
        [Display(Name = "Hts Number")]
        public string HtsNumberDescription { get; set; }
        [Display(Name = "Machined?")]
        public bool IsMachined { get; set; }
        [Display(Name = "Machined")]
        public string Machining { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Open?")]
        public bool IsOpen { get; set; }
        [Display(Name = "Hold?")]
        public bool IsHold { get; set; }
        [Display(Name = "Canceled?")]
        public bool IsCanceled { get; set; }
        [Display(Name = "Specification")]
        public Guid? MaterialSpecificationId { get; set; }
        [Display(Name = "Has Customer Order?")]
        public bool HasCustomerOrder { get; set; }
        [Display(Name = "Hold Notes")]
        public string HoldNotes { get; set; }
        [Display(Name = "Hold Expiration Date")]
        public DateTime? HoldExpirationDate { get; set; }
        [Display(Name = "Hold Expiration Date")]
        public string HoldExpirationDateStr { get; set; }
        [Display(Name = "Cancel Notes")]
        public string CancelNotes { get; set; }
        [Display(Name = "User")]
        public string CurrentUser { get; set; }
        [Display(Name = "Canceled Date")]
        public DateTime? CanceledDate { get; set; }
        [Display(Name = "Canceled Date")]
        public string CanceledDateStr { get; set; }

        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        public QuotePartViewModel QuotePart { get; set; }

        public List<SelectListItem> Parts { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableCustomerAddresses { get; set; }
        public List<SelectListItem> SelectableProjects { get; set; }
        public List<SelectListItem> SelectableShipmentTerms { get; set; }
        public List<SelectListItem> SelectablePaymentTerms { get; set; }
        public List<SelectListItem> SelectableMaterial { get; set; }
        public List<SelectListItem> SelectableCoatingTypes { get; set; }
        public List<SelectListItem> SelectableHtsNumbers { get; set; }

        public List<QuoteViewModel> Quotes { get; set; }
        public List<QuotePartViewModel> QuoteParts { get; set; }
    }
}