using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class QuotePartViewModel
    {
        [Display(Name = "Quote")]
        public Guid QuoteId { get; set; }
        [Display(Name = "Project Part")]
        public Guid ProjectPartId { get; set; }
        [Display(Name = "Part")]
        public Guid? PartId { get; set; }
        [Display(Name = "Number")]
        public string PartNumber { get; set; }
        [Display(Name = "Description")]
        public string PartDescription { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "Revision")]
        public string RevisionNumber { get; set; }
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Est. Annual Usage")]
        public int AnnualUsage { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        [Display(Name = "Cost")]
        public decimal Cost { get; set; }
        [Display(Name = "Pattern Price")]
        public decimal PatternPrice { get; set; }
        [Display(Name = "Pattern Cost")]
        public decimal PatternCost { get; set; }
        [Display(Name = "Fixture Price")]
        public decimal FixturePrice { get; set; }
        [Display(Name = "Fixture Cost")]
        public decimal FixtureCost { get; set; }
        [Display(Name = "IsRaw?")]
        public bool IsRaw { get; set; }
        [Display(Name = "IsMachined?")]
        public bool IsMachined { get; set; }
    }
}