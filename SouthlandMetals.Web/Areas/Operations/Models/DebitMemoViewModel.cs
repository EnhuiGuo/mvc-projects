using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class DebitMemoViewModel
    {
        [Display(Name = "Debit Memo#")]
        public Guid DebitMemoId { get; set; }
        [Display(Name = "Invoice Number")]
        public Guid? FoundryInvoiceId { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Debit Memo#")]
        public string DebitMemoNumber { get; set; }
        [Display(Name = "Debit Memo Date")]
        public DateTime? DebitMemoDate { get; set; }
        [Display(Name = "Debit Memo Date")]
        public string DebitMemoDateStr { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }
        [Display(Name = "Foundry")]
        public string FoundryName { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Display(Name = "Salesperson")]
        public string SalespersonId { get; set; }
        [Display(Name = "Salesperson")]
        public string SalespersonName { get; set; }
        [Display(Name = "Credit Memo")]
        public Guid CreditMemoId { get; set; }
        [Display(Name = "Credit Memo#")]
        public string CreditMemoNumber { get; set; }
        [Display(Name = "Credit Amount")]
        public decimal CreditAmount { get; set; }
        [Display(Name = "RMA#")]
        public string RmaNumber { get; set; }
        [Display(Name = "Tracking#")]
        public string TrackingNumber { get; set; }
        [Display(Name = "Amount")]
        public decimal DebitAmount { get; set; }
        [Display(Name = "Notes")]
        public string DebitMemoNotes { get; set; }
        [Display(Name = "Open?")]
        public bool IsOpen { get; set; }
        [Display(Name = "Closed?")]
        public bool IsClosed { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }
        [Display(Name = "From Date")]
        public string FromDateStr { get; set; }
        [Display(Name = "To Date")]
        public string ToDateStr { get; set; }
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }
        [Display(Name = "Part")]
        public string PartNumber { get; set; }
        [Display(Name = "Part")]
        public Guid PartId { get; set; }
        [Display(Name = "Debit Memo Item")]
        public Guid DebitMemoItemId { get; set; }
        [Display(Name = "Date")]
        public DateTime? DateCode { get; set; }
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Display(Name = "Quantity")]
        public int ItemQuantity { get; set; }
        [Display(Name = "Cost")]
        public decimal ItemCost { get; set; }
        [Display(Name = "Price")]
        public decimal ItemPrice { get; set; }
        [Display(Name = "Created")]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public List<DebitMemoViewModel> DebitMemos { get; set; }
        public List<DebitMemoItemViewModel> DebitMemoItems { get; set; }
        public List<DebitMemoAttachmentViewModel> Attachments { get; set; }
   
        public List<SelectListItem> SelectableFoundries { get; set; }
        public List<SelectListItem> SelectableFoundryInvoices { get; set; }
        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableSalespersons { get; set; }
        public List<SelectListItem> SelectableParts { get; set; }
    }
}