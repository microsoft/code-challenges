
namespace MyCompany.Visitors.Web
{
    using System.Web.Http.Filters;
    using MyCompany.Common.CrossCutting;

    /// <summary>
    /// Exception Handling
    /// </summary>
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// On Exception
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            TraceManager.TraceError(context.Exception);

            base.OnException(context);
        }
    }
}

    
