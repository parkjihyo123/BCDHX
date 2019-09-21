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
using System.Web.Security;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
    [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
    public class ProcessOrderController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public ProcessOrderController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();

        }
        public ProcessOrderController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public JsonResult PageOrders()
        {
            var temp = GetOrders();
            return Json(new { data = temp }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetOrderDetail(string key)
        {
            var temp = _db.InvoiceDetails.Where(x => x.ID_Invoice == key).Select(x=>new OrderDetailViewModel {IdInvoice=x.ID_Invoice,IdProduct=x.ID_Product,Price=x.Price.Value.ToString(),Quantity=x.Quantity.Value}).ToList();
            foreach (var item in GetOrders())
            {
                foreach (var itemDetail in temp)
                {
                    if (itemDetail.IdInvoice==item.IdInvoice)
                    {
                        itemDetail._Payment_Methods = item._Payment_Methods;
                        itemDetail._ProcessOrder = item._ProcessOrder;
                        itemDetail.ShippingAddressDetail = item.ShippingAddress;
                    }
                }
            }
            return Json(new { data = temp }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditPageOrder(string key,string values)
        {
            var tempOrder = _db.Invoices.AsNoTracking().Where(x => x.ID_Invoice == key).SingleOrDefault();
            if (tempOrder!=null)
            {
                JsonConvert.PopulateObject(values, tempOrder);
                _db.Entry(tempOrder).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
           return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        
        private List<OrderViewModel> GetOrders()
        {
            var temp = _db.Invoices.Select(x => new OrderViewModel { IdInvoice = x.ID_Invoice,IdAccount=x.ID_Account,ShippingAddress=x.Shipping_Address,Note=x.Note,OrderDate=x.Purchase_Date.Value.ToString(),Payment_Methods=x.Payment_Methods,ProcessOrder=x.ProcessOrder.ToString(),Status_Order=x.Status_Order.Value,MaVanDon=x.MaVanDon}).ToList();
            return temp;
        }
    }
}