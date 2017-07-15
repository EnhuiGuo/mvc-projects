using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class ProjectViewModel
    {
        [Display(Name = "Success")]
        public bool Success { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Project")]
        public Guid ProjectId { get; set; }

        [Display(Name = "Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Margin")]
        public string ProjectMargin { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Created Date")]
        public string CreatedDateStr { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Last Updated")]
        public string ModifiedDateStr { get; set; }

        [Display(Name = "Updated By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Open?")]
        public bool IsOpen { get; set; }

        [Display(Name = "On Hold?")]
        public bool IsHold { get; set; }

        [Display(Name = "Canceled?")]
        public bool IsCanceled { get; set; }

        [Display(Name = "Canceled Date")]
        public DateTime? CanceledDate { get; set; }

        [Display(Name = "Canceled Date")]
        public string CanceledDateStr { get; set; }

        [Display(Name = "Cancel Notes")]
        public string CancelNotes { get; set; }

        [Display(Name = "Complete?")]
        public bool IsComplete { get; set; }

        [Display(Name = "Completed Date")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "Completed Date")]
        public string CompletedDateStr { get; set; }

        [Display(Name = "Duration")]
        public int Duration { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Contact")]
        public string CustomerContact { get; set; }

        [Display(Name = "Contact Phone")]
        public string CustomerContactPhone { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryName { get; set; }

        [Display(Name = "Foundry Contact")]
        public string FoundryContact { get; set; }

        [Display(Name = "Contact Phone")]
        public string FoundryContactPhone { get; set; }

        [Display(Name = "Hold Notes")]
        public string HoldNotes { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime? HoldExpirationDate { get; set; }

        [Display(Name = "Expiration Date")]
        public string HoldExpirationDateStr { get; set; }

        [Display(Name = "User")]
        public string CurrentUser { get; set; }

        public List<SelectListItem> SelectableProjects { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }

        public List<ProjectViewModel> Projects { get; set; }
        public List<ProjectNoteViewModel> ProjectNotes { get; set; }
        public List<ProjectPartViewModel> Parts { get; set; }
        public List<RfqViewModel> RFQs { get; set; }
        public List<PriceSheetListModel> PriceSheets { get; set; }
        public List<QuoteViewModel> Quotes { get; set; }
        public List<CustomerOrderViewModel> CustomerOrders { get; set; }
        public List<FoundryOrderViewModel> FoundryOrders { get; set; }
        public List<ShipmentViewModel> Shipments { get; set; }
    }
}