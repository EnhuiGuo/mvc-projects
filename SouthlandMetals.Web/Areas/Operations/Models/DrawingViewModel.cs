using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class DrawingViewModel
    {
        [Display(Name = "Drawing")]
        public Guid DrawingId { get; set; }
        [Display(Name = "Raw?")]
        public bool IsRaw { get; set; }
        [Display(Name = "Machined?")]
        public bool IsMachined { get; set; }
        [Display(Name = "Revision Number")]
        public string RevisionNumber { get; set; }
        [Display(Name = "Last Date")]
        public DateTime? LastDate { get; set; }
        [Display(Name = "Project?")]
        public bool IsProject { get; set; }
        [Display(Name = "Type")]
        public string Type { get; set; }
        [Display(Name = "Length")]
        public long Length { get; set; }
        [Display(Name = "Content")]
        public byte[] Content { get; set; }
        [Display(Name = "Latest?")]
        public bool IsLatest { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
    }
}