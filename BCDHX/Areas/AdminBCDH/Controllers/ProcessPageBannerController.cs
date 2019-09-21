using BCDHX.Models;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
    [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
    public class ProcessPageBannerController : Controller
    {
        // GET: AdminBCDH/PageBanner
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public ProcessPageBannerController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();

        }
        public ProcessPageBannerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public JsonResult PageBanners()
        {
            var temp = _db.Silders.Select(x => x).ToList(); 
            return Json(new {data=temp },JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPageBanner(string values,string key)
        {
            if (key!=null)
            {
                var id = Convert.ToInt32(key);
                Silder temp = _db.Silders.AsNoTracking().Where(x => x.SliderID == id).SingleOrDefault();
                JsonConvert.PopulateObject(values, temp);
                _db.Entry(temp).State = System.Data.Entity.EntityState.Modified;
                var rs = _db.SaveChanges();
                if (rs > 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
        }
        [HttpPost]
        public ActionResult DeletePageBanner(string key)
        {
            var id = Convert.ToUInt32(key);
            Silder temp = _db.Silders.Where(x => x.SliderID == id).SingleOrDefault();
            _db.Entry(temp).State = System.Data.Entity.EntityState.Deleted;
            var rs = _db.SaveChanges();
            if (rs > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        }
        [HttpPost]
        public ActionResult AddPageBanner(string values, string key)
        {
            Silder temp = new Silder ();
            JsonConvert.PopulateObject(values, temp);
            _db.Entry(temp).State = System.Data.Entity.EntityState.Added;
            var rs = _db.SaveChanges();
            if (rs > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
           
        }
        public ActionResult UploadPageBanner(HttpPostedFileBase ImageSilder, string id)
        {
            try
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = ImageSilder.FileName;
                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });
                if (isValidExtenstion)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageBanner/"), Path.GetFileName(ImageSilder.FileName));
                    ImageSilder.SaveAs(path);
                    if (!id.Equals("undefined"))
                    {
                        var key = Convert.ToInt32(id);
                        var tempSilder = _db.Silders.Where(x => x.SliderID == key).SingleOrDefault();
                        tempSilder.Img = fileName;
                        _db.Entry(tempSilder).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

        }
    }
}