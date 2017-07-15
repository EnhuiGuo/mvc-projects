using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class InventoryViewModel
    {
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; }

        [Display(Name = "PO Number")]
        public string PONumber { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "Salesperson")]
        public string SalespersonName { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }

        [Display(Name = "Zero On Hand?")]
        public bool ZeroOnHand { get; set; }

        [Display(Name = "Below Minimum?")]
        public bool BelowMinimum { get; set; }

        [Display(Name = "Below Reorder Point?")]
        public bool BelowReorderPoint { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }
    }
}