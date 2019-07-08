using BCDHX.Models;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
    [CustomeAuthorizeAdmin(Roles = "Admin,Staff,Manager")]
    public class ManagerController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public ManagerController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();
        }
        public ManagerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        
        public async Task<ActionResult> Index()
        {
            var tempUserOrginData = await UserManager.FindByEmailAsync(User.Identity.Name);
            var tempUser = _db.AdminUsers.Select(x=>new UserAdminModelAdmin {Access= x.Acess.Value.ToString(),Fullname=x.FullName,ID_AdminUser=x.ID_AdminUser,Img=x.Img,Password=x.Password,Username=x.UserName }).Where(x => x.ID_AdminUser==tempUserOrginData.Id).SingleOrDefault();
           
            return View(tempUser);
        }
        
        [HttpPost]
        public JsonResult ValidateOldPassword(string oldpassword)
        {
            var UserId = User.Identity.GetUserId();
            var rs = _db.AdminUsers.Where(x => x.Password == oldpassword && x.ID_AdminUser== UserId).SingleOrDefault();
            if (rs!=null)
            {
                return Json(new { Result = true });
            }
            return Json(new { Result = false });
        }
        [HttpPost]
        public async Task<JsonResult> UpdatePasswordPrivate(string NewPassword)
        {
            var UserTemp = UserManager.FindById(User.Identity.GetUserId());
            if (await ChangePassword(NewPassword,UserTemp.Id))
            {
                var UserTempAdminAccount = _db.AdminUsers.Where(x => x.ID_AdminUser == UserTemp.Id).SingleOrDefault();
                UserTempAdminAccount.Password = NewPassword;
                _db.Entry(UserTempAdminAccount).State = System.Data.Entity.EntityState.Modified;
                return _db.SaveChanges()>0? Json(new { Result=true}):Json(new { Result=false });
            }
            return Json(new { Result = false });


        }
        [HttpPost]
        public JsonResult UpdateFullnamePrivate(string Fullname)
        {
            if (Fullname!=null)
            {
                var UserTemp = UserManager.FindById(User.Identity.GetUserId());
                var UserTempAdminAccount = _db.AdminUsers.Where(x => x.ID_AdminUser == UserTemp.Id).SingleOrDefault();
                UserTempAdminAccount.FullName = Fullname;
                _db.Entry(UserTempAdminAccount).State = System.Data.Entity.EntityState.Modified;
                    return _db.SaveChanges() > 0 ? Json(new { Result = true }) : Json(new { Result = false });
            }
            return Json(new { Result = false });
        }

        private async Task<bool> ChangePassword(string key,string id)
        {
            var TempUserOrgin = _db.AdminUsers.AsNoTracking().SingleOrDefault(x => x.ID_AdminUser == key);
            
                IdentityResult resultValidate = await UserManager.PasswordValidator.ValidateAsync(key);
                if (resultValidate.Succeeded)
                {
                    var CodedRestPasswor = await UserManager.GeneratePasswordResetTokenAsync(id);
                    var result = await UserManager.ResetPasswordAsync(id, CodedRestPasswor, key);
                    return result.Succeeded;
                }
                else
                {
                    return false;
                }
        }
        #region
        [HttpGet]
        public JsonResult GetImageStaffAvatar()
        {
            var UserId = User.Identity.GetUserId();
            var TempAvatar = _db.AdminUsers.Where(x => x.ID_AdminUser == UserId).Select(x => x.Img).SingleOrDefault();
            return Json(new {IMG = TempAvatar},JsonRequestBehavior.AllowGet);
        }
#endregion

    }
}