using BCDHX.Areas.AdminBCDH.Models;
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
    public class ProcessWithImageProductController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public ProcessWithImageProductController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();
        }
        public ProcessWithImageProductController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public JsonResult GetImageForProduct()
        {
            var temp = _db.ImageForProducts.Select(x => new ImageProductViewModel { ID_Product = x.ID_Product, ID_ImageForProduct = x.ID_ImageForProduct, IMG1 = x.IMG1, IMG2 = x.IMG2, IMG3 = x.IMG3, IMG4 = x.IMG4 }).ToList();
            foreach (var item in temp)
            {
                if (item.IMG2 == null || item.IMG2 == "")
                {
                    item.IMG2 = "nophoto.png";

                }
                if (item.IMG3 == null || item.IMG3 == "")
                {
                    item.IMG3 = "nophoto.png";
                }
                if (item.IMG4 == null || item.IMG4 == "")
                {
                    item.IMG4 = "nophoto.png";
                }
            }
            return Json(new { data = temp }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddImageProduct(string key, string values)
        {
            ImageForProduct temp = new ImageForProduct();
            JsonConvert.PopulateObject(values, temp);
            _db.Entry(temp).State = System.Data.Entity.EntityState.Added;
            var rs = _db.SaveChanges();
            if (rs > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }




        [HttpPost]
        public ActionResult EditImageProduct(string key, string values)
        {
            var keyImage = Convert.ToInt32(key);
            var temp = _db.ImageForProducts.Where(x => x.ID_ImageForProduct == keyImage).SingleOrDefault();

            if (temp != null)
            {
                JsonConvert.PopulateObject(values, temp);
                _db.Entry(temp).State = System.Data.Entity.EntityState.Modified;
                if (_db.SaveChanges() > 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        [HttpPost]
        public ActionResult RemoveImageProduct(string key)
        {
            var keyImage = Convert.ToInt32(key);
            var temp = _db.ImageForProducts.Where(x => x.ID_ImageForProduct == keyImage).SingleOrDefault();
            if (temp != null)
            {

                _db.Entry(temp).State = System.Data.Entity.EntityState.Deleted;
                if (_db.SaveChanges() > 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        public bool SaveResizeImage(Image img, string path)
        {
            try
            {
                // lấy chiều rộng và chiều cao ban đầu của ảnh
                // int originalW = img.Width;
                //int originalH = img.Height;
                // lấy chiều rộng và chiều cao mới tương ứng với chiều rộng truyền vào của ảnh (nó sẽ giúp ảnh của chúng ta sau khi resize vần giứ được độ cân đối của tấm ảnh
                //int resizedW = width;
                //int resizedH = (originalH * resizedW) / originalW;
                Bitmap b = new Bitmap(270, 320);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.Bicubic;    // Specify here
                g.DrawImage(img, 0, 0, 270, 320);
                g.Dispose();
                b.Save(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        [HttpPost]
        public ActionResult UploadImage1(HttpPostedFileBase Anh1, string id)
        {
            try
            {

                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = Anh1.FileName;

                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });
                if (isValidExtenstion)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForProduct/"), Path.GetFileName(Anh1.FileName));


                    if (SaveResizeImage(Image.FromStream(Anh1.InputStream), path))
                    {
                        if (SaveImageForProductToDB(fileName, id, "IMG1"))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.OK);
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
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

        [HttpPost]
        public ActionResult UploadImage2(HttpPostedFileBase Anh2, string id)
        {
            try
            {

                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = Anh2.FileName;

                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });
                if (isValidExtenstion)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForProduct/"), Path.GetFileName(Anh2.FileName));


                    if (SaveResizeImage(Image.FromStream(Anh2.InputStream), path))
                    {
                        if (SaveImageForProductToDB(fileName, id, "IMG2"))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.OK);
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
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
        [HttpPost]
        public ActionResult UploadImage3(HttpPostedFileBase Anh3, string id)
        {
            try
            {

                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = Anh3.FileName;

                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });
                if (isValidExtenstion)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForProduct/"), Path.GetFileName(Anh3.FileName));


                    if (SaveResizeImage(Image.FromStream(Anh3.InputStream), path))
                    {
                        if (SaveImageForProductToDB(fileName, id, "IMG3"))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.OK);
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
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

        [HttpPost]
        public ActionResult UploadImage4(HttpPostedFileBase Anh4, string id)
        {
            try
            {

                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = Anh4.FileName;

                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });
                if (isValidExtenstion)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForProduct/"), Path.GetFileName(Anh4.FileName));


                    if (SaveResizeImage(Image.FromStream(Anh4.InputStream), path))
                    {
                        if (SaveImageForProductToDB(fileName, id, "IMG4"))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.OK);
                        }
                        else
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
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
        public bool SaveImageForProductToDB(string imageName, string key, string typeImage)
        {
            var tempProduct = _db.ImageForProducts.Where(x => x.ID_Product == key).SingleOrDefault();
            ImageForProduct temp = new ImageForProduct();
            if (tempProduct != null)
            {

                if (typeImage.Equals("IMG1"))
                {
                    tempProduct.IMG1 = imageName;
                }
                else if (typeImage.Equals("IMG2"))
                {
                    tempProduct.IMG2 = imageName;
                }
                else if (typeImage.Equals("IMG3"))
                {
                    tempProduct.IMG3 = imageName;
                }
                else
                {
                    tempProduct.IMG4 = imageName;
                }
                _db.Entry(tempProduct).State = System.Data.Entity.EntityState.Modified;
                return _db.SaveChanges() > 0;

            }
            else
            {
                temp.ID_Product = key;
                if (typeImage.Equals("IMG1"))
                {
                    temp.IMG1 = imageName;
                }
                else if (typeImage.Equals("IMG2"))
                {
                    temp.IMG2 = imageName;
                }
                else if (typeImage.Equals("IMG3"))
                {
                    temp.IMG3 = imageName;
                }
                else
                {
                    temp.IMG4 = imageName;
                }
                _db.Entry(temp).State = System.Data.Entity.EntityState.Added;
                return _db.SaveChanges() > 0;
            }


        }
    }
}