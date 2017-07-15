using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SouthlandMetals.Web.Areas.Administration.Models
{
    public class SalespersonViewModel
    {
        [Display(Name = "Salesperson")]
        public string SalespersonId { get; set; }
        [Display(Name = "Salesperson")]
        public string SalespersonName { get; set; }
        [Display(Name = "Sales Territory")]
        public string SalesTerritoryDescription { get; set; }
        [Display(Name = "Phone1")]
        public string Phone1 { get; set; }
        [Display(Name = "Fax")]
        public string FaxNumber { get; set; }
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }

        public List<SalespersonViewModel> Salespersons { get; set; }
        public List<CustomerViewModel> Customers { get; set; }
    }
}