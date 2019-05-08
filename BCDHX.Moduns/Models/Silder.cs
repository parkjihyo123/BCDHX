namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Silder
    {
        [Key]
        public int SliderID { get; set; }

        [StringLength(250)]
        public string NameSlider { get; set; }

        [StringLength(250)]
        public string Img { get; set; }

        public bool? Status { get; set; }

        [StringLength(250)]
        public string Title { get; set; }
    }
}
