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
            Silder tempImagePageBanner = _db.Silders.Where(x => x.Title == "PageBannerForProduct").SingleOrDefault()??new Silder();
            return PartialView(tempImagePageBanner);
        }
    }
}