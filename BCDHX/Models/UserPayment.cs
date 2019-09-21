using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models
{
   public enum UserPaymentName
    {
        PAYTOACCOUNT,COD,BANKTRANSFER,ONLINEPAYMENT,PAYWITHACCOUNTWALLTET
    }
    public enum UserPaymentStatus
    {
        SUCCESS,PEDDING,ERROR, HOLD,SHIPPING
    }
}