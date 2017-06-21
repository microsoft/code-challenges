
namespace MyCompany.Visitors.Web
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// IdentityConfig
    /// For more information on ASP.NET Identity, visit http://go.microsoft.com/fwlink/?LinkId=301863
    /// </summary>
    public static class IdentityConfig 
    {
        /// <summary>
        /// ConfigureIdentity
        /// </summary>
        public static void ConfigureIdentity() 
        {
            RefreshValidationSettings();
        }

        /// <summary>
        /// RefreshValidationSettings
        /// </summary>
        public static void RefreshValidationSettings()
        {
            string metadataLocation = ConfigurationManager.AppSettings["ida:FederationMetadataLocation"];
            SingleTenantIssuerNameRegistry.RefreshKeys(metadataLocation);
        }
    }
}