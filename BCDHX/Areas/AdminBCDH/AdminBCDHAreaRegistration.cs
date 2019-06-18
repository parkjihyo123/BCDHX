using System.Web.Mvc;

namespace BCDHX.Areas.AdminBCDH
{
    public class AdminBCDHAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AdminBCDH";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AdminBCDH_default",
                "AdminBCDH/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] {"BCDHX.Areas.AdminBCDH.Controllers"}
            );
        }
    }
}