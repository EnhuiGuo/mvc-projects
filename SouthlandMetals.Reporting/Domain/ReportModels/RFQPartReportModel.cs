namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class RfqPartReportModel
    {
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public string Type { get; set; }
        public string RevisionNumber { get; set; }
        public decimal Weight { get; set; }
        public decimal AnnualUsage { get; set; }
        public string MaterialDescription { get; set; }
    }
}