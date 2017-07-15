using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SouthlandMetals.Core.Domain.DBModels
{
    [Table("Vessel")]
    public class Vessel : IBaseEntity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid VesselId { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [MaxLength(256)]
        public string ModifiedBy { get; set; }
    }
}
