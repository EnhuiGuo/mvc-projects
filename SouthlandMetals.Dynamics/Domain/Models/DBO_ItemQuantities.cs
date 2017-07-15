using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.ItemQuantities")]
    public class DBO_ItemQuantities
    {
        [Column]
        public string ItemNumber { get; set; }
        [Column]
        public decimal QTYOnHand { get; set; }
        [Column]
        public byte INACTIVE { get; set; }
    }
}
