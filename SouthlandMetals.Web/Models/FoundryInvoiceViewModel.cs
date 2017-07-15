using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Models
{
    public class FoundryInvoiceViewModel
    {
        [Display(Name = "Invoice")]
        public Guid FoundryInvoiceId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryName { get; set; }
        [Display(Name = "Bill of Lading")]
        public Guid BillOfLadingId { get; set; }
        [Display(Name = "Invoice #")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Amount")]
        public decimal InvoiceAmount { get; set; }
        [Display(Name = "Air Freight")]
        public decimal AirFreight { get; set; }
        [Display(Name = "Scheduled Pay Date")]
        public DateTime? ScheduledPaymentDate { get; set; }
        [Display(Name = "Scheduled Pay Date")]
        public string ScheduledPaymentDateStr { get; set; }
        [Display(Name = "Actual Pay Date")]
        public DateTime? ActualPaymentDate { get; set; }
        [Display(Name = "Actual Pay Date")]
        public string ActualPaymentDateStr { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Unscheduled")]
        public bool Unscheduled { get; set; }
        [Display(Name = "From")]
        public DateTime FromDate { get; set; }
        [Display(Name = "From")]
        public string FromDateStr { get; set; }
        [Display(Name = "To")]
        public DateTime ToDate { get; set; }
        [Display(Name = "To")]
        public string ToDateStr { get; set; }
        [Display(Name = "Created")]
        public DateTime? CreateDate { get; set; }
        [Display(Name = "Created")]
        public string CreateDateStr { get; set; }
        [Display(Name = "Processed?")]
        public bool HasBeenProcessed { get; set; }

        public List<SelectListItem> SelectableFoundries { get; set; }

        public List<FoundryInvoiceViewModel> FoundryInvoices { get; set; }
        public List<BucketViewModel> Buckets { get; set; }
        public List<DebitMemoViewModel> DebitMemos { get; set; }
    }
}