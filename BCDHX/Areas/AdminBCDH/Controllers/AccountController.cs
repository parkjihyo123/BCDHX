using BCDHX.Models;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BCDHX.Areas.AdminBCDH.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;

        public AccountController()
        {
            _db = new WebDieuHienDB();
            _dbasp = new ApplicationDbContext();

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> Login(LoginViewModelAdmin account)
        {

            var result = await SignInManager.PasswordSignInAsync(account.Email, account.Password, account.RememberMe, shouldLockout: true);

            switch (result)
            {
                case SignInStatus.Success:
                    var temp = await SignInManager.UserManager.FindByEmailAsync(account.Email);
                    var CheckUserCondition = UserManager.GetRoles(temp.Id).Select(x => x);
                    if (CheckUserCondition.Where(x => !x.StartsWith("Customer")).Count() > 0)
                    {

                        return Json(new
                        {
                            Status = 0,
                            Error = "Done",
                            ReturnUrl = account.ReturnLink
                        });
                    }
                    else
                    {
                        SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        return Json(new
                        {
                            Status = 4,
                            Error = "Không có quyền đăng nhập vào đây!",

                        });
                    }

                case SignInStatus.LockedOut:
                    return Json(new
                    {
                        Status = 1,
                        Error = "Tài khoản bị khóa"
                    });
                case SignInStatus.RequiresVerification:
                    return Json(new
                    {
                        Status = 2,
                        Error = "NeedVerification"
                    });
                case SignInStatus.Failure:
                default:

                    return Json(new
                    {
                        Status = 3,
                        Error = "Sai tên đang nhập hoặc mật khẩu"
                    });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        //GetUserLoginTemp
        public UserLoginTempAdmin GetUserLoginTempAdmin(string userName, string userId, string Role)
        {
            UserLoginTempAdmin t = new UserLoginTempAdmin
            {
                UserId = userId,
                Role = Role,
                Username = userName
            };
            return t;
        }
        //Filter when user had already Authenticated access Login page and Regsiter page
        private bool IsAuthenticated(IAuthenticationManager aut)
        {
            return aut.User.Identity.IsAuthenticated;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }



        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectCustom(string returnUrl)
        {
            return Redirect(returnUrl);
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                    DefaultAuthenticationTypes.ExternalCookie);
            return RedirectToAction("Login", "Account");
        }
        #endregion
    }
}