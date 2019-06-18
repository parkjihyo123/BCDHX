using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BCDHX.Areas.AdminBCDH.Models
{
    public class CategoryDataWithDB
    {
        private string _CreateDate;
        private string _LastModified;
        [StringLength(250)]
        public string Title { get; set; }
        [Key]
        [StringLength(50)]
        public string ID_Category { get; set; }
        [StringLength(250)]
        public string Name_Category { get; set; }
        public bool? Isactive { get; set; }
        [Column(TypeName = "date")]
        public DateTime? CreateDate
        {
            get
            {
                return Convert.ToDateTime(this._CreateDate);
            }
            set
            {
                this._CreateDate = value.ToString();
            }
        }
        [StringLength(250)]
        public string CreateBy { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LastModified
        {
            get
            {
                return Convert.ToDateTime(this._LastModified);
            }
            set
            {
                this._LastModified = value.ToString();
            }
        }
        [StringLength(250)]
        public string LastModifiedBy { get; set; }
        public string PartenID { get; set; }
        public int? StatusCategory { get; set; }
    }
}