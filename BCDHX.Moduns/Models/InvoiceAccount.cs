namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InvoiceAccount")]
    public partial class InvoiceAccount
    {
        [StringLength(20)]
        public string ID_Invoice { get; set; }

        [Key]
        [StringLength(8)]
        public string ID_InvoicePaymentAccount { get; set; }

        [StringLength(250)]
        public string NameAccount { get; set; }

        [StringLength(50)]
        public string PaymentType { get; set; }

        public bool? PaymentStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PaymentDate { get; set; }

        [StringLength(8)]
        public string ID_Account { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
