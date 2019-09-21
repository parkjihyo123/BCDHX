using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BCDHX.Models;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
namespace BCDHX.Controllers
{

    public class AccountController : Controller, IEmail
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _db;
        private ApplicationDbContext _dbasp;
        private GetInformationUserUsingOSAndBrowser _getInforUser;

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string CallStatusExtend)
        {
            if (IsAuthenticated(AuthenticationManager) && User.IsInRole("Customer"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (CallStatusExtend != null)
                {
                    ViewBag.Error = "Tài đã khoản đã tồn tại, nếu quên mật khẩu xin hãy bấm vào quên mật khẩu để tìm lại.Cần hỗ trợ hãy liên hệ ngay LiveChat!";
                }
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        /// [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true           
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            var tempuser = SignInManager.UserManager.Find(model.Email, model.Password);
            if (tempuser != null)
            {
                if (tempuser.EmailConfirmed != true)
                {
                    result = SignInStatus.RequiresVerification;
                }
            }

            switch (result)
            {
                case SignInStatus.Success:
                    var CustomerRoles = UserManager.GetRoles(tempuser.Id).Select(x => x).Where(x => x.StartsWith("Customer")).Count();
                    if (CustomerRoles > 0)
                    {
                        var GetIdUser = _db.Accounts.SingleOrDefault(x => x.Username == model.Email);
                        TempData["IDUserForUploadImage"] = GetIdUser.ID_Account;
                        return Json(new
                        {
                            Status = 0,
                            Error = "Done",
                            ReturnUrl = model.ReturnLink
                        });
                    }
                    else
                    {
                        ForceLogOff();
                        return Json(new
                        {
                            Status = 4,
                            Error = "Bạn không có quyền truy cập",

                        });
                    }
                case SignInStatus.LockedOut:
                    ForceLogOff();
                    return Json(new
                    {
                        Status = 1,
                        Error = "Tài khoản bị khóa"
                    });
                case SignInStatus.RequiresVerification:
                    ForceLogOff();
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
        public void ForceLogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (IsAuthenticated(AuthenticationManager))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }

        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> Register(Account model)
        {
            var user = new ApplicationUser { UserName = model.Username, Email = model.Username };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Errors.Count() != 0)
            {
                foreach (var item in result.Errors)
                {
                    return Json(item.ToString());
                }
            }
            else
            {
                if (result.Succeeded)
                {
                    ApplicationUser currentUser = _dbasp.Users.FirstOrDefault(x => x.Email.Equals(model.Username));
                    await UserManager.AddToRoleAsync(currentUser.Id, "Customer");
                    AddAccountToDB(model.Fullname, model.Username, model.Address, user.Id, model.Password);
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    SendHtmlFormattedEmail("Xác thực tài khoản", CreateEmailBodyConfirmation(model.Username, model.Password, DateTime.Now.ToString("dd-MM-yyyy"), callbackUrl), model.Username);
                    return Json("1");
                }
            }
            return null;
        }

        /// <summary>
        /// use for create body email for confirmation
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="title"></param>
        /// <param name="datecreate"></param>
        /// <param name="url_action"></param>
        /// <returns></returns>
        public string CreateEmailBodyConfirmation(string userName, string passWord, string datecreate, string url_action)
        {
            string body = string.Empty;
            //using streamreader for reading my htmltemplate   
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/Email/NotificationEmail/NotificationEmail.html")))

            {

                body = reader.ReadToEnd();

            }
            body = body.Replace("{username}", userName); //replacing the required things  
            body = body.Replace("{password}", passWord);
            body = body.Replace("{createdate}", datecreate);
            body = body.Replace("{action_url}", url_action);
            return body;
        }

        public string CreateEmailBodyForgetPassword(string userName, string url_action, string browserName, string browserVersion, string ip, string osName)
        {
            string body = string.Empty;

            //using streamreader for reading my htmltemplate   
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/Email/NotificationEmail/RestEmail.html")))

            {

                body = reader.ReadToEnd();

            }
            body = body.Replace("{name}", userName); //replacing the required things  
            body = body.Replace("{browserName}", browserName);
            body = body.Replace("{browserVersion}", browserVersion);
            body = body.Replace("{osName}", osName);
            body = body.Replace("{action_url}", url_action);
            body = body.Replace("{ip}", ip);
            return body;
        }

        /// <summary>
        /// send email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="sendto"></param>
        public void SendHtmlFormattedEmail(string subject, string body, string sendto)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())

                {

                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);

                    mailMessage.Subject = subject;

                    mailMessage.Body = body;

                    mailMessage.IsBodyHtml = true;

                    mailMessage.To.Add(new MailAddress(sendto));

                    SmtpClient smtp = new SmtpClient();

                    smtp.Host = ConfigurationManager.AppSettings["Host"];

                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();

                    NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"]; //reading from web.config  

                    NetworkCred.Password = ConfigurationManager.AppSettings["Password"]; //reading from web.config  

                    smtp.UseDefaultCredentials = true;
                    //smtp.EnableSsl = true;
                    smtp.Credentials = NetworkCred;

                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]); //reading from web.config  

                    smtp.Send(mailMessage);

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Add to database userinformation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// 
        public void AddAccountToDB(string fullname, string username, string address, string idAccount, string password)
        {
            var TempUserId = TempData["TempIdForUser"];
            var TempUserLinkImage = "";

            if (TempData["TempImageFileName"] != null)
            {
                TempUserLinkImage = TempData["TempImageFileName"].ToString();
            }
            else
            {
                TempUserLinkImage = "account-image-placeholder.jpg";
            }
            var userToDB = new Account { Fullname = fullname, Access = 1, Address = address, ID_Account = idAccount, Username = username, Amount = 0, Password = password, Img = TempUserLinkImage };
            _db.Entry(userToDB).State = System.Data.Entity.EntityState.Added;
            _db.SaveChanges();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "NotificationSystem");
            }

            else
            {
                var temp = SignInManager.UserManager.FindById(userId);
                if (temp == null)
                {
                    return RedirectToAction("Index", "NotificationSystem");
                }
                else
                {
                    var tempResult = await UserManager.ConfirmEmailAsync(userId, code);
                    if (tempResult.Succeeded)
                    {
                        var result = await UserManager.ConfirmEmailAsync(userId, code);

                        return View(temp);
                    }
                    else
                    {
                        return RedirectToAction("Index", "NotificationSystem");
                    }
                }


            }


        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ReSendConfirmEmail(Account model)
        {
            //ApplicationUser currentUser = _dbasp.Users.FirstOrDefault(x => x.Email.Equals(model.Username));
            Account currentUser = _db.Accounts.SingleOrDefault(x => x.Username.Equals(model.Username));
            if (currentUser != null && SignInManager.UserManager.FindById(currentUser.ID_Account) != null)
            {
                var tempUserAfterSign = SignInManager.UserManager.FindById(currentUser.ID_Account);
                if (!tempUserAfterSign.EmailConfirmed)
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(tempUserAfterSign.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = tempUserAfterSign.Id, code = code }, protocol: Request.Url.Scheme);
                    SendHtmlFormattedEmail("Xác thực tài khoản", CreateEmailBodyConfirmation(model.Username, model.Password, DateTime.Now.ToString("dd-MM-yyyy"), callbackUrl), model.Username);
                    return Json(new
                    {
                        Status = 0,
                        Error = "Xin mời kiểm tra lại hòm thư của email " + model.Username + "để kích hoạt tài khoản.Nếu có bất cứ gì thắc mắc xin nhắn tin cho livechat của chúng tôi để được hỗ trợ sớm nhất!"

                    });
                }
                else
                {
                    return Json(new
                    {
                        Status = 2,
                        Error = "Tài khoản quý khách đã kích hoạt!"

                    });
                }
            }
            else
            {
                return Json(new
                {
                    Status = 1,
                    Error = "Tài khoản không tồn tại"

                });
            }
        }
        //
        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            _getInforUser = new GetInformationUserUsingOSAndBrowser(Request);
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Json(new
                {
                    Status = 1,
                    Error = "Kiểm tra lại tài khoản email !"
                });
            }
            else
            {
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                TempData["RestTokenCode"] = code;
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                SendHtmlFormattedEmail("Xác thực tài khoản", CreateEmailBodyForgetPassword(user.UserName, callbackUrl, _getInforUser.browserName, _getInforUser.browserVersion, _getInforUser.ipAddress, _getInforUser.osName), user.UserName);
                return Json(new
                {
                    Status = 0,
                    Error = "Xin mời kiểm tra lại hòm thư của email " + user.Email + " để xác nhận rest mật khẩu.Nếu có bất cứ gì thắc mắc xin nhắn tin cho livechat của chúng tôi để được hỗ trợ sớm nhất!"
                });
            }
        }



        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            var tempCode = TempData["RestTokenCode"];
            if (code == null || tempCode == null || userId == null)
            {
                return RedirectToAction("Index", "NotificationSystem");
            }
            else
            {
                var temUser = UserManager.FindById(userId);
                if (code.Equals(tempCode.ToString()) && temUser != null)
                {

                    ResetPasswordViewModel actemp = new ResetPasswordViewModel
                    {
                        Email = temUser.Email,
                        Code = code
                    };
                    return View(actemp);
                }
                else
                {
                    return RedirectToAction("Index", "NotificationSystem");
                }
            }
            //   UserManager.UserTokenProvider.ValidateAsync(code);
            //return code == null ? View("Error") : View();

        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account", new { statuscode = "Faile" });
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                //each process hava a randomcode , just use onetime.
                var tempCodeStatus = new RandomCode().RandomCodeGenral(28, true);
                TempData["TempCodeStatus"] = tempCodeStatus;
                return RedirectToAction("ResetPasswordConfirmation", "Account", new { statuscode = tempCodeStatus, id = user.Id });
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation(string statuscode, string id)
        {
            var tempCodeStatus = TempData["TempCodeStatus"];
            if (statuscode == null || tempCodeStatus == null)
            {
                return RedirectToAction("Index", "NotificationSystem");
            }
            else
            {
                var tempUser = UserManager.FindById(id);
                if (statuscode.Equals("Faile") || tempUser == null)
                {
                    return RedirectToAction("Index", "NotificationSystem");
                }
                else
                {
                    return View(tempUser);
                }
            }

        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }
        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }
        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    var CheckUserExsit = UserManager.FindByEmail(loginInfo.Email);
                    if (CheckUserExsit == null)
                    {
                        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
                    }
                    else
                    {
                        return RedirectToAction("Login", new { CallStatusExtend = "ExsitAccount" });
                    }
            }
        }
        //


        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model)
        {

            if (User.Identity.IsAuthenticated)
            {
                return Json(new
                {
                    Status = "1",
                    Error = "IsAuthenticated",

                });
            }
            // Get the information about the user from the external login provider
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Json(new
                {
                    Status = "2",
                    Error = "Login Error"
                });
            }

            var user = new ApplicationUser { UserName = model.Username, Email = model.Username };
            var CheckUserExsit = UserManager.FindByEmail(model.Username);
            if (CheckUserExsit == null)
            {
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: true);
                        AddAccountToDB(model.Fullname, model.Username, model.Address, user.Id, user.PasswordHash);
                        await UserManager.AddToRoleAsync(user.Id, "Customer");
                        return Json(new
                        {
                            Status = "0",
                            Error = "Bạn đã đăng kí thành công!",

                        });
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Status = "3",
                    Error = "Tài khoản đã tồn tại , bạn hãy kiểm tra lại, nếu quên mật khẩu hãy bấm vào quên mật khẩu để rest mật khẩu"
                });
            }


            return Json("");
        }

        //
        // POST: /Account/LogOff

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                    DefaultAuthenticationTypes.ExternalCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        

        #region Helpers
        //GetUserLoginTemp
        public UserLoginTemp GetUserLoginTemp(string userName, string userId, string Address)
        {
            UserLoginTemp t = new UserLoginTemp
            {
                UserId = userId,
                Address = Address,
                Username = userName
            };
            return t;
        }
        //Filter when user had already Authenticated access Login page and Regsiter page
        private bool IsAuthenticated(IAuthenticationManager aut)
        {
            return aut.User.Identity.IsAuthenticated;
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
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

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}