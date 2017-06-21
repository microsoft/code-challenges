namespace MyCompany.Visitors.Data.Repositories
{
    using MyCompany.Visitors.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The employee repository implementation
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly MyCompanyContext _context;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>
        public EmployeeRepository(MyCompanyContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <param name="employeeId"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></returns>
        public async Task<Employee> GetAsync(int employeeId)
        {
            return await _context.Employees.FindAsync(employeeId);
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <param name="email"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></returns>
        public async Task<Employee> GetByEmailAsync(string email, PictureType pictureType)
        {
            var result = await _context.Employees
                .Include(e => e.ManagedTeams)
                .Where(e => e.Email == email)
                .Select(e => new
                {
                    Employee = e,
                    Pictures = e.EmployeePictures.Where(ep => ep.PictureType == pictureType)
                })
                .FirstOrDefaultAsync();

            if (result != null)
            {
                return result.Employee;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></returns>
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <param name="pageSize"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <param name="pageCount"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></returns>
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(string filter, PictureType pictureType, int pageSize, int pageCount)
        {
            //get filtered and paged employees
            var filteredEmployees = await _context.Employees
                .Where(q =>
                        String.IsNullOrEmpty(filter) ||
                        q.FirstName.Contains(filter) ||
                        q.LastName.Contains(filter) ||
                        (q.FirstName + " " + q.LastName).Contains(filter))
                .OrderBy(q => q.FirstName)
                .Skip(pageSize * pageCount)
                .Take(pageSize)
                .Select(e => new
                {
                    Employee = e,
                    Pictures = e.EmployeePictures.Where(ep => ep.PictureType == pictureType)
                })
                .ToListAsync();

            return filteredEmployees
                .Select(e => e.Employee);
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <param name="employeeId"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <returns></returns>
        public async Task<Employee> GetCompleteInfoAsync(int employeeId, PictureType pictureType)
        {
            var result = await _context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new
                {
                    Employee = e,
                    Pictures = e.EmployeePictures.Where(ep => ep.PictureType == pictureType)
                }).SingleOrDefaultAsync();

            return result.Employee;
        }



        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <param name="employee"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></returns>
        public async Task<int> AddAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return employee.EmployeeId;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <param name="employee"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        public async Task UpdateAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");

            _context.Entry<Employee>(employee)
                .State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/>
        /// </summary>
        /// <param name="employeeId"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeeRepository"/></param>
        public async Task DeleteAsync(int employeeId)
        {
            var employee = _context.Employees
                .Find(employeeId);

            if (employee != null)
            {
                _context.Employees
                    .Remove(employee);

                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Dispose all resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose all resource
        /// </summary>
        /// <param name="disposing">Dispose managed resources check</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
