using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class PartViewModel
    {
        [Display(Name = "Part")]
        public Guid? PartId { get; set; }
        [Display(Name = "Part")]
        public Guid ProjectPartId { get; set; }
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string PartDescription { get; set; }
        [Display(Name = "Latest Revision")]
        public string RevisionNumber { get; set; }
        [Display(Name = "Project")]
        public Guid? ProjectId { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Ship To Address")]
        public string CustomerAddressId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "Sub Foundry")]
        public string SubFoundryId { get; set; }
        [Display(Name = "Foundry Name")]
        public string FoundryName { get; set; }
        [Display(Name = "Hts Number")]
        public Guid? HtsNumberId { get; set; }
        [Display(Name = "Hts Number")]
        public string HtsNumber { get; set; }
        [Display(Name = "Metal")]
        public Guid? MaterialId { get; set; }
        [Display(Name = "Metal Specification")]
        public Guid? SpecificationMaterialId { get; set; }
        [Display(Name = "Status")]
        public Guid? PartStatusId { get; set; }
        [Display(Name = "Status")]
        public string PartStatusDescription { get; set; }
        [Display(Name = "Type")]
        public Guid? PartTypeId { get; set; }
        [Display(Name = "Type")]
        public string PartTypeDescription { get; set; }
        [Display(Name = "Shipment Terms")]
        public Guid? ShipmentTermId { get; set; }
        [Display(Name = "Payment Terms")]
        public Guid? PaymentTermId { get; set; }
        [Display(Name = "Surcharge")]
        public Guid? SurchargeId { get; set; }
        [Display(Name = "Ship To Site")]
        public string SiteId { get; set; }
        [Display(Name = "Destination")]
        public Guid? DestinationId { get; set; }
        [Display(Name = "Coating Type")]
        public Guid? CoatingTypeId { get; set; }
        [Display(Name = "Pattern Material")]
        public Guid? PatternMaterialId { get; set; }
        [Display(Name = "Raw?")]
        public bool IsRaw { get; set; }
        [Display(Name = "Machined?")]
        public bool IsMachined { get; set; }
        [Display(Name = "Pallet Quantity")]
        public int PalletQuantity { get; set; }
        [Display(Name = "Pallet Weight")]
        public decimal PalletWeight { get; set; }
        [Display(Name = "Unit Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Unit Cost")]
        public decimal Cost { get; set; }
        [Display(Name = "Unit Price")]
        public decimal Price { get; set; }
        [Display(Name = "Additional Cost")]
        public decimal AdditionalCost { get; set; }
        [Display(Name = "Yearly Sales")]
        public decimal YearlySalesTotal { get; set; }
        [Display(Name = "60 days Sales")]
        public decimal SixtyDaysSalesTotal { get; set; }
        [Display(Name = "Monthly Sales")]
        public decimal MonthlySalesTotal { get; set; }
        [Display(Name = "Avg. Daily Sales")]
        public decimal AverageDailySales { get; set; }
        [Display(Name = "Qty On Hand")]
        public decimal QuantityOnHand { get; set; }
        [Display(Name = "Fixture Date")]
        public DateTime? FixtureDate { get; set; }
        [Display(Name = "Fixture Date")]
        public string FixtureDateStr { get; set; }
        [Display(Name = "Fixture Cost")]
        public decimal FixtureCost { get; set; }
        [Display(Name = "Fixture Price")]
        public decimal FixturePrice { get; set; }
        [Display(Name = "Pattern Date")]
        public DateTime? PatternDate { get; set; }
        [Display(Name = "Pattern Date")]
        public string PatternDateStr { get; set; }
        [Display(Name = "Pattern Cost")]
        public decimal PatternCost { get; set; }
        [Display(Name = "Pattern Price")]
        public decimal PatternPrice { get; set; }
        [Display(Name = "Family Pattern?")]
        public bool IsFamilyPattern { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Minimum Qty")]
        public int MinimumQuantity { get; set; }
        [Display(Name = "Safety Stock")]
        public int SafetyQuantity { get; set; }
        [Display(Name = "Order Cycle")]
        public int OrderCycle { get; set; }
        [Display(Name = "Lead Time")]
        public decimal LeadTime { get; set; }
        [Display(Name = "Transit Time")]
        public decimal TransitTime { get; set; }
        [Display(Name = "Reorder Point")]
        public int ReorderPoint { get; set; }
        [Display(Name = "Est. Annual Usage")]
        public int AnnualUsage { get; set; }
        [Display(Name = "Tooling Order")]
        public Guid? FoundryOrderId { get; set; }
        [Display(Name = "Tooling Order")]
        public string ToolingOrderNumber { get; set; }
        [Display(Name = "Tooling Description")]
        public string ToolingDescription { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Project Part?")]
        public bool IsProjectPart { get; set; }

        public List<SelectListItem> SelectableAccountCodes { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }
        public List<SelectListItem> SelectableHtsNumbers { get; set; }    
        public List<SelectListItem> SelectablePartStates { get; set; }
        public List<SelectListItem> SelectableSurcharge { get; set; }
        public List<SelectListItem> SelectablePartTypes { get; set; }
        public List<SelectListItem> SelectableProjects { get; set; }
        public List<SelectListItem> SelectablePaymentTerms { get; set; }
        public List<SelectListItem> SelectableShipmentTerms { get; set; }
        public List<SelectListItem> SelectableCustomerAddresses { get; set; }
        public List<SelectListItem> SelectableSites { get; set; }
        public List<SelectListItem> SelectableCoatingTypes { get; set; }
        public List<SelectListItem> SelectablePatternMaterial { get; set; }
        public List<SelectListItem> SelectableMaterial { get; set; }
        public List<SelectListItem> SelectableSpecificationMaterial { get; set; }
        public List<SelectListItem> SelectableDestinations { get; set; }
        public List<DrawingViewModel> Drawings { get; set; }
        public List<LayoutViewModel> Layouts { get; set; }
    }
}