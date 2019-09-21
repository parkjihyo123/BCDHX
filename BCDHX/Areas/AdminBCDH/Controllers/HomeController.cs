using BCDHX.Models;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System;

namespace BCDHX.Areas.AdminBCDH.Controllers
{

    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public HomeController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();
        }
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [CustomeAuthorizeAdmin]
        public ActionResult Index()
        {
            return View();
        }
      
        [CustomeAuthorizeAdmin(Roles = "Admin")]

        public ActionResult ListStaffAccount()
        {
            return View();
        }
        [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
        public ActionResult Category()
        {
            return View();
        }
        [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
        public ActionResult Product()
        {
            return View();
        }
        [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
        public ActionResult PageBanner()
        {
            return View();
        }
        [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
        public ActionResult Orders()
        {
            return View();
        }
        public PartialViewResult WidgetPartial()
        {
            var dateNow = DateTime.Now.Date;
            int numberClients = _db.Accounts.Select(x => x).ToList().Count();
            ViewBag.tempNumberClients = numberClients;
            ViewBag.NumberOrderInADay = _db.Invoices.Where(x => x.Purchase_Date.Value.CompareTo(dateNow) == 0).ToList().Count();
            ViewBag.TotalPriceOrderInAday = _db.Invoices.Join(_db.InvoiceDetails, invoice => invoice.ID_Invoice, invoiceDetail => invoiceDetail.ID_Invoice, (invoice, invoiceDetail) => new { Invoice = invoice, InvoiceDetail = invoiceDetail }).Where(x => x.Invoice.Purchase_Date.Value.CompareTo(dateNow) == 0).Sum(x => x.InvoiceDetail.Price)??0;
            ViewBag.FeedBackInAday = _db.FeedBacks.Select(x => x).Where(x => x.FeedBackCreateDate.Value.CompareTo(dateNow) == 0).ToList().Count();
            return PartialView();
        }
        public PartialViewResult OrderPatialView()
        {
            return PartialView();
        }
    }
}