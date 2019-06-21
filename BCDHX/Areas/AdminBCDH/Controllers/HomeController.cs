using BCDHX.Moduns.Unity;
using System.Web.Mvc;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
   
    public class HomeController : Controller
    {
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
    }
}