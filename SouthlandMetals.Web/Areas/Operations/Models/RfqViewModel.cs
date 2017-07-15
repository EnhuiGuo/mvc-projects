using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class RfqViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        [Display(Name = "RFQ")]
        public Guid RfqId { get; set; }
        [Display(Name = "RFQ Number")]
        public string RfqNumber { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        [Display(Name = "Date")]
        public DateTime RfqDate { get; set; }
        [Display(Name = "Date")]
        public string RfqDateStr { get; set; }
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Display(Name = "Project")]
        public string ProjectName { get; set; }
        [Display(Name = "Country")]
        public Guid? CountryId { get; set; }
        [Display(Name = "Country")]
        public string CountryName { get; set; }
        [Display(Name = "Contact")]
        public string ContactName { get; set; }
        [Display(Name = "Salesperson")]
        public string SalespersonId { get; set; }
        [Display(Name = "Salesperson")]
        public string SalespersonName { get; set; }
        [Display(Name = "Attention")]
        [Required(ErrorMessage = "wocaonima")]
        public string Attention { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryName { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "Project")]
        public Guid ProjectId { get; set; }
        [Display(Name = "Prints Sent")]
        public string PrintsSent { get; set; }
        [Display(Name = "Sent Via")]
        public string SentVia { get; set; }
        [Display(Name = "Machined?")]
        public bool IsMachined { get; set; }
        [Display(Name = "Machined")]
        public string Machining { get; set; }
        [Display(Name = "Packaging")]
        public string Packaging { get; set; }
        [Display(Name = "Number of Samples")]
        public int NumberOfSamples { get; set; }
        [Display(Name = "Details")]
        public string Details { get; set; }
        [Display(Name = "Coating Type")]
        public Guid? CoatingTypeId { get; set; }
        [Display(Name = "Coating Type")]
        public string CoatingType { get; set; }
        [Display(Name = "Material Specification")]
        public Guid? SpecificationMaterialId { get; set; }
        [Display(Name = "Spec")]
        public string SpecificationMaterialDescription { get; set; }
        [Display(Name = "ISIR Required?")]
        public bool ISIRRequired { get; set; }
        [Display(Name = "Sample Casting Available?")]
        public bool SampleCastingAvailable { get; set; }
        [Display(Name = "Metal Certs Available?")]
        public bool MetalCertAvailable { get; set; }
        [Display(Name = "CMTR Required?")]
        public bool CMTRRequired { get; set; }
        [Display(Name = "Gauging Required?")]
        public bool GaugingRequired { get; set; }
        [Display(Name = "Test Bars Required?")]
        public bool TestBarsRequired { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Status:")]
        public string Status { get; set; }
        [Display(Name = "Open?")]
        public bool IsOpen { get; set; }
        [Display(Name = "Hold?")]
        public bool IsHold { get; set; }
        [Display(Name = "Canceled?")]
        public bool IsCanceled { get; set; }
        [Display(Name = "Ship To")]
        public string ShipmentTermDescription { get; set; }
        [Display(Name = "Ship To")]
        public Guid? ShipmentTermId { get; set; }
        [Display(Name = "Material")]
        public Guid? MaterialId { get; set; }
        [Display(Name = "Quote Price Sheet")]
        public Guid QuotePriceSheetId { get; set; }
        [Display(Name = "Quote Price Sheet:")]
        public string QuotePriceSheet { get; set; }
        [Display(Name = "Production Price Sheet:")]
        public string ProductionPriceSheet { get; set; }
        [Display(Name = "Hold Notes")]
        public string HoldNotes { get; set; }
        [Display(Name = "Hold Expiration Date")]
        public DateTime? HoldExpirationDate { get; set; }
        [Display(Name = "Hold Expiration Date")]
        public string HoldExpirationDateStr { get; set; }
        [Display(Name = "Cancel Notes")]
        public string CancelNotes { get; set; }
        [Display(Name = "Cancel Date")]
        public DateTime? CanceledDate { get; set; }
        [Display(Name = "Cancel Date")]
        public string CanceledDateStr { get; set; }
        [Display(Name = "User")]
        public string CurrentUser { get; set; }
        [Display(Name = "Has Price Sheet?")]
        public bool HasPriceSheet { get; set; }

        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }

        public RfqPartViewModel RfqPart { get; set; }

        public List<SelectListItem> Parts { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableProjects { get; set; }
        public List<SelectListItem> SelectableShipmentTerms { get; set; }
        public List<SelectListItem> SelectableCoatingTypes { get; set; }
        public List<SelectListItem> SelectableMaterial { get; set; }
        public List<SelectListItem> SelectableSpecificationMaterial { get; set; }

        public List<RfqViewModel> Rfqs { get; set; }
        public List<RfqPartViewModel> RfqParts { get; set; }
    }
}