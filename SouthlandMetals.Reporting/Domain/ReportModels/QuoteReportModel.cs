using System;

namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class QuoteReportModel
    {
        public string Status { get; set; }
        public string Date { get; set; }
        public string QuoteNumber { get; set; }
        public string RfqNumber { get; set; }
        public string QuoteDateStr { get; set; }
        public int Validity { get; set; }
        public string ContactName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string ContactCopy { get; set; }
        public string ShipmentTermDescription { get; set; }
        public string PaymentTermDescription { get; set; }
        public string MinimumShipment { get; set; }
        public string ToolingTermDescription { get; set; }
        public string SampleLeadTime { get; set; }
        public string ProductionLeadTime { get; set; }
        public string MaterialDescription { get; set; }
        public string CoatingTypeDescription { get; set; }
        public string HtsNumberDescription { get; set; }
        public bool IsMachined { get; set; }
        public string Machining { get; set; }
        public string Notes { get; set; }
        public bool IsCanceled { get; set; }
    }
}