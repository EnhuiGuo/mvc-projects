using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Core.Domain.Models
{
    public class PartOperationModel
    {
        [Display(Name = "Part")]
        public Guid ProjectPartId { get; set; }
        [Display(Name = "Part")]
        public Guid? PartId { get; set; }
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }
        [Display(Name = "Description")]
        public string PartDescription { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Revision")]
        public string RevisionNumber { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "Sub Foundry")]
        public string SubFoundryId { get; set; }
        [Display(Name = "Type")]
        public Guid? PartTypeId { get; set; }
        [Display(Name = "Hts Number")]
        public Guid? HtsNumberId { get; set; }
        [Display(Name = "Metal")]
        public Guid? MaterialId { get; set; }
        [Display(Name = "Shipment Terms")]
        public Guid? ShipmentTermId { get; set; }
        [Display(Name = "Metal Specification")]
        public Guid? MaterialSpecificationId { get; set; }
        [Display(Name = "Payment Terms")]
        public Guid? PaymentTermId { get; set; }
        [Display(Name = "Machined")]
        public bool IsMachined { get; set; }
        [Display(Name = "Status")]
        public Guid? PartStatusId { get; set; }
        [Display(Name = "Destination")]
        public Guid? DestinationId { get; set; }
        [Display(Name = "Unit Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Unit Cost")]
        public decimal Cost { get; set; }
        [Display(Name = "Unit Price")]
        public decimal Price { get; set; }
        [Display(Name = "Additional Cost")]
        public decimal AdditionalCost { get; set; }
        [Display(Name = "Surcharge")]
        public Guid? SurchargeId { get; set; }
        [Display(Name = "Pallet Quantity")]
        public int PalletQuantity { get; set; }
        [Display(Name = "Ship To Address")]
        public string CustomerAddressId { get; set; }
        [Display(Name = "Ship To Site")]
        public string SiteId { get; set; }
        [Display(Name = "Coating Type")]
        public Guid? CoatingTypeId { get; set; }
        [Display(Name = "Fixture Date")]
        public DateTime? FixtureDate { get; set; }
        [Display(Name = "Fixture Cost")]
        public decimal FixtureCost { get; set; }
        [Display(Name = "Fixture Price")]
        public decimal FixturePrice { get; set; }
        [Display(Name = "Pattern Date")]
        public DateTime? PatternDate { get; set; }
        [Display(Name = "Pattern Cost")]
        public decimal PatternCost { get; set; }
        [Display(Name = "Pattern Price")]
        public decimal PatternPrice { get; set; }
        [Display(Name = "Family Pattern")]
        public bool IsFamilyPattern { get; set; }
        [Display(Name = "Pattern Material")]
        public Guid? PatternMaterialId { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Annual Usage")]
        public int AnnualUsage { get; set; }
        [Display(Name = "Is Raw")]
        public bool IsRaw { get; set; }
        [Display(Name = "Minimum Qty")]
        public int MinimumQuantity { get; set; }
        [Display(Name = "Safety Stock")]
        public int SafetyQuantity { get; set; }
        [Display(Name = "Order Cycle")]
        public int OrderCycle { get; set; }
        [Display(Name = "Lead Time")]
        public decimal LeadTime { get; set; }
        [Display(Name = "Foundry Order")]
        public Guid? FoundryOrderId { get; set; }
        [Display(Name = "Tooling Description")]
        public string ToolingDescription { get; set; }
        public bool IsProjectPart { get; set; }
    }
}
