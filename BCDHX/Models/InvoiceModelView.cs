using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models
{
    public class InvoiceModelView
    {
        public decimal _Price { get; set; }
        public decimal _Sale { get; set; }
        public decimal _ShippingFee { get; set; }
        public bool _StatusInvoice { get; set; }
        public int ID_InvoiceDetail { get; set; }
        public string ID_Invoice { get; set; }
        public string ID_Product { get; set; }
        public string NameProduct { get; set; }
        public int Quantity { get; set; }
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

        public string StatusInvoice
        {
            get
            {
                if (_StatusInvoice)
                {
                    return "Thành công";
                }
                else
                {
                    return "Đã hủy";
                }
            }
            set
            {
                _StatusInvoice = Convert.ToBoolean(value);
            }
        }


        public string Sale
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", _Sale);
            }
            set
            {
                _Sale = Convert.ToDecimal(value);
            }

        }
        public string ShippingFee
        {
            get
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", _ShippingFee);
            }
            set
            {
                _ShippingFee = Convert.ToDecimal(value);
            }

        }

        public int? ProcessOrder { get; set; }
    }
}