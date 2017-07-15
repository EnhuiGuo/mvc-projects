namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class FoundryOrderReportModel
    {
        public string Number { get; set; }
        public string ShipDate { get; set; }
        public string Address { get; set; }
        public string Customer { get; set; }
        public string Date { get; set; }
        public string ShipVia { get; set; }
        public string Destination { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string ShipmentTerm { get; set; }
        public string Notes { get; set; }
    }
}