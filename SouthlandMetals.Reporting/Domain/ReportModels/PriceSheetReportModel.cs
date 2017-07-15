namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class PriceSheetReportModel
    {
        public string Number { get; set; }
        public decimal ProjectMargin { get; set; }
        public string Foundry { get; set; }
        public string Country { get; set; }
        public string Customer { get; set; }
        public int WAF { get; set; }
        public int AnnualDollars { get; set; }
        public decimal AnnualMargin { get; set; }
        public string AnnualMarginText { get; set; }
        public int AnnualWeight { get; set; }
        public int AnnualContainer { get; set; }
        public int DollarContainer { get; set; }
        public int InsuranceFreight { get; set; }
        public int InsurancePercentage { get; set; }
        public int InsuranceDuty { get; set; }
        public int InsuranceDivisor { get; set; }
        public int InsurancePremium { get; set; }
        public decimal TotalAnnualCost { get; set; }
        public decimal TotalAnnualPrice { get; set; }
        public string OverallMargin { get; set; }
        public decimal ToolingMargin { get; set; }
        public decimal FixtureMargin { get; set; }
    }
}
