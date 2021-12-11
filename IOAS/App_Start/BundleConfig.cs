using System.Web;
using System.Web.Optimization;

namespace IOAS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //JSGRID 
            bundles.Add(new ScriptBundle("~/bundles/JSGrid")
               .Include("~/Content/IOASContent/js/JSGrid/jsgrid.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JSGrid_Exclude_Core")
              .Include("~/Content/IOASContent/js/JSGrid/jsgrid_exclude_core.min.js"));

            //JSGRID - style files - ProcessGuideline
            bundles.Add(new StyleBundle("~/bundles/ProcessGuideline")
                    .Include("~/Content/IOASContent/css/Developercss/Style.css"));

            bundles.Add(new StyleBundle("~/bundles/JSGridCss")
                    .Include("~/Content/IOASContent/css/jsgrid.css")
                    .Include("~/Content/IOASContent/css/custom-jsgrid.css")
                    .Include("~/Content/IOASContent/img/jsgrid-icons.png")
                    .Include("~/Content/IOASContent/img/jsgrid-icons-white.png")
                    );



        }
    }
}
