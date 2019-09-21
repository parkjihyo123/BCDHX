namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CuponCode")]
    public partial class CuponCode
    {
        [Key]
        public int ID_CuponCode { get; set; }

        [StringLength(8)]
        public string Code { get; set; }

        [Column(TypeName = "ntext")]
        public string ContentCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public int? NumberUse { get; set; }

        [Column(TypeName = "money")]
        public decimal? ValueSale { get; set; }

        public int? PercentSale { get; set; }
    }
}
