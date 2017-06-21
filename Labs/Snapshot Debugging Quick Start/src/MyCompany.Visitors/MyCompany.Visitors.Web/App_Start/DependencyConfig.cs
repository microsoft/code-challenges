namespace MyCompany.Visitors.Web
{
    using System.Web.Http;

    /// <summary>
    /// Resolve dependencies
    /// </summary>
    public class DependencyConfig
    {
        /// <summary>
        /// Register trace listener
        /// </summary>
        /// <param name="config">HttpConfiguration</param>
        public static void ResolveDependencies(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityDependencyResolver();
        }
    }
}