using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BCDHX.Models;
using BCDHX.Moduns;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;

namespace BCDHX.Controllers
{
    public class UploadController : Controller
    {
        private RandomCode _randomcode;
        private WebDieuHienDB _dbBCDH;
       
        public UploadController()
        {
            _randomcode = new RandomCode();
            _dbBCDH = new WebDieuHienDB();
        }
       
        // GET: Upload
        /// <summary>
        /// upload avata của người dùng!
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        /// 

        [HttpPost]
        public void UploadImage(HttpPostedFileBase ImageUp)
        {
            try
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = ImageUp.FileName.ToLower();
                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });
                if (isValidExtenstion)
                {                   
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForUser/"), Path.GetFileName(ImageUp.FileName));                                      
                    if (SaveResizeImage(Image.FromStream(ImageUp.InputStream), path))
                    {                          
                        TempData["TempImageFileName"] = fileName;
                    }                                 
                }
            }
            catch
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //return Json("");
        }
        /// <summary>
        /// resize image
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// 
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
                Bitmap b = new Bitmap(250, 250);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.Bicubic;    // Specify here
                g.DrawImage(img, 0, 0, 250, 250);
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
        public void UploadImageForStaff(HttpPostedFileBase UpdateAvata)
        {
            try
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = UpdateAvata.FileName.ToLower();
                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });
                if (isValidExtenstion)
                {
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForUser/StaffAvata"), Path.GetFileName(UpdateAvata.FileName));
                    SaveResizeImage(Image.FromStream(UpdateAvata.InputStream), path);               
                        
                }
            }
            catch
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ChangeAvatarImage"></param>       
        [HttpPost]
        public void ProcessChangeAvatar(HttpPostedFileBase ChangeAvatarImage)
        {
            try
            {               
                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = ChangeAvatarImage.FileName.ToLower();
                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });

                if (isValidExtenstion)
                {
                    // Uncomment to save the file
                    //var path = Server.MapPath("~/Content/ImageUploaded/ImageForUser/");
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForUser/"), Path.GetFileName(ChangeAvatarImage.FileName));
                    //if (!Directory.Exists(path))
                    //    Directory.CreateDirectory(path);

                    if (SaveResizeImage(Image.FromStream(ChangeAvatarImage.InputStream), path))
                    {                  
                        
                        var tempForChange = _dbBCDH.Accounts.AsNoTracking().SingleOrDefault(x => x.Username == User.Identity.Name);
                        tempForChange.Img = fileName;
                        _dbBCDH.Entry(tempForChange).State = System.Data.Entity.EntityState.Modified;
                        _dbBCDH.SaveChanges();
                    }

                    //Test.SaveAs(path);
                }
            }
            catch
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

       [HttpPost]
       public ActionResult ProcessChangeAvatarForPrivatePerson(HttpPostedFileBase ImagePrivate)
        {
            try
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };
                var fileName = ImagePrivate.FileName.ToLower();
                var isValidExtenstion = imageExtensions.Any(ext =>
                {
                    return fileName.LastIndexOf(ext) > -1;
                });

                if (isValidExtenstion)
                {
                    // Uncomment to save the file
                    //var path = Server.MapPath("~/Content/ImageUploaded/ImageForUser/");
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForUser/"), Path.GetFileName(ImagePrivate.FileName));
                    //if (!Directory.Exists(path))
                    //    Directory.CreateDirectory(path);

                    if (SaveResizeImage(Image.FromStream(ImagePrivate.InputStream), path))
                    {

                        var tempForChange = _dbBCDH.AdminUsers.AsNoTracking().SingleOrDefault(x => x.UserName == User.Identity.Name);
                        tempForChange.Img = fileName;
                        _dbBCDH.Entry(tempForChange).State = System.Data.Entity.EntityState.Modified;
                        _dbBCDH.SaveChanges();
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }

                    //Test.SaveAs(path);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


    }
}