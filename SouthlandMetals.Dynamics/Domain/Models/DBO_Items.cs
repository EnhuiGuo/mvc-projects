using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.Items")]
    public class DBO_Items
    {
        [Column]
        public string ItemNumber { get; set; }
        [Column]
        public string ItemDescription { get; set; }
        [Column]
        public decimal StandardCost { get; set; }
        [Column]
        public string ItemClassCode { get; set; }
        [Column]
        public string LocationCode { get; set; }
    }
}
