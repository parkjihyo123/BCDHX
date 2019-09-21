namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Invoice")]
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            InvoiceAccounts = new HashSet<InvoiceAccount>();
            InvoiceDetails = new HashSet<InvoiceDetail>();
        }

        [Key]
        [StringLength(20)]
        public string ID_Invoice { get; set; }

        [StringLength(250)]
        public string ID_Account { get; set; }

        [StringLength(250)]
        public string Shipping_Address { get; set; }

        [StringLength(50)]
        public string Payment_Methods { get; set; }

        public DateTime? Purchase_Date { get; set; }

        public bool? Status_Order { get; set; }

        [StringLength(50)]
        public string PostCode { get; set; }

        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public int? ProcessOrder { get; set; }

        [StringLength(250)]
        public string MaVanDon { get; set; }

        [Column(TypeName = "ntext")]
        public string Note { get; set; }

        [StringLength(250)]
        public string Orgin { get; set; }

        public virtual Account Account { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceAccount> InvoiceAccounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
