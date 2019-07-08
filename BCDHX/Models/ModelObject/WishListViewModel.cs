using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class WishListViewModel
    {
        public string ID_Account { get; set; }
        public string IdWishList { get; set; }
        public List<Product> ListProductWishList { get; set; }
    }
}