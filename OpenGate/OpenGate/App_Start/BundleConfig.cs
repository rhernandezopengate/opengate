using System.Web;
using System.Web.Optimization;

namespace OpenGate
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/CssPrimary").Include(
                "~/vendors/bootstrap/dist/css/bootstrap.min.css",
                "~/vendors/nprogress/nprogress.css",               
                "~/build/css/custom.min.css",
                "~/Content/CalendarCSS/JqueryCalendarCSS.css"                
                ));

            bundles.Add(new ScriptBundle("~/iCheck").Include(
                "~/vendors/iCheck/icheck.min.js"
                ));

            bundles.Add(new ScriptBundle("~/Custom").Include(
                "~/build/js/custom.min.js"));

            bundles.Add(new ScriptBundle("~/JQueryUI").Include(
                "~/Scripts/JQueryUI/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/DateTimePcker").Include(
               "~/vendors/moment/min/moment.min.js",
               "~/vendors/bootstrap-datetimepicker/build/js/bootstrap-datetimepicker.min.js"));

            bundles.Add(new ScriptBundle("~/Progressbar").Include(
                "~/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js"));

        }
    }
}
