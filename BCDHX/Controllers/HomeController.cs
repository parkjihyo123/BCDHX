using BCDHX.Models.ModelObject;
using BCDHX.Moduns.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace BCDHX.Controllers
{
    public class HomeController : Controller
    {
        private WebDieuHienDB _db;
        public HomeController()
        {
            _db = new WebDieuHienDB();
        }
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult PageBannerForProductGridPartial()
        {
            Silder tempImagePageBanner = _db.Silders.Where(x => x.Title == "PageBannerForProduct" && x.Status==true).SingleOrDefault()??new Silder();
            return PartialView(tempImagePageBanner);
        }
        public PartialViewResult HeroSectionPartial()
        {
            var tempHeroSection = _db.Silders.Where(x => x.Title == "PageBannerHeroSection" && x.Status == true).ToList() ?? new List<Silder>();
            return PartialView(tempHeroSection);
        }
        public PartialViewResult ProductMenuTree()
        {   
            List<Category> categories_official = _db.Categories.Select(x => x).ToList();
            List<TreeNode> headerTree_Official = FillRecursive(categories_official, null);     
            return PartialView(headerTree_Official);
        }
        private static List<TreeNode> FillRecursive(List<Category> flatObjects, string parentId = null)
        {
            return flatObjects.Where(x => x.PartenID==parentId).Select(item => new TreeNode
            {
                CategoryName = item.Name_Category,
                CategoryID = item.ID_Category,
                Children = FillRecursive(flatObjects, item.ID_Category)
            }).ToList();
        }
      
    }
}

