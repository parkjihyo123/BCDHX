namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WishList")]
    public partial class WishList
    {
        [Key]
        [StringLength(8)]
        public string ID_Account { get; set; }

        public int? ID_WishList { get; set; }

        [StringLength(50)]
        public string ID_Product { get; set; }
    }
}
