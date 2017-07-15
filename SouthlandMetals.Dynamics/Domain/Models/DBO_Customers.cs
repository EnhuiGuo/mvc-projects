using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.Customers")]
    public class DBO_Customers
    {
        [Column(Name = "Customer Number")]
        public string CustomerNumber { get; set; }
        [Column(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Column(Name = "Customer Class")]
        public string CustomerClass { get; set; }
        [Column(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Column(Name = "Short Name")]
        public string ShortName { get; set; }
        [Column(Name = "Address Code")]
        public string AddressCode { get; set; }
        [Column(Name = "Address 1")]
        public string Address1 { get; set; }
        [Column]
        public string Country { get; set; }
        [Column]
        public string City { get; set; }
        [Column]
        public string State { get; set; }
        [Column]
        public string Zip { get; set; }
        [Column(Name = "Phone 1")]
        public string Phone1 { get; set; }
        [Column(Name = "Salesperson ID")]
        public string SalespersonID { get; set; }
        [Column(Name = "Payment Terms ID")]
        public string PaymentTermsID { get; set; }
        [Column]
        public string Inactive { get; set; }
    }
}
