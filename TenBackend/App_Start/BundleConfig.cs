using System.Web;
using System.Web.Optimization;

namespace TenBackend
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
           

            //DataTable
            bundles.Add(new ScriptBundle("~/bundles/dataTable").Include(
                "~/Scripts/jquery.dataTables.min.js",
                "~/Scripts/metisMenu.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",                   
                      "~/Content/font-awesome.min.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/metisMenu.min.css"));
            //User template Scripts
            bundles.Add(new ScriptBundle("~/bundles/sb-admin-2").Include("~/Scripts/sb-admin-2.js"));
            //Admmin.js
            bundles.Add(new ScriptBundle("~/bundles/admin").Include("~/Scripts/admin.js"));
            //mytools.js
            bundles.Add(new ScriptBundle("~/bundles/mytools").Include("~/Scripts/mytools.js"));


            //KeFu Login CSS
            bundles.Add(new StyleBundle("~/Content/css/kefu-login").Include(
                    "~/Content/KeFu/login.css"
                   ));

            bundles.Add(new StyleBundle("~/Content/css/sbadmin").Include(
                      "~/Content/sb-admin-2.css"));
             
        }
    }
}
