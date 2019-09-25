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
    public class PaymentDetailBK
    {
        public string order_id { get; set; }
        public string redirect_url { get; set; }
    }
    public class PaymentWebWook
    {
        public PaymentOrderDataWebWook order { get; set; }
        public PaymentTxnDataWebWook txt { get; set; }
        public string sign { get; set; }
    }
    public class PaymentOrderDataWebWook
    {
        public int id { get; set; }
        public string user_id { get; set; }
        public string mrc_order_id { get; set; }
        public string txn_id { get; set; }
        public string ref_no { get; set; }
        public string deposit_id { get; set; }
        public string merchant_id { get; set; }
        public int total_amount { get; set; }
        public int shipping_fee { get; set; }
        public string description { get; set; }
        public string url_success { get; set; }
        public string url_cancel { get; set; }
        public string stat { get; set; }////Trạng thái thanh toán đơn hàng: "p" - đang xử lý / "c" - "hoàn thành"
        public string bpm_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
    public class PaymentTxnDataWebWook
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int account_id { get; set; }
        public decimal opening_balance { get; set; }
        public int stat { get; set; }
        public string bank_ref_no { get; set; }
        public string description { get; set; }
    }

}