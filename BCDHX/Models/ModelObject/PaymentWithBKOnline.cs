using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class PaymentWithBKOnline
    {
        public string mrc_order_id { get; set; }
        public int total_amount { get; set; }
        public string description { get; set; }
        public string url_success { get; set; }
        public string url_detail { get; set; }
        public string lang { get; set; }
        public int bpm_id { get; set; }
        public int accept_bank { get; set; }
        public int accept_cc { get; set; }
        public string webhooks { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
        public string customer_name { get; set; }
        public string customer_address { get; set; }
    }
    public class PaymentResult
    {
        public int code { get; set; }
        public List<string> message { get; set; }
        public int count { get; set; }
        public PaymentDetailBK data { get; set; }
    }
    public class PaymentDetailBK{
        public string order_id { get; set; }
        public string redirect_url { get; set; }
    }

}