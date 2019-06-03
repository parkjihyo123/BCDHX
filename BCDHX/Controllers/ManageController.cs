using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BCDHX.Models;
using BCDHX.Moduns.Unity;
using BCDHX.Moduns.Models;
using System.Data.Entity;
using System.Net.Http;
using PayPal.Api;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Claims;
using BCDHX.Moduns.Mannger;
using Rest;
using RestSharp;

namespace BCDHX.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private WebDieuHienDB _dbBCDH;
        private SH256Code _sH256Code;
        public ManageController()
        {
            this._dbBCDH = new WebDieuHienDB();
            
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        private string UserId
        {

            get
            {
                var temp = (UserLoginTemp)Session["Authencation"];
                return temp.UserId;
            }

        }
        private string UserName
        {
            get
            {
                var temp = (UserLoginTemp)Session["Authencation"];
                return temp.Username;
            }
        }
        // Get: / Manage/Index (New)
        private Account OrginUserFormDatabase
        {
            get
            {
                return _dbBCDH.Accounts.SingleOrDefault(x => x.ID_Account == UserId);
            }
        }
        private UserViewModel GetUserInformation
        {
            get
            {
                UserViewModel tempUser = new UserViewModel
                {
                    Access = OrginUserFormDatabase.Access.Value,
                    Fullname = OrginUserFormDatabase.Fullname,
                    Address = OrginUserFormDatabase.Address,
                    ID_Account = OrginUserFormDatabase.ID_Account,
                    Img = OrginUserFormDatabase.Img,
                    Password = OrginUserFormDatabase.Password,
                    Username = OrginUserFormDatabase.Username,
                   Amount =  OrginUserFormDatabase.Amount.Value.ToString(),
                };
                return tempUser;
            }
        }
        public  ActionResult Index()
        {
            
                return View();
        }

        //
        public PartialViewResult GeneralInfromation()
        {
           
            return PartialView(GetUserInformation);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult UpdateGeneralProfile([Bind(Exclude = "ID_Account,Amount,Username")]Account account)
        {
            using (_dbBCDH)
            {
                var ChangeAccount = _dbBCDH.Accounts.AsNoTracking().SingleOrDefault(x => x.ID_Account == UserId);
                account.ID_Account = UserId;
                account.Username = UserName;
                account.Amount = ChangeAccount.Amount;
                account.Access = ChangeAccount.Access;
                account.Img = ChangeAccount.Img;
                _dbBCDH.Entry(account).State = System.Data.Entity.EntityState.Modified;
                var rs = _dbBCDH.SaveChanges();
                if (rs == 1)
                {
                    return Json(new
                    {
                        Status = 0,
                        Error = "Cập nhật thành công"
                    });
                }
                else if (rs == 0)
                {
                    return Json(new
                    {
                        Status = 1,
                        Error = "Cập nhật không thành công"
                    });
                }
            }
            return null;
        }

        public PartialViewResult GeneralAvatarUser()
        {          
            return PartialView(GetUserInformation);
        }
        
       public PartialViewResult GeneralSummaryUser()
        {
            return PartialView(GetUserInformation);
        }
        public PartialViewResult PayToAccountGeneral()
        {

            return PartialView();
        }
        
        [HttpPost]
        public ActionResult PaymentWithVTC(string data, string signature)
        {
           
            StreamReader readstream = new StreamReader(Response.OutputStream);
            string strRespone = readstream.ReadToEnd();
            readstream.Close();
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult PaymenProcess(FormCollection fr)
        {
            var money = Request.Form["moneyAccount"];
            //_sH256Code.GenerateSHA256AfterAll("");
            return new EmptyResult();
        }
        public ActionResult PaymentWithPayPal(string Cancel = null)
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            var money = Request.Form["moneyAccount"];
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Manage/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return View("SuccessView");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentWithPayPal(FormCollection fr)
        {
            var Amount = Request.Form["moneyAccount"];
            TempData["AmountTemp"] = Amount;
            return RedirectToAction("PaymentWithPayPal");
           // return Redirect("http://alpha1.vtcpay.vn/portalgateway/checkout.html" + "?amount=10000&currency=VND&receiver_account=0357758300&reference_number=5398&url_return=https://localhost:44343/Manage/PaymentWithVTC/"+ "&website_id=120273&signature=E8EA092AC96BC47F5E89EFD239560235A01EB53E9B29B041A43F67A9E0C2A34E");
           //IAuthContainerModel model = GetJWTContainerModel("Moshe Binieli", "mmoshikoo@gmail.com");
           //IAuthService authService = new JWTService(model.SecrectKey);
           ////string authToken = "<q4676q867vgb2zrr$0f534462675cf5050c045ea7c51cff03>";
           //string url = " https://sandbox-api.baokim.vn/payment/";
           //string query = string.Format("api/v4/bpm/list?jwt={0}", authService.GenerateToken(model));
           //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url+query);         
           //req.Method = "GET";
           //req.UseDefaultCredentials = true;
           //req.Accept = @"text/html, application/xhtml+xml, */*";s               
           //WebResponse response = req.GetResponse();
           //using (Stream dataStream = response.GetResponseStream())
           //{
           //    // Open the stream using a StreamReader for easy access.  
           //    StreamReader reader = new StreamReader(dataStream);
           //    // Read the content.  
           //    string responseFromServer = reader.ReadToEnd();
           //    // Display the content.  
           //    //Console.WriteLine(responseFromServer);
           //}
           //using (var stream = req.GetRequestStream())
           //    stream.Write(Encoding.ASCII, 0, data.Length);
           //StreamReader readstream = new StreamReader(req.GetResponse().GetResponseStream());
           //string strRespone = readstream.ReadToEnd();
           // readstream.Close();

            //if (strRespone.StartsWith("SUCCESS"))
            //{

            //}



        }

        #region Private Methods
        private static JWTContainerModel GetJWTContainerModel(string name, string email)
        {
            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email)
                }
            };
        }
        #endregion
        public ActionResult SuccessView()
        {
            return View();
        }
        public ActionResult FailureView()
        {
            return View();
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var amountTemp = TempData["AmountTemp"].ToString();
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Thanh Toán Nạp Tiền Vào Tài Khoản",
                currency = "USD",
                price = amountTemp,
                quantity = "1",
                
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = amountTemp,               
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToDecimal(amountTemp)).ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
                
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Thanh Toán BCDHShop",
                invoice_number = Convert.ToString((new Random()).Next(100000)),//Generate an Invoice No  
                amount = amount,
                item_list = itemList,
                
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
                

            };
            TempUserPaymentForAccount tempUser = new TempUserPaymentForAccount
            {
                Id = transactionList.Select(x => x.invoice_number).ToString(),
                NameAccount = UserName,
                PaymentDate = DateTime.Now,
                UserId = UserId,
                PaymentType = UserPaymentName.PAYTOACCOUNT.ToString(),

            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }
        //
        //
        // GET: /Manage/Index
        //public async Task<ActionResult> Index(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
        //        : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
        //        : "";

        //    var userId = User.Identity.GetUserId();
        //    var model = new IndexViewModel
        //    {
        //        HasPassword = HasPassword(),
        //        PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
        //        TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
        //        Logins = await UserManager.GetLoginsAsync(userId),
        //        BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
        //    };
        //    return View(model);
        //}

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}