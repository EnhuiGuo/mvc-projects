using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class LayoutPdf
    {
        [Display(Name = "Success")]
        public bool Success { get; set; }
        [Display(Name = "Message")]
        public string Message { get; set; }
        [Display(Name = "Layout")]
        public Guid LayoutId { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Type")]
        public string Type { get; set; }
        [Display(Name = "Content")]
        public byte[] Content { get; set; }
        [Display(Name = "Project?")]
        public bool IsProject { get; set; }
    }
}