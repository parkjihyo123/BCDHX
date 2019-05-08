namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImageForProduct")]
    public partial class ImageForProduct
    {
        [StringLength(50)]
        public string ID_Product { get; set; }

        [Key]
        public int ID_ImageForProduct { get; set; }

        [StringLength(250)]
        public string IMG1 { get; set; }

        [StringLength(250)]
        public string IMG2 { get; set; }

        [StringLength(250)]
        public string IMG3 { get; set; }

        [StringLength(250)]
        public string IMG4 { get; set; }

        public virtual Product Product { get; set; }
    }
}
