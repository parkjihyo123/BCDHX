namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BestDeal
    {
        [StringLength(5)]
        public string BestDealID { get; set; }

        [StringLength(250)]
        public string BestDealName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        [StringLength(250)]
        public string CreateBy { get; set; }

        [StringLength(250)]
        public string ModifyBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateModify { get; set; }

        [StringLength(50)]
        public string PercentSale { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateExprie { get; set; }

        public bool? Status { get; set; }

        [StringLength(250)]
        public string Content { get; set; }
    }
}
