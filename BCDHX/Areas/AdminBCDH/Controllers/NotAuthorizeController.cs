using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
    public class NotAuthorizeController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public ActionResult NotAuthorized(string notAuthorized)
        {

            return View();
        }
    }
}