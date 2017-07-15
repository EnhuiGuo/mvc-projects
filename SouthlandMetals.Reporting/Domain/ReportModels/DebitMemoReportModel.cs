using System;

namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class DebitMemoReportModel
    {
        public string DebitMemoNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string FoundryId { get; set; }
        public string FoundryName { get; set; }
        public string RmaNumber { get; set; }
        public string TrackingNumber { get; set; }
        public decimal DebitAmount { get; set; }
        public DateTime? DebitMemoDate { get; set; }
        public string DateCode { get; set; }
        public string Reason { get; set; }
        public string DebitMemoDateStr { get; set; }
        public string DebitMemoNotes { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal ExtendedCost { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }
    }
}