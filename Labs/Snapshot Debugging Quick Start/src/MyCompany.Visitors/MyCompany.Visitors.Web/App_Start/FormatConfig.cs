
namespace MyCompany.Visitors.Web
{
    using System.Web.Http;

    /// <summary>
    /// Configure 
    /// </summary>
    public class FormatConfig
    {
        /// <summary>
        /// The following code restricts the web API responses to JSON formatter
        /// </summary>
        /// <param name="config">HttpConfiguration</param>
        public static void ConfigureFormats(HttpConfiguration config)
        {
            var json = config.Formatters.JsonFormatter;

            // Preserve object references in JSON
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            
            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Write indented JSON
            json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        }
    }
}