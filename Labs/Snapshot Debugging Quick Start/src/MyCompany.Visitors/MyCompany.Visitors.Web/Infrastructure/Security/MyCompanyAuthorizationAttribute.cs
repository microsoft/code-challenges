

namespace MyCompany.Visitors.Web.Infraestructure.Security
{
    using System.Security.Claims;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    /// <summary>
    /// Validate if the request has the right provileges
    /// </summary>
    public class MyCompanyAuthorizationAttribute 
        : AuthorizeAttribute
    {
        /// <summary>
        /// Check if the request is authorized
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = (ClaimsPrincipal)Thread.CurrentPrincipal;

            if (principal.Identity.IsAuthenticated 
                || SecurityHelper.RequestIsNoAuthRoute()) // NoAuth routes are only for demo porpouses
                return true;

            return false;
        }
    }
}