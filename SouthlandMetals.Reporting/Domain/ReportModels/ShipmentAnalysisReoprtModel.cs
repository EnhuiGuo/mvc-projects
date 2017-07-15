namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class ShipmentAnalysisReoprtModel
    {
        public string ShipCode { get; set; }
        public string Customer { get; set; }
        public string BillOfLadingDate { get; set; }
        public string Date { get; set; }
        public string Foundry { get; set; }
        public string Shipped { get; set; }
        public string Vessel { get; set; }
        public string FoundryInvoice { get; set; }
        public string BucketName { get; set; }
        public string PartNumber { get; set; }
        public string PurchaseOrder { get; set; }
        public decimal CITotal { get; set; }
        public decimal OceanFrt { get; set; }
        public decimal RealCAndF { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal Cost { get; set; }
        public decimal Extension { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Overcharge { get; set; }
        public decimal BucketValue { get; set; }
        public int Quantity { get; set; }
        public bool JoinEqual { get; set; }
    }
}