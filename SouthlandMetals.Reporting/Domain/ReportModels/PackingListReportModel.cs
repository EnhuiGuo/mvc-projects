namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class PackingListReportModel
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string ShipDate { get; set; }
        public string CarrierName { get; set; }
        public string Notes { get; set; }
    }
}