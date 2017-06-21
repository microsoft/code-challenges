namespace MyCompany.Visitors.Data.Repositories
{
    using MyCompany.Visitors.Model;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Base contract for employee picture repository
    /// </summary>
    public interface IEmployeePictureRepository
         : IDisposable
    {
        /// <summary>
        /// Add new employee picture
        /// </summary>
        /// <param name="employeePicture">employee picture information</param>
        /// <returns>employeePictureId</returns>
        Task<int> AddAsync(EmployeePicture employeePicture);

        /// <summary>
        /// Update employee picture
        /// </summary>
        /// <param name="employeePicture">employee picture information</param>
        Task UpdateAsync(EmployeePicture employeePicture);

        /// <summary>
        /// Delete employee picture
        /// </summary>
        /// <param name="employeePictureId">employee picture to delete</param>
        Task DeleteAsync(int employeePictureId);
    }
}
