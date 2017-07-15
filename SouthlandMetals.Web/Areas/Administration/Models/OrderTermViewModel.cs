using System;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Administration.Models
{
    public class OrderTermViewModel
    {
        [Display(Name = "Terms")]
        public Guid OrderTermId { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "Terms")]
        public string OrderTermsDescription { get; set; }
    }
}