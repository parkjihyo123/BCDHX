using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class PagingProduct
    {
        public List<Product> Products { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
    }
}