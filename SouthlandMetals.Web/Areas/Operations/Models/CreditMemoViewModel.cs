using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class CreditMemoViewModel
    {
        [Display(Name = "Credit Memo#")]
        public Guid CreditMemoId { get; set; }

        [Display(Name = "Credit Memo#")]
        public string CreditMemoNumber { get; set; }

        [Display(Name = "Credit Memo Date")]
        public DateTime? CreditMemoDate { get; set; }

        [Display(Name = "Credit Memo Date")]
        public string CreditMemoDateStr { get; set; }

        [Display(Name = "Debit Memo#")]
        public Guid DebitMemoId { get; set; }

        [Display(Name = "Debit Memo#")]
        public string DebitMemoNumber { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Foundry")]
        public string FoundryId { get; set; }

        [Display(Name = "Salesperson")]
        public string SalespersonId { get; set; }

        [Display(Name = "Salesperson")]
        public string SalespersonName { get; set; }

        [Display(Name = "Amount")]
        public decimal CreditAmount { get; set; }

        [Display(Name = "Notes")]
        public string CreditMemoNotes { get; set; }

        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        [Display(Name = "From Date")]
        public string FromDateStr { get; set; }

        [Display(Name = "To Date")]
        public string ToDateStr { get; set; }

        [Display(Name = "RMA#")]
        public string RmaNumber { get; set; }

        [Display(Name = "Part")]
        public string PartNumber { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public List<SelectListItem> SelectableCustomers { get; set; }
        public List<SelectListItem> SelectableFoundries { get; set; }

        public List<CreditMemoViewModel> CreditMemos { get; set; }
        public List<CreditMemoItemViewModel> CreditMemoItems { get; set; }
    }
}