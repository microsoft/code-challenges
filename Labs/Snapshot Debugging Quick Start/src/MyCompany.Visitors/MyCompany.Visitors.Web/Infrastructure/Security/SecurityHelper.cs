namespace MyCompany.Visitors.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IdentityModel.Metadata;
    using System.IdentityModel.Tokens;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.ServiceModel.Security;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;
    using System.Xml;

    /// <summary>
    /// Security Helper
    /// </summary>
    public class SecurityHelper : ISecurityHelper
    {
        /// <summary>
        /// <see cref="MyCompany.Visitors.Web.ISecurityHelper"/>
        /// </summary>
        /// <returns><see cref="MyCompany.Visitors.Web.ISecurityHelper"/></returns>
        public string GetUser()
        {
            // Test Mode is only for demos without internet, not for real apps!
            if (RequestIsNoAuthRoute())
            {
                return System.Configuration.ConfigurationManager.AppSettings["testModeIdentity"];
            }

            var principal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            if (!principal.Identity.IsAuthenticated)
                return string.Empty;

            var nameClaim = principal.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == ClaimTypes.Upn);

            if (nameClaim != null)
                return nameClaim.Value;

            return string.Empty;
        }

        /// <summary>
        /// Web Apps have route called "NoAuth" that is not securized. This route is used for demos without internet access
        /// In the request come from this source, the delegation handler doesn´t validate the header token
        /// </summary>
        /// <returns></returns>
        public static bool RequestIsNoAuthRoute()
        {
               if (HttpContext.Current != null
                        && (RequestIsNoAuth() || UrlReferrerIsNoAuth()))
                    return true;

            return false;
        }
        
       
        /// <summary>
        /// This function retrieves ACS token (in format of OAuth 2.0 Bearer Token type) from 
        /// the Authorization header in the incoming HTTP request from the ShipperClient.
        /// </summary>
        /// <param name="authzHeader"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TryRetrieveToken(string authzHeader, out string token)
        {
            token = authzHeader;

            if(string.IsNullOrEmpty(authzHeader))
            {
                return false;
            }

            // Remove the bearer token scheme prefix and return the rest as ACS token 
            token = token.StartsWith("Bearer ") ? token.Substring(7) : token;
            token = token.StartsWith("Authorization Bearer ") ? token.Substring(21) : token;
            return true;
        }

        private static bool RequestIsNoAuth()
        {
            if (HttpContext.Current != null
                &&
                HttpContext.Current.Request != null
                &&
                HttpContext.Current.Request.Url != null
                &&
                HttpContext.Current.Request.Url.ToString().ToLowerInvariant().Contains("/noauth/api"))
            {
                return true;
            }

            return false;
        }

        private static bool UrlReferrerIsNoAuth()
        {
            if (HttpContext.Current != null
                &&
                HttpContext.Current.Request != null
                &&
                HttpContext.Current.Request.UrlReferrer != null
                &&
                HttpContext.Current.Request.UrlReferrer.AbsoluteUri.ToLowerInvariant().Contains("/noauth"))
            {
                return true;
            }

            return true; //set this to enable noauth by default
        }
    }
}