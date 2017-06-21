
namespace MyCompany.Visitors.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Config Routes
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Register Routes
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            // NoAuth route is only for demos without internet access
            // this route is not securized and all the requests are done with a dummy user
            routes.MapRoute(
                name: "NoAuth",
                url: "NoAuth/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}