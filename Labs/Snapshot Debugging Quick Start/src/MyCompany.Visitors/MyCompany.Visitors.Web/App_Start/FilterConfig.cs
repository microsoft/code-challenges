
namespace MyCompany.Visitors.Web
{
    using System.Web.Mvc;

    /// <summary>
    /// Filter Config
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Register Global Filters
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
        }
    }
}