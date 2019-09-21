namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class News
    {
        [Key]
        public int ID_News { get; set; }

        [StringLength(250)]
        public string NameNews { get; set; }

        [Column(TypeName = "ntext")]
        public string ContentNews { get; set; }

        public DateTime? CreateDate { get; set; }

        [StringLength(50)]
        public string ID_Category { get; set; }

        public bool? StatusNews { get; set; }

        public int? PropertyNews { get; set; }
    }
}
