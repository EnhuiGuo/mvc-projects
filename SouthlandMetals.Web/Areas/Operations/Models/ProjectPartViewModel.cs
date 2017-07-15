using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class ProjectPartViewModel
    {
        [Display(Name = "Part")]
        public Guid ProjectPartId { get; set; }

        [Display(Name = "Part")]
        public Guid? PartId { get; set; }

        [Display(Name = "Description")]
        public string PartDescription { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Hts Number")]
        public Guid? HtsNumberId { get; set; }

        [Display(Name = "Hts Number")]
        public string HtsNumber { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }

        [Display(Name = "Foundry Name")]
        public string FoundryName { get; set; }

        [Display(Name = "Status")]
        public Guid? PartStatusId { get; set; }

        [Display(Name = "Status")]
        public string PartStatusDescription { get; set; }

        [Display(Name = "Type")]
        public Guid? PartTypeId { get; set; }

        [Display(Name = "Type")]
        public string PartTypeDescription { get; set; }

        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }

        [Display(Name = "Project?")]
        public bool IsProject { get; set; }
    }
}