using System;

namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class PackingListPartReportModel
    {
        public string ShipCode { get; set; }
        public Guid PartId { get; set; }
        public string PartNumber { get; set; }
        public decimal PartWeight { get; set; }
        public string PalletNumber { get; set; }
        public int PalletQuantity { get; set; }
        public decimal PalletWeight { get; set; }
        public int PalletTotal { get; set; }
        public int TotalPalletQuantity { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNumber { get; set; }
    }
}