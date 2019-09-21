using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models
{
    public class CartViewModel
    {
        public decimal _Price { get; set; }
        private decimal _TotalPrice;
        public string ID_Product { get; set; }
        public string Name_Product { get; set; }
        public int Quantity { get; set; }
        public decimal _ShippingFee { get; set; }
        
        public string Price
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", _Price);
            }
            set
            {
                _Price = Convert.ToDecimal(value);
            }

        }  
        public string TotalPrice
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", this._TotalPrice);
            }
            set
            {
                this._TotalPrice = Convert.ToDecimal(value);
            }
        }
        public string Img { get; set; }
        public string Ver { get; set; }
    }
}