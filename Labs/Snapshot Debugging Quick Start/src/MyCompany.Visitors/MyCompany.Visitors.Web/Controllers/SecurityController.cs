namespace MyCompany.Visitors.Web.Controllers
{
    using System;
    using System.Configuration;
    using System.IdentityModel.Services;
    using System.IdentityModel.Services.Configuration;
    using System.Web.Http;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using MyCompany.Visitors.Web.Infraestructure.Security;


    /// <summary>
    /// Security Controller
    /// </summary>
    [RoutePrefix("api/security")]
    [MyCompanyAuthorization]
    public class SecurityController : ApiController
    {
        /// <summary>
        /// Get logout url.
        /// </summary>
        /// <returns></returns>
        [Route("logoutUrl")]
        [Route("~/noauth/api/security/logoutUrl")]
        public string GetLogoutUrl()
        {
            // Load Identity Configuration
            FederationConfiguration config = FederatedAuthentication.FederationConfiguration;

            // Get wtrealm from WsFederationConfiguation Section
            string wtrealm = config.WsFederationConfiguration.Realm;
            string wsFederationEndpoint = ConfigurationManager.AppSettings["ida:Issuer"];

            SignOutRequestMessage signoutRequestMessage = new SignOutRequestMessage(new Uri(wsFederationEndpoint), wtrealm);
            signoutRequestMessage.Parameters.Add("wtrealm", wtrealm);
            FederatedAuthentication.SessionAuthenticationModule.SignOut();

            string signoutUrl = signoutRequestMessage.WriteQueryString();
            return signoutUrl;
        }
    }
}
