using BCDHX.JWT;
using BCDHX.Models;
using BCDHX.Models.ModelObject;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using BCDHX.Unity.PaymentBK;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BCDHX.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CheckOutController : Controller
    {
        private const string CartSession = "CartSession";
        private readonly WebDieuHienDB _dbBCDH;
        private ApplicationUserManager _userManager;
        private PaymentLibrary _paymentLibrary;
        public CheckOutController()
        {
            _dbBCDH = new WebDieuHienDB();
            _paymentLibrary = new PaymentLibrary();
        }
        public CheckOutController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;

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
        // GET: CheckOut
        public ActionResult Index()
        {
            if (CanCheckOut())
            {
                TempData["TempUser"] = GetUserInformation();
                TempData["CartDetail"] = GetCarDetail();
                return View(GetListItems());
            }
            else
            {
                return RedirectToAction("Index", "Cart");
            }

        }
        [HttpPost]
        public async Task<JsonResult> ProcessCheckOut(InvoiceModel data)
        {
            if (data != null)
            {
                var userTemp = UserManager.Users.Where(x => x.Email == data.Email).SingleOrDefault();
                var InvoiceID = CreateInvoiceID();
                data.ID_Invoice = InvoiceID;
                data.ID_Account = userTemp.Id;
               
                    return await CallApiToBK(data);
                

            }
            else
            {
                return Json(new { Error = "Lỗi thanh toán!", Status = 2 });
            }
        }

        [HttpPost]
        public JsonResult PrcessCheckOutWithBalance(InvoiceModel data)
        {
            if (data != null)
            {
                var userTemp = UserManager.Users.Where(x => x.Email == data.Email).SingleOrDefault();
                var InvoiceID = CreateInvoiceID();
                data.ID_Invoice = InvoiceID;
                data.ID_Account = userTemp.Id;
                if (ProcessWithBalance(data))
                {
                    return Json(new { Error = GetUrlOrgin() + "/PaymentResult/ResultUseBalance/" + InvoiceID, Status = 1 });
                }
                return Json(new { Error = "Tài khoản không đủ số dư!", Status = 0 });

            }
            else
            {
                return Json(new { Error = "Lỗi thanh toán!", Status = 2 });
            }

        }


        #region Phần mở rộng checkout
        /// <summary>
        /// Check điều kiện để checkout , giỏ hàng không rỗng
        /// </summary>
        /// <returns></returns>
        private bool CanCheckOut()
        {
            List<Cart> tempCart = (List<Cart>)Session[CartSession] ?? new List<Cart>();

            return tempCart != null && tempCart.Count != 0 && tempCart.Select(x => x.ShippingLocationFee).ToList().Sum(x => x) > 0;
        }
        /// <summary>
        /// Lấy ảnh ra banner cho checkout page
        /// </summary>
        /// <returns></returns>
        public PartialViewResult PageBannerCheckOut()
        {
            Silder tempImagePageBanner = _dbBCDH.Silders.Where(x => x.Title == "PageBannerForCheckOut").SingleOrDefault() ?? new Silder();
            return PartialView(tempImagePageBanner);
        }
        //private List<Cart> GetListItems()
        //{
        //    List<Cart> tempCart = (List<Cart>)Session[CartSession];
        //    if (tempCart!=null)
        //    {
        //        return tempCart;
        //    }else
        //    {
        //        return new List<Cart>();
        //    }
        //}
        private CartDetail GetCarDetail()
        {
            var cartDetail = (CartDetail)Session["CartDetail"];
            if (cartDetail != null)
            {
                return cartDetail;
            }
            else
            {
                return new CartDetail();
            }
        }
        private UserViewModel GetUserInformation()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userTemp = UserManager.Users.Where(x => x.Email == User.Identity.Name).SingleOrDefault();
                return _dbBCDH.Accounts.Select(x => new UserViewModel { Address = x.Address, ID_Account = x.ID_Account, Amount = x.Amount.Value.ToString(), Fullname = x.Fullname, Username = x.Username }).Where(x => x.ID_Account == userTemp.Id).SingleOrDefault();
            }
            else
            {
                return new UserViewModel();
            }
        }
        private List<CartViewModel> GetListItems()
        {
            List<Cart> tempCart = (List<Cart>)Session[CartSession];
            List<CartViewModel> tempListCartViewModel = new List<CartViewModel>();
            if (tempCart != null && tempCart.Count > 0)
            {
                foreach (var item in tempCart)
                {

                    CartViewModel tempCartViewModel = new CartViewModel
                    {
                        ID_Product = item.Product.ID_Product,
                        Name_Product = item.Product.Name_Product,
                        Img = item.Product.Img,
                        Quantity = item.Quantity,
                        Ver = item.OptionVer ?? "Other",
                        _ShippingFee = item.ShippingLocationFee,
                    };

                    if (item.Product.Sale != 0)
                    {
                        tempCartViewModel.Price = (item.Quantity * item.Product.Sale).ToString();
                    }
                    else
                    {
                        tempCartViewModel.Price = (item.Quantity * item.Product.Price).ToString();
                    }
                    tempListCartViewModel.Add(tempCartViewModel);
                }
            }

            return tempListCartViewModel;
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetInvoieToEmail(string InvoiceID)
        {

            if (InvoiceID != null)
            {
                //var temp = _dbBCDH.InvoiceDetails.Select(x=>new InvoiceModelView {ID_Invoice = x.ID_Invoice,ID_InvoiceDetail=x.ID_InvoiceDetail,ID_Product=x.ID_Product,_Price=x.Price.Value,_Sale=x.Sale.Value,_ShippingFee=x.ShippingFee.Value,_StatusInvoice=x.StatusInvoice.Value,Quantity=x.Quantity.Value,ProcessOrder=x.ProcessOrder }).Where(x => x.ID_Invoice == InvoiceID).ToList();
                var temp = _dbBCDH.InvoiceDetails.Join(_dbBCDH.Products, invoiceDetail => invoiceDetail.ID_Product, product => product.ID_Product, (invoiceDetail, product) => new { InvoiceDetail = invoiceDetail, Product = product }).Select(x => new InvoiceModelView { ID_Invoice = x.InvoiceDetail.ID_Invoice, ID_InvoiceDetail = x.InvoiceDetail.ID_InvoiceDetail, ID_Product = x.InvoiceDetail.ID_Product, _Price = x.InvoiceDetail.Price.Value, _Sale = x.InvoiceDetail.Sale.Value, _ShippingFee = x.InvoiceDetail.ShippingFee.Value, _StatusInvoice = x.InvoiceDetail.StatusInvoice.Value, Quantity = x.InvoiceDetail.Quantity.Value, ProcessOrder = x.InvoiceDetail.ProcessOrder, NameProduct = x.Product.Name_Product }).Where(x => x.ID_Invoice == InvoiceID).ToList();
                if (temp.Count > 0)
                {
                    return Json(new { data = temp });
                }
                else
                {
                    return Json("Not Found InvoiceID");
                }
            }
            else
            {
                return Json("Not Found InvoiceID");
            }
        }
        public bool ProcessWithBalance(InvoiceModel invoice)
        {
            var dateCreate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            if (CheckBalaceForCheckOut())
            {
                var tempCodeSale = CreateCupon(invoice.ID_Account);
                List<InvoiceDetail> listInvoiceDetail = new List<InvoiceDetail>();
                if (tempCodeSale != null)
                {
                    foreach (var item in GetListItems())
                    {
                        InvoiceDetail invoiceDetail = new InvoiceDetail { ID_Invoice = invoice.ID_Invoice, ProcessOrder = invoice.ProcessOrder, ID_Product = item.ID_Product, Quantity = item.Quantity, Price = item._Price, ShippingFee = item._ShippingFee, StatusInvoice = true };
                        listInvoiceDetail.Add(invoiceDetail);
                        SendHtmlFormattedEmail("Hóa đơn thanh toán BCDH", CreateEmailBodyConfirmation(invoice.Fullname, tempCodeSale.PercentSale.ToString(), tempCodeSale.Code, tempCodeSale.EndDate.Value.ToString("MM/dd/yyyy HH:mm:ss"), invoice.ID_Invoice, GetUrlOrgin(), dateCreate, string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", GetCarDetail().TotalGrand), item.Price, item.Name_Product), invoice.Email);
                    }
                    UsedCode(invoice.ID_Account);
                }
                else
                {
                    foreach (var item in GetListItems())
                    {
                        InvoiceDetail invoiceDetail = new InvoiceDetail { ID_Invoice = invoice.ID_Invoice, ProcessOrder = invoice.ProcessOrder, ID_Product = item.ID_Product, Quantity = item.Quantity, Price = item._Price, ShippingFee = item._ShippingFee, StatusInvoice = true };
                        listInvoiceDetail.Add(invoiceDetail);
                        SendHtmlFormattedEmail("Hóa đơn thanh toán BCDH", CreateEmailBodySecond(invoice.Fullname, invoice.ID_Invoice, GetUrlOrgin(), dateCreate, string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", GetCarDetail().TotalGrand), item.Price, item.Name_Product), invoice.Email);
                    }
                    UsedCode(invoice.ID_Account);
                }
                CreateInvoice(invoice);
                CreatDetailInvoice(listInvoiceDetail);
                ChangeAmountAccount();
                RemoveCartAndSession();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckOutOnline(InvoiceModel invoice)
        {
            var dateCreate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            if (CheckBalaceForCheckOut())
            {
                var tempCodeSale = CreateCupon(invoice.ID_Account);
                List<InvoiceDetail> listInvoiceDetail = new List<InvoiceDetail>();
                if (tempCodeSale != null)
                {
                    foreach (var item in GetListItems())
                    {
                        InvoiceDetail invoiceDetail = new InvoiceDetail { ID_Invoice = invoice.ID_Invoice, ProcessOrder = invoice.ProcessOrder, ID_Product = item.ID_Product, Quantity = item.Quantity, Price = item._Price, ShippingFee = item._ShippingFee, StatusInvoice = true };
                        listInvoiceDetail.Add(invoiceDetail);
                        SendHtmlFormattedEmail("Hóa đơn thanh toán BCDH", CreateEmailBodyConfirmation(invoice.Fullname, tempCodeSale.PercentSale.ToString(), tempCodeSale.Code, tempCodeSale.EndDate.Value.ToString("MM/dd/yyyy HH:mm:ss"), invoice.ID_Invoice, GetUrlOrgin(), dateCreate, string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", GetCarDetail().TotalGrand), item.Price, item.Name_Product), invoice.Email);
                    }
                    UsedCode(invoice.ID_Account);
                }
                else
                {
                    foreach (var item in GetListItems())
                    {
                        InvoiceDetail invoiceDetail = new InvoiceDetail { ID_Invoice = invoice.ID_Invoice, ProcessOrder = invoice.ProcessOrder, ID_Product = item.ID_Product, Quantity = item.Quantity, Price = item._Price, ShippingFee = item._ShippingFee, StatusInvoice = true };
                        listInvoiceDetail.Add(invoiceDetail);
                        SendHtmlFormattedEmail("Hóa đơn thanh toán BCDH", CreateEmailBodySecond(invoice.Fullname, invoice.ID_Invoice, GetUrlOrgin(), dateCreate, string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", GetCarDetail().TotalGrand), item.Price, item.Name_Product), invoice.Email);
                    }
                    UsedCode(invoice.ID_Account);
                }
                CreateInvoice(invoice);
                CreatDetailInvoice(listInvoiceDetail);

                RemoveCartAndSession();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<JsonResult> CallApiToBK(InvoiceModel invoicemodel)
        {
            var cartDetail = GetCarDetail();
            var baseUrl = ConfigurationManager.AppSettings["BKUrl"];
            PaymentWithBKOnline paymentBK = new PaymentWithBKOnline {mrc_order_id= invoicemodel.ID_Invoice, total_amount= 100000,description="Thanh Toán Hóa đơn số"+ invoicemodel.ID_Invoice, url_success=GetUrlOrgin()+"/PaymentResult"};
            _paymentLibrary.AddRequestData("mrc_order_id", invoicemodel.ID_Invoice);
            _paymentLibrary.AddRequestData("total_amount", "100000");
            _paymentLibrary.AddRequestData("description", "Thanh toan don hang so:"+ invoicemodel.ID_Invoice);
            _paymentLibrary.AddRequestData("jwt", new JWTService().GenerateToken(paymentBK));
            _paymentLibrary.AddRequestData("url_success", GetUrlOrgin()+"/PaymentResult/ShowTest1");
            _paymentLibrary.AddRequestData("webhooks", GetUrlOrgin() + "/PaymentResult/WebHookResult");
            
            string paymentUrl = _paymentLibrary.CreateRequestUrl(baseUrl + "api/v4/order/send");
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(paymentUrl);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync(paymentUrl, null);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var table = JsonConvert.DeserializeObject<PaymentResult>(data);
                    return table.code == 0 ? Json(new { Error = table.data.redirect_url, Status = 1 }): Json(new { Error = table.data.redirect_url, Status = 1 });
                  
                }
                else
                {
                    return Json(new { Error = "Xảy ra lỗi trong quá trình Thanh Toán", Status = 0 });
                }

            }
        }
    
        private CuponCode CreateCupon(string userid)
        {
            var tempUser = _dbBCDH.Invoices.Where(x=>x.ID_Account==userid&&x.Payment_Methods!="PayForAccount").FirstOrDefault();
            if (tempUser!=null)
            {
                return null;
            }else
            {
                var dateCreate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                var code = new RandomCode().RandomNumber(8);
                CuponCode tempCode = new CuponCode { Code = code, ContentCode = "KMFristTimeBuy", CreateDate = Convert.ToDateTime(dateCreate), NumberUse = 1, PercentSale = 10, ValueSale = 0, EndDate = Convert.ToDateTime(dateCreate).AddDays(7) };
                _dbBCDH.Entry(tempCode).State = System.Data.Entity.EntityState.Added;
                _dbBCDH.SaveChanges();
                return tempCode;
            }
               
        }
        public void UsedCode(string iduser)
        {
          
            if (Session["Code"] != null)
            {
                var useCode = Session["Code"].ToString();
                var codeInvoice = _dbBCDH.CuponCodes.AsNoTracking().Where(x => x.Code == useCode).SingleOrDefault();
                CuponCode temp = codeInvoice;
                temp.NumberUse = temp.NumberUse - 1;
                var dateCreate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                CuponCodeHistory tempcode = new CuponCodeHistory { CuponCodeUsed = useCode, ID_User = iduser, DateUsed = Convert.ToDateTime(dateCreate) };
                _dbBCDH.Entry(tempcode).State = System.Data.Entity.EntityState.Added;
                _dbBCDH.Entry(temp).State = System.Data.Entity.EntityState.Modified;
                _dbBCDH.SaveChanges();
            }
          
            
        }
        public string GetUrlOrgin()
        {
            var request = HttpContext.Request;
            var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
            return address;
        }

        private void ChangeAmountAccount()
        {
            var userTemp = GetUserInformation();
            var cartDetail = GetCarDetail();
            Account mg = new Account { Address = userTemp.Address, ID_Account = userTemp.ID_Account, Amount = userTemp._amount - cartDetail.TotalGrand, Fullname = userTemp.Fullname, Username = userTemp.Username };
            _dbBCDH.Entry(mg).State = System.Data.Entity.EntityState.Modified;
            _dbBCDH.SaveChanges();
        }

        private string CreateInvoiceID()
        {
            return new RandomCode().RandomCodeGenral(20, false);
        }

        public bool CheckBalaceForCheckOut()
        {
            var tempCartDetail = GetCarDetail();
            if (tempCartDetail != null)
            {
                var userTemp = GetUserInformation();
                if (userTemp._amount >= tempCartDetail.TotalGrand)
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
        public JsonResult CheckBalance()
        {
            var tempCartDetail = GetCarDetail();
            var userTemp = GetUserInformation();
            if (CheckBalaceForCheckOut())
            {

                string leftBalance = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", userTemp._amount - tempCartDetail.TotalGrand);
                return Json(new { Error = leftBalance, Status = 1 });
            }
            else
            {
                string leftBalance = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", userTemp._amount);
                return Json(new { Error = leftBalance, Status = 2 });
            }

        }
        public void RemoveCartAndSession()
        {
            Session.Remove("CartSession");
            Session.Remove("CartDetail");
            Session.Remove("Code");
        }
        private void CreateInvoice(InvoiceModel invoice)
        {
            var dateCreate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            Invoice temp = new Invoice { ID_Account = invoice.ID_Account, ID_Invoice = invoice.ID_Invoice, Email = invoice.Email, Phone = invoice.Phone, MaVanDon = null, Status_Order = true, Payment_Methods = invoice._Payment_Methods.ToString(), ProcessOrder = 1, Purchase_Date = Convert.ToDateTime(dateCreate), Shipping_Address = invoice.Shipping_Address, Note = invoice.Note, Orgin = invoice.Orginal };
            // var result = 0;
            _dbBCDH.Entry(temp).State = System.Data.Entity.EntityState.Added;
            _dbBCDH.SaveChanges();
            // return result > 0;
        }
        private void CreatDetailInvoice(List<InvoiceDetail> listinvoice)
        {
            // var result = 0;
         
            foreach (var item in listinvoice)
            {
                _dbBCDH.Entry(item).State = System.Data.Entity.EntityState.Added;
            }
            _dbBCDH.SaveChanges();
            //return result > 0;
        }
      
        public string CreateEmailBodyConfirmation(string fullname, string valueSale,string code, string expiration_date, string invoiceID, string homepage, string dateBuy, string total, string price, string nameProduct)
        {
            string body = string.Empty;
            
            var tempCartDetail = GetListItems();
            //using streamreader for reading my htmltemplate   
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/Email/InvoiceEmail/InvcoiveEmail.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{fullname}", fullname); //replacing the required things  
            body = body.Replace("{valueSale}", valueSale);
            body = body.Replace("{expiration_date}", expiration_date);
            body = body.Replace("{invoiceID}", invoiceID);
            body = body.Replace("{homepage}", homepage);
            body = body.Replace("{dateBuy}", dateBuy);
            body = body.Replace("{total}", total);
            body = body.Replace("{description}", nameProduct);
            body = body.Replace("{amount}", price);
            body = body.Replace("{SaleCode}", code);
            return body;
        }
        public string CreateEmailBodySecond(string fullname, string invoiceID, string homepage, string dateBuy, string total, string price, string nameProduct)
        {
            string body = string.Empty;

            var tempCartDetail = GetListItems();
            //using streamreader for reading my htmltemplate   
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/Email/InvoiceEmail/InvoiceAfterFristSaleCode.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{fullname}", fullname); //replacing the required things  
            body = body.Replace("{invoiceID}", invoiceID);
            body = body.Replace("{homepage}", homepage);
            body = body.Replace("{dateBuy}", dateBuy);
            body = body.Replace("{total}", total);
            body = body.Replace("{description}", nameProduct);
            body = body.Replace("{amount}", price);
            return body;
        }
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

        #endregion
    }
}