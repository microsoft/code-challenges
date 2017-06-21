
namespace MyCompany.Visitors.Data.Repositories
{
    using System.Collections.Generic;
    using MyCompany.Visitors.Model;
    using System.Threading.Tasks;
    using System;

    /// <summary>
    /// Base contract for employee repository
    /// </summary>
    public interface IEmployeeRepository
         : IDisposable
    {
        /// <summary>
        /// Get employee by Id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<Employee> GetAsync(int employeeId);

        /// <summary>
        /// Get employee by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pictureType"></param>
        /// <returns></returns>
        Task<Employee> GetByEmailAsync(string email, PictureType pictureType);

        /// <summary>
        /// Get All employees
        /// </summary>
        /// <returns>List of employees</returns>
        Task<IEnumerable<Employee>> GetAllAsync();

        /// <summary>
        /// Get Employees
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pictureType"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        Task<IEnumerable<Employee>> GetEmployeesAsync(string filter, PictureType pictureType, int pageSize, int pageCount);

        /// <summary>
        /// Get Employee by Id and Picture Type
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        /// <param name="pictureType">Picture Type</param>
        /// <returns></returns>
        Task<Employee> GetCompleteInfoAsync(int employeeId, PictureType pictureType);

        /// <summary>
        /// Add new employee
        /// </summary>
        /// <param name="employee">employee information</param>
        /// <returns>commentId</returns>
        Task<int> AddAsync(Employee employee);

        /// <summary>
        /// Update employee
        /// </summary>
        /// <param name="expense">employee information</param>
        Task UpdateAsync(Employee expense);

        /// <summary>
        /// Delete employee
        /// </summary>
        /// <param name="employeeId">employee to delete</param>
        Task DeleteAsync(int employeeId);
    }
}
