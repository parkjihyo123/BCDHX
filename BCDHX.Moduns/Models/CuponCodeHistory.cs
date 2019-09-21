namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CuponCodeHistory")]
    public partial class CuponCodeHistory
    {
        [Key]
        public int ID_CuponCodeHistory { get; set; }

        [StringLength(8)]
        public string CuponCodeUsed { get; set; }

        [StringLength(250)]
        public string ID_User { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateUsed { get; set; }
    }
}
