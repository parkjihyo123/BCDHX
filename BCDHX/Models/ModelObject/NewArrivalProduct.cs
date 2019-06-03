using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class NewArrivalProduct
    {
        private decimal _Price;
        private decimal _Sale;
        public string ID_Product { get; set; }
        public string ID_Category { get; set; }
        public string Name_Product { get; set; }
        public int Quantity { get; set; }
        public string Price
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", this._Price);
            }
            set
            {
                this._Price = Convert.ToDecimal(value);
            }

        }
        public int Status { get; set; }
        public string Sale
        {
            get
            {
                return this._Sale == 0 ? "null" : string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", this._Sale);
            }
            set
            {
                this._Sale = Convert.ToDecimal(value);
            }
        }
        public string Img { get; set; }
        public string Description { get; set; }
        public string NameCatogory { get; set; }
    }
}