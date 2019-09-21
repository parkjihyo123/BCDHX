using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models
{
    public class InvoiceModelDetail
    {
        public int ID_InvoiceDetail { get; set; }
     
        public string ID_Invoice { get; set; }

        public string ID_Product { get; set; }

        public int Quantity { get; set; }
        public decimal? Price { get; set; }

        public bool? StatusInvoice { get; set; }


        public decimal? Sale { get; set; }


        public decimal? ShippingFee { get; set; }

        public int? ProcessOrder { get; set; }
    }
}