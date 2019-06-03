using BCDHX.Moduns.Models;
using System.Linq;
using System.Web.Mvc;
using BCDHX.Models.ModelObject;
using System.Collections.Generic;
namespace BCDHX.Controllers
{
    public class ProductController : Controller
    {
        private WebDieuHienDB _dbBCDHX;

        public ProductController()
        {
            _dbBCDHX = new WebDieuHienDB();

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
                        where p.Status == 3 &&p.Quantity>0 ||p.BestSale == true 
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
                        where g.StatusCategory == 4 && p.Quantity>0 && p.Status !=0 ||p.Status==4
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
            BestDealContent t =null;
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
            var temp = _dbBCDHX.Categories.Select(x =>x).Take(7).ToList();
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
                        join e in _dbBCDHX.ImageForProducts
                        on p.ID_Product equals e.ID_Product
                        where p.Quantity >0 && p.Status!=0
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
                            Description = p.Description
                        }
                          ).ToList();
            return temp.ToLookup(x => x.ID_Category);
        }
        #endregion

        #region
        private List<NewArrivalProduct> GetNewArrivalProduct()
        {

            var dbNewArrivalProduct = _dbBCDHX.Products.Select(x => new NewArrivalProduct { Description = x.Description, ID_Category = x.ID_Category, Quantity = x.Quantity.Value, ID_Product = x.ID_Product, Status = x.Status.Value,Img=x.Img,Name_Product=x.Name_Product,Price=x.Price.Value.ToString(),Sale=x.Sale.Value.ToString()}).Where(x => x.Status == 2 &&x.Quantity>0).ToList();
            return dbNewArrivalProduct;
        }
        #endregion
    }
}