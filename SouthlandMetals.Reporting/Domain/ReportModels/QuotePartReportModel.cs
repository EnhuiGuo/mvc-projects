namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class QuotePartReportModel
    {
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public string RevisionNumber { get; set; }
        public decimal Weight { get; set; }
        public decimal AnnualUsage { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal PatternPrice { get; set; }
        public decimal PatternCost { get; set; }
        public decimal FixturePrice { get; set; }
        public decimal FixtureCost { get; set; }
    }
}