
namespace MyCompany.Visitors.Web
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    /// <summary>
    /// MvcApplication
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application_Start
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            IdentityConfig.ConfigureIdentity();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FormatConfig.ConfigureFormats(GlobalConfiguration.Configuration);
            DependencyConfig.ResolveDependencies(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }

        /// <summary>
        /// Application_BeginRequest
        /// </summary>
        protected void Application_BeginRequest()
        {
            //if (this.Request.Url.ToString().ToLowerInvariant().Contains("signalr") || this.Request.Url.ToString().ToLowerInvariant().Contains("api"))
            //{ }
            //else if (!this.Request.Url.ToString().ToLowerInvariant().Contains("/noauth"))
            //{
            //    Response.Redirect(this.Request.Url.ToString() + "noauth");
            //}
        }
    }
}