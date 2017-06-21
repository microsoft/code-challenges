namespace MyCompany.Visitors.Web.Controllers
{
    using MyCompany.Visitors.Data.Repositories;
    using MyCompany.Visitors.Model;
    using MyCompany.Visitors.Web.Infraestructure.Security;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Employee Controller
    /// </summary>
    [RoutePrefix("api/employees")]
    [MyCompanyAuthorization]
    public class EmployeesController : ApiController
    {
        private readonly IEmployeeRepository _employeeRepository = null;
        private readonly ISecurityHelper _securityHelper = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="employeeRepository">IEmployeeRepository dependency</param>
        /// <param name="securityHelper">ISecurityHelper dependency</param>
        public EmployeesController(IEmployeeRepository employeeRepository, ISecurityHelper securityHelper)
        {
            if (employeeRepository == null)
                throw new ArgumentNullException("employeeRepository");

            if (securityHelper == null)
                throw new ArgumentNullException("securityHelper");

            _employeeRepository = employeeRepository;
            _securityHelper = securityHelper;
        }

        /// <summary>
        /// Get logged employee info
        /// </summary>
        /// <param name="pictureType">PictureType</param>
        /// <returns></returns>
        [WebApiOutputCacheAttribute(true)]
        [Route("current/{pictureType:int:range(1,2)}")]
        [Route("~/noauth/api/employees/current/{pictureType:int:range(1,2)}")]
        public async Task<Employee> GetLoggedEmployeeInfo(PictureType pictureType)
        {
            var employee = await _employeeRepository.GetByEmailAsync(_securityHelper.GetUser(), pictureType);
            return employee;
        }

        /// <summary>
        /// Get Employee function
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        /// <param name="pictureType">Picture Type</param>
        /// <returns></returns>
        [WebApiOutputCacheAttribute()]
        [Route("{employeeId:int:min(1)}/{pictureType:int:range(1,2)}")]
        [Route("~/noauth/api/employees/{employeeId:int:min(1)}/{pictureType:int:range(1,2)}")]
        public async Task<Employee> Get(int employeeId, PictureType pictureType)
        {
            return await _employeeRepository.GetCompleteInfoAsync(employeeId, pictureType);
        }

        /// <summary>
        /// Get Employees
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">Picture type</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageCount">Page count</param>
        /// <returns></returns>
        [WebApiOutputCacheAttribute()]
        [Route("GetEmployees")]
        [Route("~/noauth/api/employees/GetEmployees")]
        public async Task<IEnumerable<Employee>> GetEmployees(string filter, PictureType pictureType, int pageSize, int pageCount)
        {
            return await _employeeRepository.GetEmployeesAsync(filter, pictureType, pageSize, pageCount);
        }
    }
}
