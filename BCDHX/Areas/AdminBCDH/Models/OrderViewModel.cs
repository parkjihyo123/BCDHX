using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCDHX.Models;
namespace BCDHX.Areas.AdminBCDH.Models
{
    public class OrderDetailViewModel
    {
        public int _Payment_Methods { get; set; }
        public decimal _Price { get; set; }
        public int _ProcessOrder { get; set; }
        public string IdInvoice { get; set; }
        public string IdProduct { get; set; }
        public int Quantity { get; set; }
        public string ShippingAddressDetail { get; set; }
        public string ProcessOrder
        {
            get
            {

                if ((int)UserPaymentStatus.PEDDING == _ProcessOrder)
                {
                    return "Đã tiếp nhận đơn hàng!";
                }
                else if ((int)UserPaymentStatus.ERROR == _ProcessOrder)
                {
                    return "Xảy ra lỗi khi thanh toán hoặc đơn hàng có lỗi!";
                }
                else if ((int)UserPaymentStatus.HOLD == _ProcessOrder)
                {
                    return "Đơn hàng tạm giữ!(Tạm ngưng!)";
                }
                else if ((int)UserPaymentStatus.SUCCESS == _ProcessOrder)
                {
                    return "Đơn hàng đã rao thành công!";
                }
                else
                {
                    return "Đơn hàng đang được ship!";
                }
            }
            set
            {
                _ProcessOrder = Convert.ToInt32(value);
            }

        }
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
        public string Payment_Methods
        {
            get
            {
                if (_Payment_Methods == 0)
                {
                    return "Thanh toán bằng số dư!";
                }
                else if (_Payment_Methods == 1)
                {
                    return "Sử dụng COD!";
                }
                else if (_Payment_Methods == 2)
                {
                    return "Thanh toán bằng InternetBanking!";
                }
                else if (_Payment_Methods == 3)
                {
                    return "Thanh toán bằng cổng thanh toán điện tử!";
                }
                else
                {
                    return "Nạp tiền vô tài khoản!";
                }
            }
            set
            {
                _Payment_Methods = Convert.ToInt32(value);
            }
        }
    }
    public class OrderViewModel
    {
        public int _ProcessOrder { get; set; }
        public DateTime _OrderDate { get; set; }
        public int _Payment_Methods { get; set; }
        public string IdInvoice { get; set; }
        public string IdAccount { get; set; }
        public string ShippingAddress { get; set; }
        public string OrderDate {
            get
            {
                return String.Format("{0:MM/dd/yy H:mm:ss zzz}", _OrderDate);
            }
            set
            {
                _OrderDate = Convert.ToDateTime(value);
            }
        }
        public bool Status_Order { get; set; }
        public string MaVanDon { get; set; }
        public string Note { get; set; }
        public string Payment_Methods
        {
            get
            {
                if (_Payment_Methods == 0)
                {
                    return "Thanh toán bằng số dư!";
                }
                else if (_Payment_Methods == 1)
                {
                    return "Sử dụng COD!";
                }
                else if (_Payment_Methods == 2)
                {
                    return "Thanh toán bằng InternetBanking!";
                }
                else if (_Payment_Methods == 3)
                {
                    return "Thanh toán bằng cổng thanh toán điện tử!";
                }
                else
                {
                    return "Nạp tiền vô tài khoản!";
                }
            }
            set
            {
                _Payment_Methods = Convert.ToInt32(value);
            }
        }
        public string ProcessOrder
        {
            get
            {

                if ((int)UserPaymentStatus.PEDDING == _ProcessOrder)
                {
                    return "Đã tiếp nhận đơn hàng!";
                }
                else if ((int)UserPaymentStatus.ERROR == _ProcessOrder)
                {
                    return "Xảy ra lỗi khi thanh toán hoặc đơn hàng có lỗi!";
                }
                else if ((int)UserPaymentStatus.HOLD == _ProcessOrder)
                {
                    return "Đơn hàng tạm giữ!(Tạm ngưng!)";
                }
                else if ((int)UserPaymentStatus.SUCCESS == _ProcessOrder)
                {
                    return "Đơn hàng đã giao thành công!";
                }
                else
                {
                    return "Đơn hàng đang được ship!";
                }
            }
            set
            {
                _ProcessOrder = Convert.ToInt32(value);
            }

        }
    }
}