namespace MyCompany.Visitors.Data.Repositories
{
    using MyCompany.Visitors.Model;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The employee picture repository implementation
    /// </summary>
    public class EmployeePictureRepository : IEmployeePictureRepository
    {
        private readonly MyCompanyContext _context;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>
        public EmployeePictureRepository(MyCompanyContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            _context = context;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeePictureRepository"/>
        /// </summary>
        /// <param name="employeePicture"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeePictureRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IEmployeePictureRepository"/></returns>
        public async Task<int> AddAsync(EmployeePicture employeePicture)
        {
            if (employeePicture == null) 
                throw new ArgumentNullException("employeePicture");

            _context.EmployeePictures.Add(employeePicture);
            await _context.SaveChangesAsync();

            return employeePicture.EmployeePictureId;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeePictureRepository"/>
        /// </summary>
        /// <param name="employeePicture"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeePictureRepository"/></param>
        public async Task UpdateAsync(EmployeePicture employeePicture)
        {
            if (employeePicture == null) throw new ArgumentNullException("employeePicture");

            _context.Entry<EmployeePicture>(employeePicture)
                .State = System.Data.Entity.EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IEmployeePictureRepository"/>
        /// </summary>
        /// <param name="employeePictureId"><see cref="MyCompany.Visitors.Data.Repositories.IEmployeePictureRepository"/></param>
        public async Task DeleteAsync(int employeePictureId)
        {
            var picture = _context.EmployeePictures
                .Find(employeePictureId);

            if (picture != null)
            {
                _context.EmployeePictures
                    .Remove(picture);

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
