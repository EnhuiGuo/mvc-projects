using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Operations.Models
{
    public class NewAttachmentModel
    {
        [Display(Name = "Success")]
        public bool Success { get; set; }
        [Display(Name = "Message")]
        public string Message { get; set; }
        [Display(Name = "Debit Memo Attachment")]
        public Guid DebitMemoAttachmentId { get; set; }
        [Display(Name = "Debit Memo")]
        public Guid DebitMemoId { get; set; }
        [Display(Name = "Type")]
        public string Type { get; set; }
        [Display(Name = "Content")]
        public byte[] Content { get; set; }
        [Display(Name = "Attachment")]
        public string AttachmentName { get; set; }
    }
}