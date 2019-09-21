using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class ProductModelCart
    {
        public string ID_Product { get; set; }
        public string ID_Category { get; set; }
        public string Name_Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public decimal Sale { get; set; }
        public string Img { get; set; }
    }
}