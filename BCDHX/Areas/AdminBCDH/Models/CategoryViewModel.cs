using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Areas.AdminBCDH.Models
{
    public class CategoryViewModel
    {
        private DateTime _createDate;
        private DateTime _LastModified;
        public string Title { get; set; }
        public string ID_Category { get; set; }
        public string Name_Category { get; set; }
        public bool Isactive { get; set; }
        public string CreateDate
        {
            get
            {
                return this._createDate.ToString();
            }


            set
            {
                this._createDate = Convert.ToDateTime(value);
            }
        }
        public string CreateBy { get; set; }
        public string LastModified {
            get
            {
                return this._LastModified.ToString();
            }
            set
            {
                this._LastModified = Convert.ToDateTime(value);
            }
        }
        public string LastModifiedBy { get; set; }
        public string PartenID { get; set; }
        public int? StatusCategory { get; set; }
    }
}