using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.Vendors")]
    public class DBO_Foundry
    {
        [Column]
        public string VendorID { get; set; }
        [Column]
        public string VendorName { get; set; }
        [Column]
        public string VendorShortName { get; set; }
        [Column]
        public string VendorClassID { get; set; }
        [Column]
        public string VendorContact { get; set; }
        [Column]
        public string Address1 { get; set; }
        [Column]
        public string Address2 { get; set; }
        [Column]
        public string Address3 { get; set; }
        [Column]
        public string City { get; set; }
        [Column]
        public string State { get; set; }
        [Column]
        public string ZipCode { get; set; }
        [Column]
        public string Country { get; set; }
        [Column]
        public string PhoneNumber1 { get; set; }
        [Column]
        public string PhoneNumber2 { get; set; }
        [Column]
        public string Phone3 { get; set; }
        [Column]
        public string FaxNumber { get; set; }
        [Column]
        public string PaymentTermsID { get; set; }
    }
}
