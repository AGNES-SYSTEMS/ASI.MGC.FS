using System.Web.Optimization;

namespace ASI.MGC.FS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"//,
                //"~/Scripts/jquery.unobtrusive-ajax.min.js"
                        ));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.unobtrusive*"));

            bundles.Add(new ScriptBundle("~/bundles/JqueryUi").Include(
                "~/Scripts/jquery-ui.min.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/libraries").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/i18n/grid.locale-en.js",
                        "~/Scripts/jquery.jqGrid.min.js",
                        "~/Scripts/bootstrapValidator.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/formValidation.min.js",
                        "~/Scripts/bootstrap.framework.js",
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
                      "~/Content/bootstrap.typeahead.css",
                      "~/Content/style.css",
                      "~/Content/jquery.easy-pie-chart.css",
                      "~/Content/owl.carousel.css",
                      "~/Content/owl.theme.css",
                      "~/Content/owl.transitions.css",
                      "~/Content/toastr.min.css",
                      "~/Content/ui.jqgrid-bootstrap.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/jquery-ui.structure.min.css",
                      "~/Content/themes/base/jquery-ui.theme.css",
                      "~/Content/themes/base/jquery.ui.all.css",
                      "~/Content/jquery.jqGrid/ui.jqgrid.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrapValidator.min.css",
                      "~/Content/formValidation.min.css"));
            /** End - Adding Custom CSS Files for Master Page Dated 23-05-2015 By Zeeshan Mahmood**/
        }
    }
}
