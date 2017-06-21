namespace MyCompany.Visitors.Data.Infrastructure
{
    using MyCompany.Visitors.Data.Infrastructure.Initializers;
    using MyCompany.Visitors.Data.Infrastructure.Interceptors;
    using System.Data.Entity;
    using System.Data.Entity.SqlServer;

    /// <summary>
    /// My Company Db configuration
    /// </summary>
    public class MyCompanyDbConfiguration
        : DbConfiguration
    {
        /// <summary>
        /// My Company Db configuration constuctor
        /// </summary>
        public MyCompanyDbConfiguration()
        {
            //set the seed initializer
            SetDatabaseInitializer<MyCompanyContext>(new MyCompanyContextInitializer());

            //Set Sql Azure Strategy ( check common erros in azure,cluster with connection lost and retry operations )
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());

            //Un-comment next line to test execution strategy!
            //Interceptor(new ConnectionBreakInterceptor());
        }
    }
}
