﻿using System.Web.Optimization;

namespace Presentation.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // disable bundling and minification... it causes issues :(
            // TODO fix the errors when bundling and minifying
            BundleTable.EnableOptimizations = false;

            // jQuery and plugins
            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/holder.js",
                "~/Scripts/select2.js",
                "~/Scripts/moment.js"));

            /*
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
             * 
             */

            // standalone libraries
            bundles.Add(new ScriptBundle("~/Scripts/libraries").Include(
                "~/Scripts/underscore.js",
                "~/Scripts/lodash.js",
                "~/Scripts/bootstrap.js"));

            // angularjs and plugins
            bundles.Add(new ScriptBundle("~/Scripts/angular").Include(
                "~/Scripts/angular.js",
                "~/Scripts/i18n/angular-locale_da-dk.js",
                "~/Scripts/angular-animate.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/AngularUI/ui-router.js",
                "~/Scripts/angular-ui/ui-bootstrap.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                "~/Scripts/ui-select2.js",
                "~/Scripts/notify/*.js",
                "~/Scripts/angular-ui-util/ui-utils.js"));

            // angular app files
            bundles.Add(new ScriptBundle("~/Scripts/appbundle").IncludeDirectory(
                "~/Scripts/app", "*.js", true));

            // css
            bundles.Add(new StyleBundle("~/Content/cssbundle").Include(
                "~/Content/bootstrap/bootstrap.css",
                "~/Content/notify/notify.css",
                "~/Content/select2.css",
                "~/Content/select2-bootstrap.css",
                "~/Content/kitos.css"));
        }
    }
}