namespace MyCompany.Visitors.Web
{
    using System.Web.Optimization;

    /// <summary>
    /// Bundle Config
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Register bundles
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/vendor")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/knockout-{version}.js")
                .Include("~/Scripts/knockout.mapping-latest.js")
                .Include("~/Scripts/knockout.validation.js")
                .Include("~/Scripts/sammy-{version}.js")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/jquery.dotdotdot-1.5.9.js")
                .Include("~/Content/select2/select2.js")
                .Include("~/Scripts/jquery.signalR-{version}.js")
                .Include("~/Scripts/toastr.js")
                .Include("~/Scripts/jquery.imgareaselect.js")
                );

            bundles.Add(new ScriptBundle("~/scripts/app")
                .Include("~/Scripts/knockout.bindings.js")
                );

            bundles.Add(new ScriptBundle("~/scripts/modernizr")
                .Include("~/Scripts/modernizr-*")
                );

            bundles.Add(new ScriptBundle("~/scripts/bootstrap")
                .Include("~/Scripts/bootstrap.js")
                );

            bundles.Add(new StyleBundle("~/Content/styles")
                .Include("~/Content/ie10mobile.css")
                .Include("~/Content/durandal.css")
                .Include("~/Content/select2/select2.css")
                .Include("~/Content/imgareaselect-animated.css")
                .Include("~/Content/site.css")
                .Include("~/Content/toastr.css")
                .Include("~/Content/visitorForm.css")
                .Include("~/Content/list.css")
                .Include("~/Content/visitors.css")
                .Include("~/Content/visits.css")
                .Include("~/Content/messageBox.css")
                .Include("~/Content/visitorDetail.css")
                .Include("~/Content/visitDetail.css")
                .Include("~/Content/visitForm.css")
                );

            bundles.Add(new StyleBundle("~/Content/bootstrap/css")
                .Include("~/Content/bootstrap/bootstrap.css")
                );

            bundles.Add(new ScriptBundle("~/scripts/faqvendor")
                 .Include("~/Scripts/jquery-{version}.js")
            );
            bundles.Add(new ScriptBundle("~/scripts/mainLayout")
                 .Include("~/Scripts/mainLayout.js")
            );

            bundles.Add(new StyleBundle("~/Content/faqcss")
                .Include("~/Content/toastr.css")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/site.css")
                .Include("~/Content/faq.css")
            );
        }
    }
}