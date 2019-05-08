namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InvoiceDetail")]
    public partial class InvoiceDetail
    {
        [Key]
        public int ID_InvoiceDetail { get; set; }

        [StringLength(20)]
        public string ID_Invoice { get; set; }

        [StringLength(50)]
        public string ID_Product { get; set; }

        public int? Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public bool? StatusInvoice { get; set; }

        [Column(TypeName = "money")]
        public decimal? Sale { get; set; }

        [Column(TypeName = "money")]
        public decimal? ShippingFee { get; set; }

        public int? ProcessOrder { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
