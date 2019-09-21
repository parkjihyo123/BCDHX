using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCDHX.Models.ModelObject
{
    public class TreeNode
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public TreeNode ParentCategory { get; set; }
        public List<TreeNode> Children { get; set; }
    }
}