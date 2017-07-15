using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class PriceSheetViewModel
    {
        [Display(Name = "Price Sheet")]
        public Guid PriceSheetId { get; set; }
        [Display(Name = "Number")]
        public string Number { get; set; }
        [Display(Name = "RFQ")]
        public Guid RfqId { get; set; }
        [Display(Name = "Include Raw?")]
        public bool IncludeRaw { get; set; }
        [Display(Name = "Include Machined?")]
        public bool IncludeMachined { get; set; }
        [Display(Name = "Quote")]
        public Guid QuoteId { get; set; }
        [Display(Name = "RFQ")]
        public string RfqNumber { get; set; }
        [Display(Name = "Project Margin")]
        public decimal ProjectMargin { get; set; }
        [Display(Name = "Project Margin Text")]
        public string ProjectMarginText { get; set; }
        [Display(Name = "Foundry")]
        public string Foundry { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "Project")]
        public string ProjectName { get; set; }
        [Display(Name = "Customer")]
        public string Customer { get; set; }
        [Display(Name = "WAF")]
        public int WAF { get; set; }
        [Display(Name = "Annual Dollars")]
        public int AnnualDollars { get; set; }
        [Display(Name = "Annual Margin")]
        public decimal AnnualMargin { get; set; }
        [Display(Name = "Annual Margin")]
        public string AnnualMarginText { get; set; }
        [Display(Name = "Annual Weight")]
        public int AnnualWeight { get; set; }
        [Display(Name = "Annual Container")]
        public int AnnualContainer { get; set; }
        [Display(Name = "Dollar Container")]
        public int DollarContainer { get; set; }
        [Display(Name = "Insurance Freight")]
        public int InsuranceFreight { get; set; }
        [Display(Name = "Insurance Percentage")]
        public int InsurancePercentage { get; set; }
        [Display(Name = "Insurance Duty")]
        public int InsuranceDuty { get; set; }
        [Display(Name = "Insurance Divisor")]
        public int InsuranceDivisor { get; set; }
        [Display(Name = "Insurance Premium")]
        public int InsurancePremium { get; set; }
        [Display(Name = "Total Annual Cost")]
        public decimal TotalAnnualCost { get; set; }
        [Display(Name = "Total Annual Price")]
        public decimal TotalAnnualPrice { get; set; }
        [Display(Name = "Overall Margin")]
        public string OverallMargin { get; set; }
        [Display(Name = "Tooling Margin")]
        public decimal ToolingMargin { get; set; }
        [Display(Name = "Fixture Margin")]
        public decimal FixtureMargin { get; set; }
        [Display(Name = "Quote?")]
        public bool IsQuote { get; set; }
        [Display(Name = "Production?")]
        public bool IsProduction { get; set; }
        [Display(Name = "No Edit?")]
        public bool NoEdit { get; set; }
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
        [Display(Name = "Price Sheet Type")]
        public string PriceSheetType { get; set; }
        [Display(Name = "Total Weight")]
        public int TotalWeight { get; set; }

        public List<PriceSheetPartViewModel> PriceSheetParts { get; set; }
        public List<PriceSheetViewModel> PriceSheets { get; set; }
        public List<PriceSheetBucket> BucketList { get; set; }
        public List<PriceSheetCostDetailViewModel> CostDetailList { get; set; }
        public List<PriceSheetPriceDetailViewModel> PriceDetailList { get; set; }
    }
}