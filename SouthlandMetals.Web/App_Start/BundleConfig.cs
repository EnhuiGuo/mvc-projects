using System.Web.Optimization;

namespace SouthlandMetals.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.2.0.min.js",
                        "~/Scripts/jquery.signalR-2.2.1.min.js",
                        "~/Scripts/DataTables/jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.select.min.js",
                        "~/Scripts/DataTables/dataTables.buttons.min.js",
                        "~/Scripts/DataTables/dataTables.bootstrap.min.js",
                        "~/Scripts/DataTables/buttons.bootstrap.min.js",
                        "~/Scripts/DataTables/buttons.flash.min.js",
                        "~/Scripts/DataTables/buttons.html5.min.js",
                        "~/Scripts/DataTables/buttons.print.min.js",
                        "~/Scripts/DataTables/dataTables.scroller.min.js",
                        "~/Scripts/DataTables/dataTables.responsive.min.js",
                        "~/Scripts/DataTables/responsive.bootstrap.min.js",
                        "~/Scripts/jquery-unobtrusive-ajax.js",
                        "~/Scripts/jquery.confirm.js",
                        "~/Scripts/jquery.jeditable.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-3.4.0.js",
                        "~/Scripts/knockout-sortable.min.js",
                        "~/Scripts/knockout.mapping-latest.js"));

            bundles.Add(new ScriptBundle("~/bundles/select").Include(
                     "~/Content/Selectize/js/standalone/selectize.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/spin").Include(
                     "~/Scripts/spin.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                        "~/Scripts/southlandMetals-common.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/bootstrap-datepicker3.min.css",
                        "~/Content/DataTables/css/buttons.bootstrap.min.css",
                        "~/Content/DataTables/css/select.dataTables.min.css",
                        "~/Content/DataTables/css/buttons.dataTables.min.css",
                        "~/Content/DataTables/css/scroller.bootstrap.min.css",
                        "~/Content/DataTables/css/scroller.dataTables.min.css",
                        "~/Content/DataTables/css/responsive.dataTables.min.css",
                        "~/Content/DataTables/css/dataTables.bootstrap.min.css",
                        "~/Content/DataTables/css/responsive.bootstrap.min.css",
                        "~/Content/southlandMetals-main.css",
                        "~/Content/southlandMetals-icons.css",
                        "~/Content/southlandMetals-print.css",
                        "~/Content/southlandMetals-sidebar.css",
                        "~/Content/southlandMetals-login.css",
                        "~/Content/font-awesome.min.css",
                        "~/Content/awesome-bootstrap-checkbox.css",
                        "~/Content/Selectize/css/selectize.bootstrap3.css"));
        }
    }
}
