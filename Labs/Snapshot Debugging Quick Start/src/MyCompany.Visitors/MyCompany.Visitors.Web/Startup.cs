namespace MyCompany.Visitors.Web
{
    using Owin;

    /// <summary>
    /// Startup
    /// </summary>
    public partial class Startup 
    {
        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            ConfigureSignalR(app);
        }
    }
}