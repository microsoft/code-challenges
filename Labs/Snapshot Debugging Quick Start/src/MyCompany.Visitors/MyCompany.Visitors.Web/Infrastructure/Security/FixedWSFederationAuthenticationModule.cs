namespace MyCompany.Visitors.Web.Infraestructure.Security
{
    using System.IdentityModel.Services;
    using System;

    /// <summary>
    /// Customize the redirecto to identity provider function
    /// </summary>
    public class FixedWSFederationAuthenticationModule : WSFederationAuthenticationModule
    {
        /// <summary>
        /// Redirects user to identity provider
        /// </summary>
        /// <param name="uniqueId">The WSFAM saves this value in the wctx parameter in the WS-Federation sign in request; however, the module does not use it when processing sign-in requests or sign-in responses. 
        /// You can set it to any value. It does not have to be unique.</param>
        /// <param name="returnUrl">The URL to which the module should return upon authentication.</param>
        /// <param name="persist">he WSFAM saves this value in the wctx parameter in the WS-Federation sign in request; however, the module does not use it when processing sign-in requests or sign-in responses. </param>
        public override void RedirectToIdentityProvider(string uniqueId, string returnUrl, bool persist)
        {
            if (!returnUrl.EndsWith("/"))
            {
                if (String.Compare(System.Web.HttpContext.Current.Request.Url.AbsoluteUri + "/", base.Realm, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    returnUrl += "/";
                }
            }
            base.RedirectToIdentityProvider(uniqueId, returnUrl, persist);
        }
    }
}