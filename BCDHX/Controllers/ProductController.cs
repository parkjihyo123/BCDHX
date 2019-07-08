using BCDHX.Moduns.Models;
using System.Linq;
using System.Web.Mvc;
using BCDHX.Models.ModelObject;
using System.Collections.Generic;
using System;
using PagedList;
namespace BCDHX.Controllers
{
    public class ProductController : Controller
    {
        private WebDieuHienDB _dbBCDHX;

        public ProductController()
        {
            _dbBCDHX = new WebDieuHienDB();

        }


        public ActionResult Index(int? page, int? pagesize, string sortby, string selectmode)
        {
            string sortBy = (sortby ?? "def");
            string selectMode = (selectmode ?? "grid");
            int pageNumber = (page ?? 1);
            int pageSizeNumber = (pagesize ?? 8);
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
                         ).ToList();
            ViewBag.tempSortBy = sortby;
            ViewBag.Temppagesize = pageSizeNumber;
            ViewBag.SelectMode = selectMode;
            switch (sortby)
            {
                case "trending":
                    products = products.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 3 || x.Status == 2).ToList();
                    break;
                case "sales":
                    products = products.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 3).ToList();
                    break;
                case "bestdeal":
                    products = products.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 4).ToList();
                    break;
                case "date":
                    products = products.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 2).ToList();
                    break;
                case "priceasc":
                    products = (from p in _dbBCDHX.Products
                                join e in _dbBCDHX.ImageForProducts
                                on p.ID_Product equals e.ID_Product
                                where p.Quantity > 0
                                orderby p.Price
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
                                    Description = p.Description
                                }
                         ).ToList();
                    break;
                case "pricedesc":
                    products = (from p in _dbBCDHX.Products
                                join e in _dbBCDHX.ImageForProducts
                                on p.ID_Product equals e.ID_Product
                                where p.Quantity > 0
                                orderby p.Price descending
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
                                    Description = p.Description
                                }
                         ).ToList();
                    break;
                default:

                    break;
            }
            return View(products.ToPagedList(pageNumber, pageSizeNumber));
        }

        public ActionResult ProductByCategory(string categoryname, int? page, int? pagesize, string sortby, string selectmode)
        {
            string sortBy = (sortby ?? "def");
            string selectMode = (selectmode ?? "grid");
            int pageNumber = (page ?? 1);
            int pageSizeNumber = (pagesize ?? 8);
            var tempProductWithCategory = (from p in _dbBCDHX.Products
                                           join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                                           join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                                           where p.Quantity>0
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

                                           }).ToList().Where(x => x.NameCatogory == categoryname);
            ViewBag.tempSortBy = sortby;
            ViewBag.Temppagesize = pageSizeNumber;
            ViewBag.SelectMode = selectMode;
            switch (sortby)
            {
                case "trending":
                    tempProductWithCategory = tempProductWithCategory.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 3 || x.Status == 2).ToList();
                    break;
                case "sales":
                    tempProductWithCategory = tempProductWithCategory.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 3).ToList();
                    break;
                case "bestdeal":
                    tempProductWithCategory = tempProductWithCategory.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 4).ToList();
                    break;
                case "date":
                    tempProductWithCategory = tempProductWithCategory.Select(x => x).OrderBy(x => x.Name_Product).Where(x => x.Status == 2).ToList();
                    break;
                case "priceasc":
                    tempProductWithCategory = (from p in _dbBCDHX.Products
                                               join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                                               join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                                               where p.Quantity > 0
                                               orderby p.Price
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
                                                   Description = p.Description
                                               } ).ToList();                       
                    break;
                    case "pricedesc":
                    tempProductWithCategory = (from p in _dbBCDHX.Products
                                               join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                                               join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                                               where p.Quantity > 0
                                               orderby p.Price descending
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
                                               } ).ToList();
                    break;
                    default:
                    break;
            }
            ViewBag.tempSortBy = sortby;
            ViewBag.Temppagesize = pageSizeNumber;
            ViewBag.SelectMode = selectMode;
            return View(tempProductWithCategory.ToPagedList(pageNumber, pageSizeNumber));
        }


        public ActionResult ProductDetail(string productid)
        {
            var tempProductDetail = (from p in _dbBCDHX.Products
                                     join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                                        join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                                     where p.Quantity > 0
                                     select new Models.ModelObject.ProductDetailModelView
                                     {
                                         ID_Product = p.ID_Product,
                                         ID_Category = p.ID_Category,
                                         Name_Product = p.Name_Product,
                                         Quantity = p.Quantity.Value,
                                         Price = p.Price.Value.ToString(),
                                         Status = p.Status.Value,
                                         Sale = p.Sale.Value.ToString(),
                                         Img1 = e.IMG1,
                                         Img2 = e.IMG2,
                                         Img3 = e.IMG3,
                                         Img4 = e.IMG4,
                                         Description = p.Description,
                                         NameCatogory = g.Name_Category
                                     }).Where(x=>x.ID_Product==productid).SingleOrDefault();
            if (tempProductDetail!=null)
            {
                ViewBag.RelatedProduct = (tempProductDetail.NameCatogory ?? null);
            }
         
            return View(tempProductDetail);
        }


        public PartialViewResult RealtedProduct(string NameCatogory)
        {
            var tempRealtedProduct = (from p in _dbBCDHX.Products
                                     join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                                     join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                                     where p.Quantity > 0
                                     select new Models.ModelObject.ProductDetailModelView
                                     {
                                         ID_Product = p.ID_Product,
                                         ID_Category = p.ID_Category,
                                         Name_Product = p.Name_Product,
                                         Quantity = p.Quantity.Value,
                                         Price = p.Price.Value.ToString(),
                                         Status = p.Status.Value,
                                         Sale = p.Sale.Value.ToString(),
                                         Img1 = e.IMG1,
                                         Img2 = e.IMG2,
                                         Img3 = e.IMG3,
                                         Img4 = e.IMG4,
                                         Description = p.Description,
                                         NameCatogory = g.Name_Category
                                     }).Where(x => x.NameCatogory == NameCatogory).ToList();
            return PartialView(tempRealtedProduct);
        }

        private PagingProduct GetProductForPaging(int currentPage)
        {
            PagingProduct tempPagingProduct = new PagingProduct();
            tempPagingProduct.Products = (from p in _dbBCDHX.Products
                                          join e in _dbBCDHX.ImageForProducts
                                          on p.ID_Product equals e.ID_Product
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
                                              Description = p.Description
                                          }
                         ).OrderBy(x => x.ID_Product).Skip((currentPage - 1) * 4).Take(4).ToList();
            double pageCount = (double)((decimal)_dbBCDHX.Products.Count() / Convert.ToDecimal(4));
            tempPagingProduct.PageCount = (int)Math.Ceiling(pageCount);
            tempPagingProduct.CurrentPageIndex = currentPage;
            return tempPagingProduct;
        }
        [HttpGet]
        public JsonResult GetDefaulPage()
        {

            double pageCount = (double)((decimal)_dbBCDHX.Products.Count() / Convert.ToDecimal(4));
            int rs = (int)Math.Ceiling(pageCount);
            List<int> temp = new List<int>();
            for (int i = 1; i <= rs; i++)
            {
                temp.Add(i);
            }
            return Json(new { temp }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// PartialView Feature
        /// </summary>
        /// <returns></returns>
        public PartialViewResult FeaturePartial()
        {
            TempData["CategoryName"] = GetCategoryFeature();
            return PartialView(GetProductFeature());
        }
        /// <summary>
        /// Best Seller
        /// </summary>
        /// <returns></returns>
        public PartialViewResult BestSeller()
        {
            var temp = (from p in _dbBCDHX.Products
                        join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                        join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                        where p.Status == 3 && p.Quantity > 0 || p.BestSale == true
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
                       ).ToList();

            return PartialView(temp);
        }
        public PartialViewResult BestDeal()
        {
            TempData["CategoryBD"] = GetCategoryBD();
            TempData["BestDealContent"] = GetBestDealContent();
            return PartialView(GetProductBestDeal());
        }
        public PartialViewResult NewArrival()
        {
            var temp = GetNewArrivalProduct();
            return PartialView(temp);
        }
        #region
        //Get Category Best Deal
        private List<CategoryBestDeal> GetCategoryBD()
        {
            var temp = _dbBCDHX.Categories.Where(x => x.StatusCategory == 4 && x.Isactive == true);
            List<CategoryBestDeal> t = new List<CategoryBestDeal>();
            foreach (var item in temp)
            {
                CategoryBestDeal CategoryTemp = new CategoryBestDeal
                {
                    CategoryID = item.ID_Category,
                    CategoryName = item.Name_Category

                };
                t.Add(CategoryTemp);
            }
            return t;
        }

        private List<ProductBestDeal> GetProductBestDeal()
        {
            var temp = (from p in _dbBCDHX.Products
                        join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                        join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                        where g.StatusCategory == 4 && p.Quantity > 0 && p.Status != 0 || p.Status == 4
                        select new Models.ModelObject.ProductBestDeal
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
                      ).ToList();
            return temp;
        }
        private BestDealContent GetBestDealContent()
        {
            var temp = _dbBCDHX.BestDeals.Select(x => x).Where(x => x.Status == true);
            BestDealContent t = null;
            foreach (var item in temp)
            {
                t = new BestDealContent
                {
                    BestDealID = item.BestDealID,
                    BestDealName = item.BestDealName,
                    Status = item.Status.Value,
                    CreateBy = item.CreateBy,
                    CreateDate = item.CreateDate.Value,
                    DateExprie = item.DateExprie.Value,
                    DateModify = item.DateModify.Value,
                    ModifyBy = item.ModifyBy,
                    PercentSale = item.PercentSale,
                    Content = item.Content
                };
            }
            return t;
        }
        #endregion

        #region
        //Get Feature
        /// <summary>
        /// Get category for feature
        /// </summary>
        /// <returns></returns>
        private List<CategoryFeature> GetCategoryFeature()
        {
            var temp = _dbBCDHX.Categories.Select(x => x).Take(7).ToList();
            List<CategoryFeature> g = new List<CategoryFeature>();
            foreach (var item in temp)
            {
                CategoryFeature t = new CategoryFeature
                {
                    CategoryID = item.ID_Category,
                    CategoryName = item.Name_Category
                };
                g.Add(t);
            }
            return g;
        }
        /// <summary>
        /// get product for feature
        /// </summary>
        /// <returns></returns>
        private ILookup<string, ProductFeature> GetProductFeature()
        {
            var temp = (from p in _dbBCDHX.Products                      
                        join e in _dbBCDHX.ImageForProducts on p.ID_Product equals e.ID_Product
                        join g in _dbBCDHX.Categories on p.ID_Category equals g.ID_Category
                        where p.Quantity > 0 && p.Status != 0
                        select new Models.ModelObject.ProductFeature
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
                            NameCatogory=g.Name_Category
                        }
                          ).ToList();
            return temp.ToLookup(x => x.ID_Category);
        }
        #endregion

        #region
        private List<NewArrivalProduct> GetNewArrivalProduct()
        {

            var dbNewArrivalProduct = _dbBCDHX.Products.Join(_dbBCDHX.ImageForProducts, pro => pro.ID_Product, image => image.ID_Product, (proN, imageN) =>new {Product =proN,ImageForProduct=imageN }).Join(_dbBCDHX.Categories,pro1=>pro1.Product.ID_Category,image1=>image1.ID_Category,(pro1N,image1N)=>new {Product=pro1N,Category=image1N }).Select(x=>new NewArrivalProduct { Description = x.Product.Product.Description, ID_Category = x.Category.ID_Category, Quantity = x.Product.Product.Quantity.Value, ID_Product = x.Product.Product.ID_Product, Status = x.Product.Product.Status.Value, Img = x.Product.Product.Img, Name_Product = x.Product.Product.Name_Product, Price = x.Product.Product.Price.Value.ToString(), Sale = x.Product.Product.Sale.Value.ToString(),NameCatogory=x.Category.Name_Category}).Where(x =>x.Status==2 && x.Quantity > 0).ToList();                
            return dbNewArrivalProduct;
        }
        #endregion
    }
}