using System.Web;
using System.Web.Optimization;
using System.Web.Optimization.React;

namespace BCDHX
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new BabelBundle("~/bundles/React").Include(
                            "~/ScriptsBundle/React/react.development.js",
                            "~/ScriptsBundle/React/react-dom.development.js",
                            "~/ScriptsBundle/React/remarkable.min.js", 
                            "~/ScriptsBundle/React/Login.jsx"
                ));
            bundles.Add(new BabelBundle("~/bundles/Admin/React").Include(
                 "~/ScriptsBundle/React/react.development.js",
                 "~/ScriptsBundle/React/react-dom.development.js",
                "~/ScriptsBundle/React/remarkable.min.js",
                "~/ScriptsBundle/React/Admin/ListStaffAccount/ListStaffAccount.jsx"
                ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/popper.min.js",
                      "~/Scripts/bootstrap.js",
                      "~/Content/MainPage/js/plugins.js",
                      "~/Content/MainPage/js/main.js",
                      "~/Content/LoadingScreen/js/jquery.loadingModal.js",
                      "~/Content/SweetAlret/sweetalert2.all.js",
                      "~/Content/MainPage/js/jquery.validate.js"
                      ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/MainPage/css/bootstrap.min.css",                               
                      "~/Content/MainPage/css/plugins.css",
                       "~/Content/MainPage/css/themify-icons.css",
                        "~/Content/MainPage/css/icofont.css",
                          "~/Content/MainPage/css/all.css",
                      "~/Content/MainPage/css/style.css"                  
                    ) );
            bundles.Add(new StyleBundle("~/AdminContent/css").Include(
              "~/Content/AdminPage/assets/css/normalize.min.css",
                   "~/Content/MainPage/css/bootstrap.min.css",
                  "~/Content/AdminPage/assets/css/all.css",
                   "~/Content/AdminPage/assets/css/themify-icons.css",
                  "~/Content/AdminPage/assets/css/pe-icon-7-stroke.css",
                     "~/Content/AdminPage/assets/css/flag-icon.min.css",
                   "~/Content/AdminPage/assets/css/cs-skin-elastic.css",
                   "~/Content/SweetAlret/sweetalert2.min.css",
                   "~/Content/AdminPage/assets/css/style.css")
                  );
            bundles.Add(new ScriptBundle("~/AdminBCDH/JS").Include(
                   "~/Scripts/popper.min.js",
                     "~/Scripts/bootstrap.js",
                     "~/Content/SweetAlret/sweetalert2.all.js",
                     "~/Content/AdminPage/assets/js/main.js"
                     //"~/ScriptsBundle/AdminJquery/ListStaffAccount/ListStaff.js"
                     ));
        }
    }
}
