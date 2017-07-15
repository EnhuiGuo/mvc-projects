using System;
using System.Data.Linq.Mapping;

namespace SouthlandMetals.Dynamics.Domain.Models
{
    [Table(Name = "dbo.RM20101")]
    public class RM20101
    {
        [Column]
        public Int16 RMDTYPAL { get; set; }
        [Column]
        public string DOCNUMBR { get; set; }
        [Column]
        public DateTime DOCDATE { get; set; }
        [Column]
        public string BACHNUMB { get; set; }
        [Column]
        public string CUSTNMBR { get; set; }
        [Column]
        public decimal DOCAMNT { get; set; }
        [Column]
        public decimal SLSAMNT { get; set; }
    }
}
