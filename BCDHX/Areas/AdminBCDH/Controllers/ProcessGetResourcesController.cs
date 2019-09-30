using BCDHX.Models;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNet.Identity;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
    [CustomeAuthorizeAdmin(Roles = "Admin")]
    public class ProcessGetResourcesController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        public ProcessGetResourcesController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();

        }
        public ProcessGetResourcesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [HttpPost]
        public JsonResult GetListStaffAccount()
        {
            var ListAccount = _db.AdminUsers.Select(x => x).ToList();
            return Json(ListAccount);
        }

        [HttpPost]
        public async Task<ActionResult> AddStaffAccount(string values)
        {
            AdminUser userAdmin = new AdminUser();
            JsonConvert.PopulateObject(values, userAdmin);
            var user = new ApplicationUser { UserName = userAdmin.UserName, Email = userAdmin.UserName };
            var result = await UserManager.CreateAsync(user, userAdmin.Password);
            if (result.Succeeded)
            {
                ApplicationUser currentUser = _dbasp.Users.FirstOrDefault(x => x.Email.Equals(userAdmin.UserName));
                await UserManager.AddToRoleAsync(currentUser.Id, ConvertAccsessToRole(userAdmin.Acess.Value));
                currentUser.EmailConfirmed = true;
                userAdmin.ID_AdminUser = currentUser.Id;
                _db.Entry(userAdmin).State = System.Data.Entity.EntityState.Added;
                _db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public string ConvertAccsessToRole(int access)
        {
            if (access == 2)
            {
                return "Admin";
            }
            else if (access == 3)
            {
                return "Staff";
            }
            else
            {
                return "Manager";
            }
        }
        [HttpPost]
        public async Task<ActionResult> UpdateStaffAccount(string key, string values)
        {

            if (values != null)
            {
                var TempUser = _db.AdminUsers.AsNoTracking().SingleOrDefault(k => k.ID_AdminUser == key);
                var TempUserAccountAdmin = _dbasp.Users.Select(g => g).Where(x => x.Id == key).SingleOrDefault();
                _dbasp.Users.Select(x => x).Where(x => x.Id == key).Single();
                JsonConvert.PopulateObject(values, TempUser);
                if (await ChangePassword(key, TempUser) && await ChangeRoleStaff(key, TempUser) && ChangeUsername(key, TempUser))
                {
                    _db.Entry(TempUser).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return Json(new { Error = HttpStatusCode.OK });
            }

        }

        [HttpPost]
        public ActionResult RemoveStaffAccount(string key)
        {
            var StaffAccountFromAsp = _db.AdminUsers.AsNoTracking().Where(x => x.ID_AdminUser == key).SingleOrDefault();
            var StaffAccount = UserManager.FindById(key);
            if (StaffAccountFromAsp == null || StaffAccount == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                _db.Entry(StaffAccountFromAsp).State = System.Data.Entity.EntityState.Deleted;
                _db.SaveChanges();
                IdentityResult resultRemove = UserManager.Delete(StaffAccount);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

        }
        private async Task<bool> ChangePassword(string key, AdminUser NewAdminUser)
        {
            var TempUserOrgin = _db.AdminUsers.AsNoTracking().SingleOrDefault(x => x.ID_AdminUser == key);
            if (TempUserOrgin.Password != NewAdminUser.Password)
            {
                IdentityResult resultValidate = await UserManager.PasswordValidator.ValidateAsync(NewAdminUser.Password);
                if (resultValidate.Succeeded)
                {
                    var CodedRestPasswor = await UserManager.GeneratePasswordResetTokenAsync(NewAdminUser.ID_AdminUser);
                    var result = await UserManager.ResetPasswordAsync(NewAdminUser.ID_AdminUser, CodedRestPasswor, NewAdminUser.Password);
                    return result.Succeeded;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return true;
            }
        }
        private bool ChangeUsername(string key, AdminUser user)
        {
            var newUser = _dbasp.Users.Where(x => x.Id == key).SingleOrDefault();
            newUser.UserName = user.UserName;
            newUser.Email = user.UserName;
            newUser.EmailConfirmed = true;
            _dbasp.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
            _dbasp.SaveChanges();
            return true;
        }

        private async Task<bool> ChangeRoleStaff(string key, AdminUser user)
        {
            var oldUser = UserManager.FindById(key);
            var oldRoleId = oldUser.Roles.SingleOrDefault().RoleId;
            var oldRoleName = _dbasp.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
            var newRoleName = _dbasp.Roles.SingleOrDefault(r => r.Id == user.Acess.ToString()).Name;
            if (oldRoleName != newRoleName)
            {
                IdentityResult resultRemoveRole = await UserManager.RemoveFromRolesAsync(key, oldRoleName);
                if (resultRemoveRole.Succeeded)
                {
                    IdentityResult resultAddRole = await UserManager.AddToRoleAsync(key, newRoleName);
                    SaveChange(key);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// Dùng để bắt buộc đăng xuất khi thay đổi quyền hạn ngươi dùng!
        /// </summary>
        /// <param name="key"></param>
        private void SaveChange(string key)
        {
            var CurrentUser = _dbasp.Users.Where(x => x.Id == key).SingleOrDefault();
            if (User.Identity.Name == CurrentUser.Email)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }

        }

        /// <summary>
        /// Dùng để check tồn tại khi edit hoặc tạo mới account!
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsExsitAccount(AdminUser data)
        {
            bool fg = false;
            var Mess = "Tài khoản " + data.UserName + " đã tồn tại";
            var TempUserAccountAdmin = _dbasp.Users.Select(g => g).Where(x => x.Id == data.ID_AdminUser && x.Email == data.UserName).SingleOrDefault();
            if (TempUserAccountAdmin != null)
            {
                fg = true;
                Mess = "";
            }
            else
            {
                var CheckAccountNameExist = _dbasp.Users.Select(x => x).Where(x => x.Email == data.UserName).Count();
                if (CheckAccountNameExist == 0)
                {
                    fg = true;
                    Mess = "";
                }
            }
            return Json(new { Result = fg, Message = Mess }
            );

        }

        /// <summary>
        /// Dùng để tìm kiếm quyền !
        /// </summary>
        /// <returns></returns>
        public JsonResult RolesLookUp()
        {
            var temp = _dbasp.Roles.Select(x => x).ToArray();
            var t = from p in _dbasp.Roles
                    select new
                    {
                        IdRole = p.Id,
                        Name = p.Name
                    };

            return Json(new { data = t }, JsonRequestBehavior.AllowGet);
        }

        #region
        //Helper
        private async Task<bool> IsLoginAndAuthenAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var UserLogin = _dbasp.Users.Where(x => x.Email == User.Identity.Name).SingleOrDefault();
                if (UserLogin != null)
                {
                    if (await SignInManager.UserManager.IsInRoleAsync(UserLogin.Id, "Admin") || await SignInManager.UserManager.IsInRoleAsync(UserLogin.Id, "Manager"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion
    }
}