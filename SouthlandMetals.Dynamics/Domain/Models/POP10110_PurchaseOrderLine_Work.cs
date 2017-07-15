using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.POP10110")]
    public class POP10110_PurchaseOrderLine_Work
    {
        [Column]
        public string PONUMBER { get; set; }
        [Column]
        public string VENDORID { get; set; }
        [Column]
        public string DOCDATE { get; set; }
        [Column]
        public string LOCNCODE { get; set; }
        [Column]
        public string VNDITNUM { get; set; }
        [Column]
        public string ITEMNMBR { get; set; }
        [Column]
        public decimal QUANTITY { get; set; }
        [Column]
        //DUE DATE
        public string PRMDATE { get; set; }
        [Column]
        //SHIP DATE
        public string PRMSHPDTE { get; set; }
    }
}
