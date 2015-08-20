using System.Web;
using System.Web.Optimization;

namespace ASI.MGC.FS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/JqueryUi").Include(
                "~/Scripts/jquery-ui.min.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            /** Start - Adding Custom CSS Files for Master Page Dated 23-05-2015 By Zeeshan Mahmood**/

            bundles.UseCdn = true;

            bundles.Add(new StyleBundle("~/fonts").Include(
                "~/fonts/fontawesome-webfont862f.eot",
                "~/fonts/fontawesome-webfont862f.ttf",
                "~/fonts/fontawesome-webfont862f.woff",
                "~/fonts/fontawesome-webfontd41d.eot",
                "~/fonts/glyphicons-halflings-regular.eot",
                "~/fonts/glyphicons-halflings-regular.ttf",
                "~/fonts/glyphicons-halflings-regular.woff",
                "~/fonts/glyphicons-halflings-regulard41d.eot"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/font-awesome.min.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap-reset.css",
                      "~/Content/GoogleFontFace.css",
                      "~/Content/style.css",
                      "~/Content/jquery.easy-pie-chart.css",
                      "~/Content/owl.carousel.css",
                      "~/Content/owl.theme.css",
                      "~/Content/owl.transitions.css"));
            /** End - Adding Custom CSS Files for Master Page Dated 23-05-2015 By Zeeshan Mahmood**/
        }
    }
}
