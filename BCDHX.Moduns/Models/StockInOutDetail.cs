namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StockInOutDetail
    {
        public int? StockOut { get; set; }

        [StringLength(50)]
        public string StockIDProduct { get; set; }

        public int? StockIn { get; set; }

        public int? TotalSold { get; set; }

        public int ID_StockInOut { get; set; }

        [StringLength(10)]
        public string StockLeft { get; set; }

        public bool? StockStatus { get; set; }

        [Key]
        public int ID_StockInOutDetail { get; set; }

        public virtual StockInOut StockInOut { get; set; }
    }
}
