using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class AttachmentModel
    {
        [Display(Name = "Attachment")]
        public HttpPostedFileBase Attachment { get; set; }

        [Display(Name = "Debit Memo")]
        public Guid DebitMemoId { get; set; }
    }
}