using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class CuponModel
    {  
        public int ID_CuponCode { get; set; }
        public string Code { get; set; }
        public string ContentCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberUse { get; set; }
        public decimal? ValueSale { get; set; }
        public int? PercentSale { get; set; }
    }
}