using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class RfqPartViewModel
    {
        [Display(Name = "Part")]
        public Guid ProjectPartId { get; set; }
        [Display(Name = "Part")]
        public Guid? PartId { get; set; }
        [Display(Name = "Number")]
        public string PartNumber { get; set; }
        [Display(Name = "Description")]
        public string PartDescription { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "PartDrawing")]
        public Guid PartDrawingId { get; set; }
        [Display(Name = "Raw?")]
        public bool IsRaw { get; set; }
        [Display(Name = "Machine?")]
        public bool IsMachined { get; set; }
        [Display(Name = "Revision")]
        public string RevisionNumber { get; set; }
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Est. Annual Usage")]
        public int AnnualUsage { get; set; }
        [Display(Name = "Material")]
        public Guid? MaterialId { get; set; }
        [Display(Name = "Material Description")]
        public string MaterialDescription { get; set; }
        [Display(Name = "Type")]
        public Guid? PartTypeId { get; set; }
        [Display(Name = "Status")]
        public Guid? PartStatusId { get; set; }
        [Display(Name = "Destination")]
        public Guid? DestinationId { get; set; }
        [Display(Name = "Surcharge")]
        public Guid? SurchargeId { get; set; }
        [Display(Name = "Sub Foundry")]
        public string SubFoundryId { get; set; }
        [Display(Name = "New?")]
        public bool IsNew { get; set; }

        public DrawingViewModel Drawing { get; set; }
    }
}