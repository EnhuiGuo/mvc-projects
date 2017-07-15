
namespace SouthlandMetals.Reporting.Domain.ReportModels
{
    public class OpenOrdersReportModel
    {
        public string FoundryId { get; set; }
        public string CustomerId { get; set; }
        public string OrderTypeId { get; set; }
        public string PartNumber { get; set; }
        public string ShipCode { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string Foundry { get; set; }
        public string Customer { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string ShipDate { get; set; }
        public string PONumber { get; set; }
        public string PODate { get; set; }
        public int Quantity { get; set; }
        public string DueDate { get; set; }
        public bool IsSample { get; set; }
        public bool IsTooling { get; set; }
        public bool IsProduction { get; set; }
        public bool IsScheduled { get; set; }
    }
}