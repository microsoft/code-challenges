
namespace MyCompany.Visitors.Web
{
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.ExceptionHandling;

    /// <summary>
    /// WebApiConfig
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register Routes
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            // NoAuth routes are only for demos without internet access
            // this routes are not securized and all the requests are done with a dummy user
            config.Routes.MapHttpRoute(
                name: "DefaultApiNoAuth",
                routeTemplate: "noauth/api/{controller}",
                defaults: null
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}",
                defaults: null
            );

            config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger());
        }
    }
}
