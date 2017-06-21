
namespace MyCompany.Visitors.Web
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IdentityModel.Tokens;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Web.Configuration;

    /// <summary>
    /// Custom Jwt Security Token Handler
    /// </summary>
    public class CustomJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        private string audience = WebConfigurationManager.AppSettings["ida:Audience"];
        private string validIssuerString = WebConfigurationManager.AppSettings["ida:Issuer"];

        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="validationParameters"></param>
        /// <returns></returns>
        public override ClaimsPrincipal ValidateToken(JwtSecurityToken jwt, TokenValidationParameters validationParameters)
        {
            // set up valid issuers
            if ((validationParameters.ValidIssuer == null) &&
                (validationParameters.ValidIssuers == null || !validationParameters.ValidIssuers.Any()))
            {
                validationParameters.ValidIssuers = new List<string> { validIssuerString };
            }
            // and signing token.
            if (validationParameters.SigningToken == null)
            {
                var resolver = (NamedKeyIssuerTokenResolver)this.Configuration.IssuerTokenResolver;
                if (resolver.SecurityKeys != null)
                {
                    IList<SecurityKey> skeys;
                    if (resolver.SecurityKeys.TryGetValue(audience, out skeys))
                    {
                        var tok = new NamedKeySecurityToken(audience, skeys);
                        validationParameters.SigningToken = tok;
                    }
                }
            }
            return base.ValidateToken(jwt, validationParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public override ClaimsPrincipal ValidateToken(JwtSecurityToken jwt)
        {
            var vparms = new TokenValidationParameters
            {
                AllowedAudiences = Configuration.AudienceRestriction.AllowedAudienceUris.Select(s => s.ToString())
            };
            return ValidateToken(jwt, vparms);
        }

    } 
}