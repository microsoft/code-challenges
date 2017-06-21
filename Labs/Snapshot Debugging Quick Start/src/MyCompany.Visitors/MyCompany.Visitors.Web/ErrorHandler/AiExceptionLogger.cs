using Microsoft.ApplicationInsights;
using System.Web.Http.ExceptionHandling;

namespace MyCompany.Visitors.Web
{
    /// <summary>
    /// Web API exception logger. Logs unhandled exceptions to Application Insights.
    /// </summary>
    public class AiExceptionLogger : ExceptionLogger
    {
        /// <summary>
        /// Called by the WebApi framework when an unhandled exception is thrown from a controller
        /// method.
        /// </summary>
        /// <param name="context">The HTTP action context containing the exception.</param>
        public override void Log(ExceptionLoggerContext context)
        {
            if (context?.Exception != null)
            {
                var ai = new TelemetryClient();
                ai.TrackException(context.Exception);
            }
        
            base.Log(context);
        }
    }
}