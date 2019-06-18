namespace BCDHX.Moduns.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AdminUser
    {
        [Key]
        [StringLength(250)]
        public string ID_AdminUser { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public int? Acess { get; set; }

        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(250)]
        public string Img { get; set; }
    }
}
