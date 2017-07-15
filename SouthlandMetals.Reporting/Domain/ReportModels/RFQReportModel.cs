using System;

namespace SouthlandMetals.Reporting.Domain.Models.ReportModels
{
    public class RfqReportModel
    {
        public string RfqNumber { get; set; }
        public string Date { get; set; }
        public string RfqDateStr { get; set; }
        public string CustomerName { get; set; }
        public string ProjectName { get; set; }
        public string CountryName { get; set; }
        public string ContactName { get; set; }
        public string SalespersonName { get; set; }
        public string Attention { get; set; }
        public string PrintsSent { get; set; }
        public string SentVia { get; set; }
        public bool IsMachined { get; set; }
        public string ShipmentTermDescription { get; set; }
        public string Packaging { get; set; }
        public int NumberOfSamples { get; set; }
        public string Details { get; set; }
        public string CoatingTypeDescription { get; set; }
        public string SpecificationMaterialDescription { get; set; }
        public bool ISIRRequired { get; set; }
        public bool SampleCastingAvailable { get; set; }
        public bool MetalCertAvailable { get; set; }
        public bool CMTRRequired { get; set; }
        public bool GaugingRequired { get; set; }
        public bool TestBarsRequired { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string QuotePriceSheet { get; set; }
        public string ProductionPriceSheet { get; set; }
        public string FoundryName { get; set; }
        public bool IsCanceled { get; set; }
    }
}