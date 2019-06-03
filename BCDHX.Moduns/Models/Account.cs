namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            Invoices = new HashSet<Invoice>();
            StockInOuts = new HashSet<StockInOut>();
        }

        [Key]
        [StringLength(250)]
        public string ID_Account { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(250)]
        public string Password { get; set; }

        public int? Access { get; set; }

        [StringLength(250)]
        public string Fullname { get; set; }

        [Column(TypeName = "ntext")]
        public string Address { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [StringLength(250)]
        public string Img { get; set; }
      
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockInOut> StockInOuts { get; set; }
    }
}


