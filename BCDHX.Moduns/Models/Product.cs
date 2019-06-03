namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ImageForProducts = new HashSet<ImageForProduct>();
        }

        [Key]
        [StringLength(50)]
        public string ID_Product { get; set; }

        [StringLength(50)]
        public string ID_Category { get; set; }

        [StringLength(50)]
        public string Name_Product { get; set; }

        public int? Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public int? Status { get; set; }
        public bool? NewArrival { get; set; }
        public bool? BestSale { get; set; }
        [Column(TypeName = "money")]
        public decimal? Sale { get; set; }

        [StringLength(250)]
        public string Img { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageForProduct> ImageForProducts { get; set; }
    }
}
