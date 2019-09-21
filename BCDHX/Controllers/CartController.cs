using BCDHX.Models;
using BCDHX.Moduns.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BCDHX.Controllers
{
    [AllowAnonymous]
    public class CartController : Controller
    {
        private const string CartSession = "CartSession";
        private readonly WebDieuHienDB _dbBCDH;
        private ApplicationUserManager _userManager;
        public CartController()
        {
            _dbBCDH = new WebDieuHienDB();
        }
        public CartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddItemToCart(string productID, int quantity, string option)
        {
            var productTemp = _dbBCDH.Products.Join(_dbBCDH.ImageForProducts, product => product.ID_Product, image => image.ID_Product, (productN, ImageN) => new { Product = productN, ImageForProduct = ImageN }).Where(x => x.Product.ID_Product == productID).SingleOrDefault();
            if (productTemp == null)
            {
                return Json(new { Error = "Lỗi xảy ra!", Status = 0 });
            }
            else
            {
                var cartTemp = Session[CartSession];
                if (cartTemp != null)
                {
                    var list = (List<Cart>)cartTemp;
                    if (list.Exists(x => x.Product.ID_Product == productID))
                    {
                        foreach (var item in list)
                        {
                            if (item.Product.ID_Product == productID)
                            {
                                item.Quantity += quantity;
                            }
                        }
                    }
                    else
                    {
                        var item = new Cart();
                        item.Product = new Models.ModelObject.ProductModelCart { ID_Category = productTemp.Product.ID_Category, ID_Product = productTemp.Product.ID_Product, Name_Product = productTemp.Product.Name_Product, Img = productTemp.ImageForProduct.IMG1, Price = productTemp.Product.Price.Value, Quantity = productTemp.Product.Quantity.Value, Sale = productTemp.Product.Sale.Value };
                        item.Quantity = quantity;
                        item.OptionVer = option;
                        list.Add(item);
                    }
                    Session[CartSession] = list;
                }
                else
                {
                    var item = new Cart();
                    item.Product = new Models.ModelObject.ProductModelCart { ID_Category = productTemp.Product.ID_Category, ID_Product = productTemp.Product.ID_Product, Name_Product = productTemp.Product.Name_Product, Img = productTemp.ImageForProduct.IMG1, Price = productTemp.Product.Price.Value, Quantity = productTemp.Product.Quantity.Value, Sale = productTemp.Product.Sale.Value };
                    item.Quantity = quantity;
                    item.OptionVer = option;
                    var list = new List<Cart>();
                    list.Add(item);
                    Session[CartSession] = list;
                }


                return Json(new { Error = "Thêm vào giỏ hàng thành công!", Status = 1 });
            }
        }
        [HttpPost]
        public JsonResult UserUseCupon(string code)
        {
            List<Cart> tempCart = (List<Cart>)Session[CartSession];
            ArrayList temp = new ArrayList();

            var dateTime = DateTime.Now.Date;
            if (CheckLoginUser())
            {
                if (tempCart != null && tempCart.Count != 0)
                {
                    var tempCodeCupon = _dbBCDH.CuponCodes.Where(x => x.Code == code).SingleOrDefault();

                    if (tempCodeCupon != null)
                    {
                        if (DateTime.Compare(dateTime, tempCodeCupon.EndDate.Value) <= 0 && tempCodeCupon.NumberUse > 0)
                        {
                            var cartDetail = (CartDetail)Session["CartDetail"];
                            var percentSale = tempCodeCupon.PercentSale.Value;
                            var SaleValue = tempCodeCupon.ValueSale.Value;
                            var userTemp = UserManager.FindByName(User.Identity.Name);
                            if (Session["Code"] != null)
                            {
                                if (Session["Code"].ToString().Equals(code))
                                {
                                    
                                    return Json(new { Error = "Cupon đã được sử dụng!", Status = 5 });
                                }
                                else
                                {
                                    Session["Code"] = code;                    
                                    if (CheckUsedCode(code, userTemp.Id))
                                    {
                                        cartDetail = ModelForUponCode();
                                        if (percentSale != 0)
                                        {
                                            cartDetail.TotalGrand = cartDetail.TotalGrand - ((cartDetail.TotalGrand * percentSale) / 100);
                                        }
                                        if (SaleValue != 0)
                                        {
                                            cartDetail.TotalGrand = cartDetail.TotalGrand - SaleValue;
                                        }
                                        cartDetail.UseCupon = true;
                                        Session["CartDetail"] = cartDetail;
                                        return Json(new { Error = "Sử dụng Cupon thành công!", Status = 1 });
                                    }
                                    else
                                    {
                                        Session.Remove("Code");
                                        return Json(new { Error = "Cupon đã được sử dụng!", Status = 5 });
                                    }
                                }
                            }
                            else
                            {
                                Session["Code"] = code;
                                if (CheckUsedCode(code, userTemp.Id))
                                {
                                    if (percentSale != 0)
                                    {
                                        cartDetail.TotalGrand = cartDetail.TotalGrand - ((cartDetail.TotalGrand * percentSale) / 100);
                                    }
                                    if (SaleValue != 0)
                                    {
                                        cartDetail.TotalGrand = cartDetail.TotalGrand - SaleValue;
                                    }
                                    cartDetail.UseCupon = true;
                                    return Json(new { Error = "Sử dụng Cupon thành công!", Status = 1 });
                                }
                                else
                                {
                                    Session.Remove("Code");
                                    return Json(new { Error = "Cupon đã được sử dụng!", Status = 5 });
                                }
                                
                            }
                        }
                        else
                        {
                            return Json(new { Error = "Mã Cupon đã quá hạn hoặc hết lượt sử dụng", Status = 6 });
                        }

                    }
                    else
                    {
                        return Json(new { Error = "Mã Cupon không đúng!", Status = 4 });
                    }
                }
                else
                {
                    RemoveSessionCart();
                    return Json(new { Error = "Bạn chưa chọn sản phẩm nào!", Status = 3 });
                }

            }
            else
            {
                return Json(new { Status = 2 });
            }



        }

        [HttpPost]
        public JsonResult UpdateItemFromCart(string Items)
        {
            var cartUpdateViewtemp = new JavaScriptSerializer().Deserialize<List<UpdateCartModelView>>(Items);
            List<Cart> tempCart = (List<Cart>)Session[CartSession];
            foreach (var item in cartUpdateViewtemp)
            {
                foreach (var itemCartSession in tempCart)
                {
                    if (item.ID == itemCartSession.Product.ID_Product)
                    {
                        itemCartSession.Quantity = item.QuantityToCT;
                    }
                }

            }
            RemoveSessionCart();
            return Json(new { Error = "Cập nhật thành công", Status = 1 });
        }
        [HttpPost]
        public JsonResult ShippingUpdateLoction(string location)
        {
            List<Cart> tempCart = (List<Cart>)Session[CartSession];
            if (tempCart != null && tempCart.Count > 0)
            {
                foreach (var item in tempCart)
                {

                    item.ShippingLocationFee = Convert.ToDecimal(location);
                }

                return Json(new { Status = 1 });
            }
            else
            {
                return Json(new { Error = "Bạn phải chọn sản phẩm trước!", Status = 0 });
            }

        }
        [HttpPost]
        public JsonResult RemoveItemFromCart(string productId)
        {
            List<Cart> tempCart = (List<Cart>)Session[CartSession];
            foreach (var item in tempCart)
            {
                if (item.Product.ID_Product == productId)
                {
                    tempCart.Remove(item);
                    return Json(new { Error = "Xóa thành công!", Status = 1 });
                }

            }
            RemoveSessionCart();
            return Json(new { });
        }
        

        public bool CheckUsedCode(string IDcode, string idUser)
        {
            var tempHistroyCode = _dbBCDH.CuponCodeHistories.Where(x => x.CuponCodeUsed == IDcode && x.ID_User == idUser).SingleOrDefault();
            if (tempHistroyCode == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<CartViewModel> ModelFroShowItemCart()
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
            else
            {
                RemoveSessionCart();
            }
            return tempListCartViewModel;
        }
        [HttpGet]
        public async Task<JsonResult> ShowItemOnCart()
        {

            List<CartViewModel> tempCartViewModel = ModelFroShowItemCart();
            var wishListTemp = 0;
            if (tempCartViewModel.Count > 0)
            {

                var _shipping = new Decimal(0);

                foreach (var item in tempCartViewModel)
                {
                    _shipping = item._ShippingFee;
                }
                var Totalprice = string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", tempCartViewModel.Sum(x => x._Price));
                var ShippingFee = string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", _shipping);
                var TotalGrand = string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", tempCartViewModel.Sum(x => x._Price) + _shipping);
                var NumberItems = tempCartViewModel.Count();
                if (Session["CartDetail"] == null)
                {
                    CartDetail cartDetail = new CartDetail { ShippingLocationFee = _shipping, TotalGrand = tempCartViewModel.Sum(x => x._Price), UseCupon = false };
                    Session["CartDetail"] = cartDetail;
                }
                else
                {
                    var cartDetail = (CartDetail)Session["CartDetail"];
                    cartDetail.ShippingLocationFee = _shipping;
                    if (cartDetail.UseCupon)
                    {
                        TotalGrand = string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:C}", cartDetail.TotalGrand);
                    }
                    else
                    {
                        cartDetail.ShippingLocationFee = _shipping;
                        cartDetail.TotalGrand = tempCartViewModel.Sum(x => x._Price) + _shipping;
                    }

                }

                if (CheckLoginUser())
                {
                    var userTemp = await UserManager.FindByEmailAsync(User.Identity.Name);
                    wishListTemp = _dbBCDH.WishLists.Select(x => x).Where(x => x.ID_Account == userTemp.Id).ToList().Count();
                }
                return Json(new { data = tempCartViewModel, TotalPrice = Totalprice, TotalItems = NumberItems, WishListCount = wishListTemp, ShippingFeeLast = ShippingFee, TotalPriceGrand = TotalGrand }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (CheckLoginUser())
                {
                    var userTemp = await UserManager.FindByEmailAsync(User.Identity.Name);
                    wishListTemp = _dbBCDH.WishLists.Select(x => x).Where(x => x.ID_Account == userTemp.Id).ToList().Count();
                }
                return Json(new { data = "Empty", WishListCount = wishListTemp }, JsonRequestBehavior.AllowGet);
            }


        }
        #region Phần mở rộng cho cart
        private CartDetail ModelForUponCode()
        {
            List<CartViewModel> tempCartViewModel = ModelFroShowItemCart();
            var _shipping = new Decimal(0);

            foreach (var item in tempCartViewModel)
            {
                _shipping = item._ShippingFee;
            }
            return new CartDetail { ShippingLocationFee = _shipping, TotalGrand = tempCartViewModel.Sum(x => x._Price), UseCupon = false };
        }
        private void RemoveSessionCart()
        {
            Session.Remove("Code");
            Session.Remove("CartDetail");
        }
        public bool CheckLoginUser()
        {
            return User.Identity.IsAuthenticated;
        }
        public PartialViewResult CartPartial()
        {
            return PartialView();
        }
        public PartialViewResult PageBannerCart()
        {
            Silder tempImagePageBanner = _dbBCDH.Silders.Where(x => x.Title == "PageBannerForCart").SingleOrDefault() ?? new Silder();
            return PartialView(tempImagePageBanner);
        }
        #endregion
    }
}
