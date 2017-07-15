using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class NotesViewModel
    {
        [Display(Name = "Notes")]
        public string HoldNotes { get; set; }
        [Display(Name = "Expiration Date")]
        public DateTime? HoldExpirationDate { get; set; }
        [Display(Name = "Expiration Date")]
        public string HoldExpirationDateStr { get; set; }
        [Display(Name = "Notes")]
        public string CancelNotes { get; set; }
        [Display(Name = " Date")]
        public DateTime? CanceledDate { get; set; }
        [Display(Name = "Date")]
        public string CanceledDateStr { get; set; }
    }
}