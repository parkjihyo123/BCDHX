using BCDHX.Models;
using BCDHX.Models.ModelObject;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using BCDHX.Unity.PaymentBK;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BCDHX.Controllers
{
    [Authorize(Roles ="Customer")]
    public class PaymentResultController : Controller
    {
        private const string CartSession = "CartSession";
        private readonly WebDieuHienDB _dbBCDH;
        private ApplicationUserManager _userManager;
        private readonly PaymentLibrary _paymentLibrary;
        public PaymentResultController()
        {
            _dbBCDH = new WebDieuHienDB();
            _paymentLibrary = new PaymentLibrary();
        }
        public PaymentResultController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;

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
        // GET: PaymentResult
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult WebHookResult(HttpContext context)
        {
            var jsonSerialzer = new JavaScriptSerializer();
            var jsonString = string.Empty;
            context.Request.InputStream.Position = 0;
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }
            var data = new PaymentWebWook();
            data = jsonSerialzer.Deserialize<PaymentWebWook>(jsonString);
            var number_ramdom = new RandomCode().RandomNumber(2);
            LinkSystem mg = new LinkSystem {ID_LinkSystem= Convert.ToInt32(number_ramdom)};
            _dbBCDH.Entry(mg).State = System.Data.Entity.EntityState.Added;
            _dbBCDH.SaveChanges();
            return new EmptyResult();
        }
        public ActionResult ShowTest()
        {
            

            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ShowTest(string order)
        {
            
            var vnpayData = Request.QueryString;
            foreach (var item in vnpayData)
            {
                if (item!=null)
                {
                    ViewBag.TEST = item;
                }
                
            }
            ViewBag.X = order;

            return View();
        }
        public ActionResult ResultUseBalance(string id)
        {
            if (id!=null)
            {
                if (GetInvoieDetail(id).Count()==0 || GetInvoie(id)==null)
                {
                    return RedirectToAction("Index", "NotificationSystem");
                }else
                {
                    ViewBag.tempInvoieDetail = GetInvoieDetail(id);
                    ViewBag.tempInvoie = GetInvoie(id);
                    return View();

                }
            }else
            {
                return RedirectToAction("Index", "NotificationSystem");
            }

        }
       
       
        #region
        private InvoiceModel GetInvoie(string InvoiceID)
        {

            if (InvoiceID != null)
            {
                //var temp = _dbBCDH.InvoiceDetails.Select(x=>new InvoiceModelView {ID_Invoice = x.ID_Invoice,ID_InvoiceDetail=x.ID_InvoiceDetail,ID_Product=x.ID_Product,_Price=x.Price.Value,_Sale=x.Sale.Value,_ShippingFee=x.ShippingFee.Value,_StatusInvoice=x.StatusInvoice.Value,Quantity=x.Quantity.Value,ProcessOrder=x.ProcessOrder }).Where(x => x.ID_Invoice == InvoiceID).ToList();
                var temp = _dbBCDH.Invoices.Join(_dbBCDH.Accounts, invoice => invoice.ID_Account, account => account.ID_Account, (invoice, account) => new {Invoice=invoice,Account=account}).Select(x => new InvoiceModel { ID_Invoice = x.Invoice.ID_Invoice, Email=x.Invoice.Email,ID_Account=x.Account.ID_Account,Fullname=x.Account.Fullname,MaVanDon=x.Invoice.MaVanDon,Note=x.Invoice.Note,Orginal=x.Invoice.Orgin,Payment_Methods=x.Invoice.Payment_Methods,Phone=x.Invoice.Phone,PostCode=x.Invoice.PostCode,ProcessOrder=x.Invoice.ProcessOrder,Purchase_Date=x.Invoice.Purchase_Date,Shipping_Address=x.Invoice.Shipping_Address,Status_Order=x.Invoice.Status_Order}).Where(x => x.ID_Invoice == InvoiceID).SingleOrDefault();
                if (temp!=null)
                {
                    return temp;
                }
                else
                {
                    return temp = new InvoiceModel();
                }
            }
            else
            {
                return new InvoiceModel();
            }
        }
        private List<InvoiceModelView> GetInvoieDetail(string InvoiceID)
        {

            if (InvoiceID != null)
            {
                //var temp = _dbBCDH.InvoiceDetails.Select(x=>new InvoiceModelView {ID_Invoice = x.ID_Invoice,ID_InvoiceDetail=x.ID_InvoiceDetail,ID_Product=x.ID_Product,_Price=x.Price.Value,_Sale=x.Sale.Value,_ShippingFee=x.ShippingFee.Value,_StatusInvoice=x.StatusInvoice.Value,Quantity=x.Quantity.Value,ProcessOrder=x.ProcessOrder }).Where(x => x.ID_Invoice == InvoiceID).ToList();
                var temp = _dbBCDH.InvoiceDetails.Join(_dbBCDH.Products, invoiceDetail => invoiceDetail.ID_Product, product => product.ID_Product, (invoiceDetail, product) => new { InvoiceDetail = invoiceDetail, Product = product }).Select(x => new InvoiceModelView { ID_Invoice = x.InvoiceDetail.ID_Invoice, ID_InvoiceDetail = x.InvoiceDetail.ID_InvoiceDetail, ID_Product = x.InvoiceDetail.ID_Product, _Price = x.InvoiceDetail.Price.Value,  _ShippingFee = x.InvoiceDetail.ShippingFee.Value, _StatusInvoice = x.InvoiceDetail.StatusInvoice.Value, Quantity = x.InvoiceDetail.Quantity.Value, ProcessOrder = x.InvoiceDetail.ProcessOrder, NameProduct = x.Product.Name_Product }).Where(x => x.ID_Invoice == InvoiceID).ToList();
                if (temp.Count > 0)
                {
                    return temp;
                }
                else
                {
                    return temp = new List<InvoiceModelView>();
                }
            }
            else
            {
                return new List<InvoiceModelView>();
            }
        }
        public PartialViewResult PageBannerPaymentresult()
        {

            Silder tempImagePageBanner = _dbBCDH.Silders.Where(x => x.Title == "PageBannerForResult").SingleOrDefault() ?? new Silder();
            return PartialView(tempImagePageBanner);
        }
        #endregion
    }
}