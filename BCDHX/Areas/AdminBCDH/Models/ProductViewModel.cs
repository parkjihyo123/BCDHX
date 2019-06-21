using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Areas.AdminBCDH.Models
{
    public class ProductViewModel
    {
        public string ID_Product { get; set; }
        public string ID_Category { get; set; }
        public string Name_Product { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? Status { get; set; }
        public decimal? Sale { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
        public bool? NewArrival { get; set; }
        public bool? BestSale { get; set; }
    }
}