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
{   [CustomeAuthorizeAdmin(Roles ="Admin,Staff,Manager")]
    public class ProcessWithCategoryController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public ProcessWithCategoryController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();

        }
        public ProcessWithCategoryController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        ////
        [HttpGet]
        public JsonResult GetCategories()
        {
           
            var listCategories = from cate in _db.Categories
                                 select new CategoryViewModel
                                 {
                                     ID_Category=cate.ID_Category,
                                     Title = cate.Title,
                                     CreateBy=cate.CreateBy,
                                     CreateDate = cate.CreateDate.Value.ToString(),
                                     Isactive= cate.Isactive.Value,
                                     LastModified= cate.LastModified.Value.ToString(),
                                     LastModifiedBy = cate.LastModifiedBy,
                                     Name_Category=cate.Name_Category,
                                     PartenID=cate.PartenID,
                                     StatusCategory=cate.StatusCategory.Value
                                 };
    
            return Json(new { data = listCategories }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteCategory(string key)
        {
            var TempCategory = _db.Categories.Select(x => x).Where(x => x.ID_Category == key).SingleOrDefault();
            if (TempCategory!=null)
            {
                _db.Entry(TempCategory).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        public ActionResult EditCategory(string key,string values)
        {
            var TempCategory = _db.Categories.Select(x => x).Where(x => x.ID_Category == key).SingleOrDefault();          
            JsonConvert.PopulateObject(values, TempCategory);
            _db.Entry(TempCategory).State=System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        [HttpPost]
        public ActionResult AddCategory(string values)
        {
            CategoryDataForVadiate categoryModel = new CategoryDataForVadiate();        
            JsonConvert.PopulateObject(values, categoryModel);
            Category t = new Category
            {
                CreateBy = categoryModel.CreateBy,
                CreateDate = categoryModel.CreateDate,
                ID_Category = categoryModel.ID_Category,
                Isactive = categoryModel.Isactive,
                LastModified = categoryModel.LastModified,
                LastModifiedBy = categoryModel.LastModifiedBy,
                Name_Category = categoryModel.Name_Category,
                PartenID = categoryModel.PartenID,
                StatusCategory = categoryModel.StatusCategory,
                Title = categoryModel.Title
            };
            try
            {

                _db.Entry(t).State = System.Data.Entity.EntityState.Added;
                _db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                
            }
            
        }
        
        #region
        [HttpPost]
        public ActionResult IsVadiateIdCategory(CategoryDataForVadiate data)
        {
            bool fg = false;
            var Mess = "Mã danh mục không hợp lệ";
            
                var checkResult = _db.Categories.Where(x => x.ID_Category == data.ID_Category).SingleOrDefault();
                var checkExsit = _db.Categories.Select(x=>x).Where(x => x.ID_Category == data.ID_Category && x.Name_Category==data.Name_Category).Count();
                
                if (checkResult==null)
                {
                    return Json(new { Result = true});
                }else
                {
                if (checkExsit==1)
                {
                    return Json(new { Result = true });
                }
                else
                {
                    return Json(new { Result = fg, Message = Mess });
                }
                   
                }
            
           
        }
        [HttpPost]
        public ActionResult GetIdOfCategory()
        {
           
            var gh = _db.Categories.Select(x => new { ID_Category = x.ID_Category, Name_Category = x.Name_Category }).ToList();
            if (gh!=null)
            {
                return Json( gh );
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        #endregion


    }
}