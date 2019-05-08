using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BCDHX.Moduns;
using BCDHX.Moduns.Unity;

namespace BCDHX.Controllers
{
    public class UploadController : Controller
    {
        RandomCode _randomcode;
        public UploadController()
        {
            _randomcode = new RandomCode();
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
                    // Uncomment to save the file
                    //var path = Server.MapPath("~/Content/ImageUploaded/ImageForUser/");
                    string path = Path.Combine(Server.MapPath("~/Content/ImageUploaded/ImageForUser/"), Path.GetFileName(ImageUp.FileName));
                    //if (!Directory.Exists(path))
                    //    Directory.CreateDirectory(path);
                    
                    if (SaveResizeImage(Image.FromStream(ImageUp.InputStream), path))
                    {
                        TempData["TempIdForUser"] = _randomcode.RandomCodeGenral(18, true);
                  
                        TempData["TempImageFileName"] = fileName;
                    }
                    
                    //Test.SaveAs(path);
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
    }
}