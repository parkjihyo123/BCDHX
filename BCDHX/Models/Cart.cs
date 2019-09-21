using BCDHX.Models.ModelObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models
{   [Serializable]
    public class Cart
    {
        public ProductModelCart Product { get; set; }
        public int Quantity { get; set; }
        public string OptionVer  { get; set; }
        public decimal ShippingLocationFee { get; set; }
        public decimal TotalGrand { get; set; }
    }

    public class CartDetail
    {
        public decimal ShippingLocationFee { get; set; }
        public decimal TotalGrand { get; set; }
        public bool UseCupon { get; set; }
    }
}