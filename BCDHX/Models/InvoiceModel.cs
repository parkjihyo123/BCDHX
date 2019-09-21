using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models
{
    public class InvoiceModel
    {   
        public int _Payment_Methods { get; set; }
        public string ID_Invoice { get; set; }    
        public string ID_Account { get; set; }
        public string Shipping_Address { get; set; }
        public string Payment_Methods {
            get
            {
                if (_Payment_Methods==0)
                {
                    return "Thanh toán bằng số dư!";
                }
                else if (_Payment_Methods==1)
                {
                    return "Sử dụng COD!";
                }
                else if (_Payment_Methods==2)
                {
                    return "Thanh toán bằng InternetBanking!";
                }
                else if (_Payment_Methods==3)
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
        public DateTime? Purchase_Date { get; set; }
        public bool? Status_Order { get; set; }
        public string PostCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? ProcessOrder { get; set; }
        public string MaVanDon { get; set; }
        public string Fullname { get; set; }
        public string Orginal { get; set; }
        public string  Note { get; set; }
    }
}