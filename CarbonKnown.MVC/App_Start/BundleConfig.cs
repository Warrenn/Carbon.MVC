using System.Web.Optimization;

namespace CarbonKnown.MVC.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            bundles.UseCdn = true;
            var jqueryuicss = new StyleBundle("~/Content/jqueryui",
                                              "//code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css");
            bundles.Add(jqueryuicss);

            var datatablescss = new StyleBundle("~/Content/datatable",
                                                "//ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/css/jquery.dataTables.css");
            bundles.Add(datatablescss);

            var bootstrapcss = new StyleBundle("~/Content/bootstrapcss",
                                               "//netdna.bootstrapcdn.com/bootstrap/3.1.0/css/bootstrap.min.css");
            bundles.Add(bootstrapcss);

            var angularuicss = new StyleBundle("~/Content/angularuicss",
                                               "//cdnjs.cloudflare.com/ajax/libs/angular-ui/0.4.0/angular-ui.min.css");
            bundles.Add(angularuicss);

            var pickacolorcss = new StyleBundle("~/Content/pickacolorcss",
                                               "//cdn.jsdelivr.net/jquery.pick-a-color/1.1.7/css/pick-a-color-1.1.7.min.css");
            bundles.Add(pickacolorcss);

            var select2Css = new StyleBundle("~/Content/select2",
                                               "//cdnjs.cloudflare.com/ajax/libs/select2/3.4.5/select2.css");
            bundles.Add(select2Css);

            var googlefontscss = new StyleBundle("~/Content/googlefonts","//fonts.googleapis.com/css?family=Open+Sans:400,600,700");
            bundles.Add(googlefontscss);

            var zendeskcss = new StyleBundle("~/Content/zendesk", "//assets.zendesk.com/external/zenbox/v2.6/zenbox.css");
            bundles.Add(zendeskcss);

            bundles.Add(new StyleBundle("~/Content/landing").Include(
                "~/Content/stylesheets/landing.css"));
            bundles.Add(new StyleBundle("~/Content/census").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/stylesheets/census.css"));
            bundles.Add(new StyleBundle("~/Content/checklist").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/stylesheets/checklist.css"));
            bundles.Add(new StyleBundle("~/Content/comparison").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/stylesheets/comparison.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/crumbselector.css"));
            bundles.Add(new StyleBundle("~/Content/costcentre").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/stylesheets/costcentre.css"));
            bundles.Add(new StyleBundle("~/Content/dashboard").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/stylesheets/dashboard.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/crumbselector.css"));
            bundles.Add(new StyleBundle("~/Content/editsource").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/stylesheets/editsource.css"));
            bundles.Add(new StyleBundle("~/Content/input").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/stylesheets/input.css"));
            bundles.Add(new StyleBundle("~/Content/inputhistory").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/stylesheets/inputhistory.css"));
            bundles.Add(new StyleBundle("~/Content/overviewreport").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/stylesheets/overviewreport.css"));
            bundles.Add(new StyleBundle("~/Content/target").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/crumbselector.css",
                "~/Content/stylesheets/target.css"));
            bundles.Add(new StyleBundle("~/Content/tracesource").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/crumbselector.css",
                "~/Content/stylesheets/tracesource.css"));
            bundles.Add(new StyleBundle("~/Content/variance").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/stylesheets/variance.css"));
            bundles.Add(new StyleBundle("~/Content/uploadfile").Include(
                "~/Content/stylesheets/layout.css",
                "~/Content/jquery.mCustomScrollbar.css",
                "~/Content/stylesheets/uploadfile.css"));
            bundles.Add(new StyleBundle("~/Content/dashboardprint").Include(
                "~/Content/stylesheets/printlayout.css",
                "~/Content/stylesheets/dashboard.css")); 
            bundles.Add(new StyleBundle("~/Content/overviewreportprint").Include(
                "~/Content/stylesheets/printlayout.css",
                "~/Content/stylesheets/overviewreport.css"));
            
            var zenbox = new ScriptBundle("~/Scripts/zendesk", "//assets.zendesk.com/external/zenbox/v2.6/zenbox.js");
            bundles.Add(zenbox);

            var jquery = new ScriptBundle("~/Scripts/jquery", "//code.jquery.com/jquery-1.10.2.min.js");
            bundles.Add(jquery);

            var jqueryui = new ScriptBundle("~/Scripts/jqueryui", "//code.jquery.com/ui/1.10.3/jquery-ui.min.js");
            bundles.Add(jqueryui);

            var mousewheel = new ScriptBundle("~/Scripts/mousewheel",
                                              "//cdnjs.cloudflare.com/ajax/libs/jquery-mousewheel/3.0.6/jquery.mousewheel.min.js");
            bundles.Add(mousewheel);

            var jqueryvalidate = new ScriptBundle("~/Scripts/jqueryvalidate",
                                                  "//ajax.aspnetcdn.com/ajax/jquery.validate/1.11.1/jquery.validate.min.js");
            bundles.Add(jqueryvalidate);

            var tinycolorjs = new ScriptBundle("~/Scripts/tinycolorjs", "//cdn.jsdelivr.net/tinycolor/0.9.16/tinycolor-min.js");
            bundles.Add(tinycolorjs);

            var bootstrap = new ScriptBundle("~/Scripts/bootstrap", "//netdna.bootstrapcdn.com/bootstrap/3.1.0/js/bootstrap.min.js");
            bundles.Add(bootstrap);

            var fileapishim = new ScriptBundle("~/Scripts/fileapishim",
                                               "//cdn.jsdelivr.net/angular.file-upload/1.1.11/angular-file-upload-shim.min.js");
            bundles.Add(fileapishim);

            var highcharts = new ScriptBundle("~/Scripts/charts", "//code.highcharts.com/highcharts.js");
            bundles.Add(highcharts);

            var datatable = new ScriptBundle("~/Scripts/datatable",
                                             "//ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/jquery.dataTables.min.js");
            bundles.Add(datatable);

            var angular = new ScriptBundle("~/Scripts/angular",
                                           "//ajax.googleapis.com/ajax/libs/angularjs/1.2.6/angular.min.js");
            bundles.Add(angular);

            var select2 = new ScriptBundle("~/Scripts/select2",
                                               "///cdnjs.cloudflare.com/ajax/libs/select2/3.4.5/select2.min.js");
            bundles.Add(select2);

            var unobtrusive = new ScriptBundle("~/Scripts/unobtrusive",
                                               "//ajax.aspnetcdn.com/ajax/mvc/5.0/jquery.validate.unobtrusive.min.js");
            bundles.Add(unobtrusive);

            var pickacolorjs = new ScriptBundle("~/Scripts/pickacolorjs", "//cdn.jsdelivr.net/jquery.pick-a-color/1.1.8/pick-a-color.min.js");
            bundles.Add(pickacolorjs);

            var angularuijs = new ScriptBundle("~/Scripts/angularuijs",
                                               "//cdnjs.cloudflare.com/ajax/libs/angular-ui/0.4.0/angular-ui.min.js");
            bundles.Add(angularuijs);

            var fileapi = new ScriptBundle("~/Scripts/fileapi",
                                           "//cdn.jsdelivr.net/angular.file-upload/1.1.11/angular-file-upload.min.js");
            bundles.Add(fileapi);

            var uibootstrap = new ScriptBundle("~/Scripts/uibootstrap",
                                               "//cdnjs.cloudflare.com/ajax/libs/angular-ui-bootstrap/0.10.0/ui-bootstrap-tpls.min.js");
            bundles.Add(uibootstrap);

            var angularResource = new ScriptBundle("~/Scripts/angular-resource",
                                                   "//ajax.googleapis.com/ajax/libs/angularjs/1.2.6/angular-resource.min.js");
            bundles.Add(angularResource);

            var angularRoute = new ScriptBundle("~/Scripts/angular-route",
                                                "//ajax.googleapis.com/ajax/libs/angularjs/1.2.6/angular-route.min.js");
            bundles.Add(angularRoute);

            var angularSanitize = new ScriptBundle("~/Scripts/angular-sanitize",
                                                   "//ajax.googleapis.com/ajax/libs/angularjs/1.2.6/angular-sanitize.min.js");
            bundles.Add(angularSanitize);

            var angularanimate = new ScriptBundle("~/Scripts/angularanimate",
                                                  "//ajax.googleapis.com/ajax/libs/angularjs/1.2.6/angular-animate.min.js");
            bundles.Add(angularanimate);

            bundles.Add(new ScriptBundle("~/Scripts/login").Include(
                 "~/Scripts/customzendesk.js",
                "~/Scripts/Views/Account/login.js"));
            bundles.Add(new ScriptBundle("~/Scripts/census").Include(
                "~/Scripts/datefix.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/directives/customscroller.js",
                "~/Scripts/directives/hoverclass.js",
                 "~/Scripts/customzendesk.js",
                "~/Scripts/controllers/census.js"));
            bundles.Add(new ScriptBundle("~/Scripts/checklist").Include(
                 "~/Scripts/customzendesk.js",
                "~/Scripts/controllers/checklist.js"));
            bundles.Add(new ScriptBundle("~/Scripts/comparison").Include(
                "~/Scripts/datefix.js",
                "~/Scripts/customzendesk.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/crumbselector/crumbselector.js",
                "~/Scripts/angular-highcharts.js",
                "~/Scripts/directives/crumbselector.js",
                "~/Scripts/directives/hoverclass.js",
                "~/Scripts/controllers/comparison.js"));
            bundles.Add(new ScriptBundle("~/Scripts/costcentre").Include(
                "~/Scripts/customzendesk.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/angular-nested-sortable.js",
                "~/Scripts/directives/customscroller.js",
                "~/Scripts/directives/select2.js",
                "~/Scripts/directives/pickacolor.js",
                "~/Scripts/controllers/costcentre.js"));
            bundles.Add(new ScriptBundle("~/Scripts/dashboard").Include(
                "~/Scripts/datefix.js",
                "~/Scripts/customzendesk.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/crumbselector/crumbselector.js",
                "~/Scripts/angular-highcharts.js",
                "~/Scripts/filters/dashboard.js",
                "~/Scripts/directives/crumbselector.js",
                "~/Scripts/directives/hoverclass.js",
                "~/Scripts/directives/customscroller.js",
                "~/Scripts/directives/dashboard.js",
                "~/Scripts/controllers/dashboard.js"));
            bundles.Add(new ScriptBundle("~/Scripts/viewseditsourceindex").Include(
                "~/Scripts/customzendesk.js",
                "~/Scripts/DataTables-1.9.4/extras/Scroller/media/js/dataTables.scroller.min.js",
                "~/Scripts/Views/EditSource/index.js"));
            bundles.Add(new ScriptBundle("~/Scripts/input").Include(
                "~/Scripts/customzendesk.js",
                "~/Scripts/directives/select2.js",
                "~/Scripts/directives/hoverclass.js",
                "~/Scripts/controllers/input.js"));
            bundles.Add(new ScriptBundle("~/Scripts/viewsinputindex").Include(
                "~/Scripts/customzendesk.js",
                "~/Scripts/DataTables-1.9.4/extras/Scroller/media/js/dataTables.scroller.min.js",
                "~/Scripts/Views/Input/index.js"));
            bundles.Add(new ScriptBundle("~/Scripts/overviewreport").Include(
                "~/Scripts/customzendesk.js",
                "~/Scripts/directives/hoverclass.js",
                "~/Scripts/controllers/overviewreport.js"));
            bundles.Add(new ScriptBundle("~/Scripts/target").Include(
                "~/Scripts/datefix.js",
                "~/Scripts/customzendesk.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/crumbselector/crumbselector.js",
                "~/Scripts/directives/select2.js",
                "~/Scripts/directives/customscroller.js",
                "~/Scripts/directives/crumbselector.js",
                "~/Scripts/directives/hoverclass.js",
                "~/Scripts/controllers/target.js"));
            bundles.Add(new ScriptBundle("~/Scripts/tracesource").Include(
                "~/Scripts/datefix.js",
                "~/Scripts/customzendesk.js",
                "~/Scripts/DataTables-1.9.4/extras/Scroller/media/js/dataTables.scroller.min.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/crumbselector/crumbselector.js",
                "~/Scripts/directives/crumbselector.js",
                "~/Scripts/directives/customscroller.js",
                "~/Scripts/controllers/tracesource.js"));
            bundles.Add(new ScriptBundle("~/Scripts/variance").Include(
                "~/Scripts/customzendesk.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/directives/select2.js",
                "~/Scripts/directives/customscroller.js",
                "~/Scripts/directives/hoverclass.js",
                "~/Scripts/controllers/variance.js"));
            bundles.Add(new ScriptBundle("~/Scripts/upload").Include(
                "~/Scripts/customzendesk.js",
                "~/Scripts/jquery.mCustomScrollbar.js",
                "~/Scripts/directives/customscroller.js",
                "~/Scripts/controllers/upload.js"));
        }
    }
}