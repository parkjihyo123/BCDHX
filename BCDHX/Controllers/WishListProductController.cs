using BCDHX.Models.ModelObject;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BCDHX.Controllers
{
    [Authorize(Roles = "Customer")]
    public class WishListProductController : Controller
    {
        private WebDieuHienDB _dbBCDHX;
        private ApplicationUserManager _userManager;
        private RandomCode _randomcode;
        public WishListProductController()
        {
            _dbBCDHX = new WebDieuHienDB();
            _randomcode = new RandomCode();

        }
        public WishListProductController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [HttpPost]
        public async Task<JsonResult> AddWishList(string productID)
        {
            if (productID != null || productID != "")
            {
                var userTemp = await UserManager.FindByEmailAsync(User.Identity.Name);
                if (userTemp != null)
                {
                    var productExsit = _dbBCDHX.WishLists.AsNoTracking().Where(x => x.ID_Account == userTemp.Id && x.ID_Product == productID).SingleOrDefault();
                    if (productExsit != null)
                    {
                        return Json(new { Error = "Sản phẩm đã tồn tại trong wishList", Status = 3 });
                    }
                    else
                    {
                        var productTemp = _dbBCDHX.Products.AsNoTracking().Where(x => x.ID_Product == productID).SingleOrDefault();
                        if (productTemp != null)
                        {
                            var wishListID = _randomcode.RandomNumber();
                            WishList wishListmodel = new WishList { ID_Account = userTemp.Id, ID_Product = productID, ID_WishList = wishListID };
                            _dbBCDHX.Entry(wishListmodel).State = System.Data.Entity.EntityState.Added;
                            _dbBCDHX.SaveChanges();
                            return Json(new { Error = "Thêm sản phẩm thành công vào wishList", Status = 1 });
                        }
                        else
                        {
                            return Json(new { Error = "Lỗi sảy ra", Status = 2 });
                        }

                    }
                }
                else
                {
                    return Json(new { Error = "Bạn phải đăng nhập mới thêm sản phẩm vào wishList của mình được!", Status = 4 });
                }

            }
            else
            {
                return Json(new { Error = "Lỗi sảy ra", Status = 2 });
            }
        }
        public async Task<ActionResult> Index()
        {
            var userTemp = await UserManager.FindByEmailAsync(User.Identity.Name);
            var tempProductId = _dbBCDHX.WishLists.Where(x => x.ID_Account == userTemp.Id).Select(x => new { wishListId = x.ID_WishList, ID_Product = x.ID_Product });
            WishListViewModel tempWishListViewModel = new WishListViewModel();
            List<Models.ModelObject.Product> tempWishListProduct = new List<Models.ModelObject.Product>();
            tempWishListViewModel.ID_Account = userTemp.Id;
            foreach (var item in tempProductId)
            {
                var products = (from p in _dbBCDHX.Products
                                join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                                join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                                where p.Quantity > 0
                                orderby p.Name_Product
                                select new Models.ModelObject.Product
                                {
                                    ID_Product = p.ID_Product,
                                    ID_Category = p.ID_Category,
                                    Name_Product = p.Name_Product,
                                    Quantity = p.Quantity.Value,
                                    Price = p.Price.Value.ToString(),
                                    Status = p.Status.Value,
                                    Sale = p.Sale.Value.ToString(),
                                    Img = e.IMG1,
                                    Description = p.Description,
                                    NameCatogory = g.Name_Category
                                }
                         ).Where(x => x.ID_Product == item.ID_Product).SingleOrDefault();

                if (products != null)
                {
                    tempWishListViewModel.IdWishList = item.wishListId;
                    tempWishListProduct.Add(products);
                }
            }
            tempWishListViewModel.ListProductWishList = tempWishListProduct;
            return View(tempWishListViewModel);
        }
        [HttpPost]
        public async Task<JsonResult> RemoveWishListItem(string productId)
        {
            if (productId != null || productId != "")
            {
                var userTemp = await UserManager.FindByEmailAsync(User.Identity.Name);
                var tempWishList = _dbBCDHX.WishLists.AsNoTracking().Where(x => x.ID_Product == productId && x.ID_Account == userTemp.Id).SingleOrDefault();
                _dbBCDHX.Entry(tempWishList).State = System.Data.Entity.EntityState.Deleted;
                _dbBCDHX.SaveChanges();
                return Json(new { Error = "Xóa item thành công", Status = 0 });
            }
            else
            {
                return Json(new { Error = "Lỗi xảy ra", Status = 1 });
            }

        }
        public PartialViewResult PageBannerForWishList()
        {
            Silder tempImagePageBanner = _dbBCDHX.Silders.Where(x => x.Title == "PageBannerForWishList").SingleOrDefault() ?? new Silder();
            return PartialView(tempImagePageBanner);
        }
    }
}