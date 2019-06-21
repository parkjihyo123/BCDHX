using BCDHX.Areas.AdminBCDH.Models;
using BCDHX.Models;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
    [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
    public class ProcessWithProductController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public ProcessWithProductController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();

        }
        public ProcessWithProductController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        public JsonResult Getproducts()
        {

            var listProduct = from cate in _db.Products
                              select new ProductViewModel
                              {
                                  ID_Category = cate.ID_Category,
                                  BestSale = cate.BestSale,
                                  Description = cate.Description,
                                  ID_Product = cate.ID_Product,
                                  Img = cate.Img,
                                  Name_Product = cate.Name_Product,
                                  NewArrival = cate.NewArrival,
                                  Price = cate.Price,
                                  Quantity = cate.Quantity,
                                  Sale = cate.Sale,
                                  Status = cate.Status
                              };

            return Json(new { data = listProduct }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult AddProduct(string values)
        {
            Product temp = new Product();
            var t = JsonConvert.DeserializeObject(values);
            JsonConvert.PopulateObject(values, temp);
            _db.Entry(temp).State = System.Data.Entity.EntityState.Added;
           var rs = _db.SaveChanges();
            if (rs>0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        [HttpPost]
        public ActionResult EditProduct(string key,string values)
        {
            var temp = _db.Products.Where(x => x.ID_Product == key).SingleOrDefault();
            JsonConvert.PopulateObject(values,temp);
            _db.Entry(temp).State = System.Data.Entity.EntityState.Modified;
            var rs = _db.SaveChanges();
            if (rs>0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        [HttpPost]
        public ActionResult RemoveProduct(string key)
        {
            var temp = _db.Products.Where(x => x.ID_Product == key).SingleOrDefault();
            if (temp!=null)
            {
                _db.Entry(temp).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        #region
        [HttpPost]
        public JsonResult GetIdProductLookUp()
        {
            var temp = _db.Products.Select(x => new { ID_Product = x.ID_Product, Name_Product = x.Name_Product }).ToList();
            return Json(temp);
        }
        [HttpGet]
        public JsonResult GetImageForProduct(string key)
        {
            var temp = _db.ImageForProducts.Where(x => x.ID_Product == key).Select(x => x.IMG1).SingleOrDefault();
            return Json(new { data = temp }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult IsVadiateIdProduct(ProductViewModel data)
        {
            bool fg = false;
            var Mess = "Mã sản phẩm không hợp lệ";
            var productExsit = _db.Products.Where(x => x.ID_Product == data.ID_Product).SingleOrDefault();
            if (productExsit == null)
            {
                return Json(new { Result = true });
            }
            else
            {
                var productEdit = _db.Products.Where(x => x.ID_Product == data.ID_Product && x.Name_Product == data.Name_Product).SingleOrDefault();
                if (productEdit != null)
                {
                    return Json(new { Result = true });
                }
                else
                {
                    return Json(new { Result = fg, Message = Mess });
                }
            }
        }
        #endregion
    }
}