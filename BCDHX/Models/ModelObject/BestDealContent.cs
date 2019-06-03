using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class BestDealContent
    {
        private DateTime _createDate;
        private DateTime _DateModify;
        private DateTime _DateExprie;
        public string BestDealID { get; set; }
        public string BestDealName { get; set; }
        public DateTime CreateDate
        {

            get
            {
                return Convert.ToDateTime(string.Format("{0:MM/dd/yyyy}", this._createDate));
            }

            set
            {
                this._createDate = value;
            }
        }
        public string CreateBy { get; set; }
        public string ModifyBy { get; set; }
        public DateTime DateModify
        {

            get
            {
                return Convert.ToDateTime(string.Format("{0:MM/dd/yyyy}", this._DateModify));
            }
            set
            {
                this._DateModify = value;
            }
        }
        public string PercentSale { get; set; }
        public DateTime DateExprie
        {
            get
            {
                return Convert.ToDateTime(string.Format("{0:yyyy/MM/dd}", this._DateExprie));
            }
            set
            {
                this._DateExprie = value;
            }
        }
        public bool Status { get; set; }
        public string Content { get; set; }
    }
}