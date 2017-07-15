using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.CustomerAddress")]
    public class DBO_CustomerAddress
    {
        [Column]
        public string CustomerNumber { get; set; }
        [Column]
        public string AddressCode { get; set; }
        [Column]
        public string ContactPerson { get; set; }
        [Column]
        public string Address1 { get; set; }
        [Column]
        public string Address2 { get; set; }
        [Column]
        public string Address3 { get; set; }
        [Column]
        public string Country { get; set; }
        [Column]
        public string City { get; set; }
        [Column]
        public string State { get; set; }
        [Column]
        public string Zip { get; set; }
        [Column]
        public string Phone1 { get; set; }
        [Column]
        public string Phone2 { get; set; }
        [Column]
        public string Phone3 { get; set; }
        [Column]
        public string Fax { get; set; }
        [Column]
        public string SiteID { get; set; }
    }
}
