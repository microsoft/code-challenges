
namespace MyCompany.Visitors.Web
{
    using Owin;
    using System.Configuration;
    using Microsoft.Owin.Security.ActiveDirectory;

    /// <summary>
    /// Configure OAuth Security
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Audience = ConfigurationManager.AppSettings["ida:Audience"],
                    Tenant = ConfigurationManager.AppSettings["ida:Tenant"]
                });
        }
    }
}