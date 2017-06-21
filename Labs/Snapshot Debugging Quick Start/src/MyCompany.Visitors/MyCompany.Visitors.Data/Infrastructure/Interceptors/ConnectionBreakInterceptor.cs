
namespace MyCompany.Visitors.Data.Infrastructure.Interceptors
{
    using System.Data.Entity.Infrastructure.Interception;

    /// <summary>
    /// Connection breaker interceptor
    /// </summary>
    class ConnectionBreakInterceptor
        :IDbCommandInterceptor
    {
        /// <summary>
        /// Reader executing
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ReaderExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            //add a error in the command ( 40501:The service is currently busy ) to 
            //test if the execution strategy works correctly! If command logger is setted to console
            //you can view the retries produced for configured execution strategy
            command.CommandText = string.Format("{0};{1}", command.CommandText, "RAISERROR(40501,18,1)");
        }

        /// <summary>
        /// Non query executed
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void NonQueryExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        /// <summary>
        /// Non query executing
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void NonQueryExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        /// <summary>
        /// Reader executed
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ReaderExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
        }

       /// <summary>
       /// Scalar executed
       /// </summary>
       /// <param name="command"></param>
       /// <param name="interceptionContext"></param>
        public void ScalarExecuted(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        /// <summary>
        /// Scalar executing
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ScalarExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }
    }
}
