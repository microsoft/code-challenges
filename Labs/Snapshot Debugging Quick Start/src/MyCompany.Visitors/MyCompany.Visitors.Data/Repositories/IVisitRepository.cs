namespace MyCompany.Visitors.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using MyCompany.Visitors.Model;
    using System.Threading.Tasks;

    /// <summary>
    /// Base contract for visits repository
    /// </summary>
    public interface IVisitRepository
        : IDisposable
    {
        /// <summary>
        /// Get visit by Id
        /// </summary>
        /// <param name="visitId"></param>
        /// <returns></returns>
        Task<Visit> GetAsync(int visitId);

        /// <summary>
        /// Get All visits
        /// </summary>
        /// <returns>List of visits</returns>
        Task<IEnumerable<Visit>> GetAllAsync();

        /// <summary>
        /// Get expense by Id
        /// </summary>
        /// <param name="visitId"></param>
        /// <param name="pictureType">PictureType</param>
        /// <returns></returns>
        Task<Visit> GetCompleteInfoAsync(int visitId, PictureType pictureType);

        /// <summary>
        /// Get Visits.
        /// If dateFilter is populated, only gets the visits from this day
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <param name="dateFilter">Date filter</param>
        /// <param name="toDate">To date filter</param>
        /// <returns>List of visits</returns>
        Task<IEnumerable<Visit>> GetVisitsAsync(string filter, PictureType pictureType, int pageSize, int pageCount, DateTime? dateFilter, DateTime? toDate);

        /// <summary>
        /// Get Visits from Date
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <param name="dateFilter">Date filter</param>
        /// <returns>List of visits</returns>
        Task<IEnumerable<Visit>> GetVisitsFromDateAsync(string filter, PictureType pictureType, int pageSize, int pageCount, DateTime dateFilter);

        /// <summary>
        /// Get Visits
        /// </summary>
        /// <param name="employeeEmail">employee Identity</param>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <returns>List of visits</returns>
        Task<IEnumerable<Visit>> GetUserVisitsAsync(string employeeEmail, string filter, PictureType pictureType, int pageSize, int pageCount);

        /// <summary>
        /// Get Visit Count
        /// </summary>
        /// <param name="dateFilter">Date Filter</param>
        /// <param name="filter">Filter</param>
        /// <param name="toDate">to date filter</param>
        /// <returns>Number of visits</returns>
        Task<int> GetCountAsync(string filter, DateTime? dateFilter, DateTime? toDate);

        /// <summary>
        /// Get Visit Count from Date
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="dateFilter">Date Filter</param>
        /// <returns>Number of visits from a Date</returns>
        Task<int> GetCountFromDateAsync(string filter, DateTime dateFilter);

        /// <summary>
        /// Get Visit Count
        /// </summary>
        /// <param name="employeeIdentity">employee Identity</param>
        /// <param name="filter">Filter</param>
        /// <returns>Number of visits</returns>
        Task<int> GetUserCountAsync(string employeeIdentity, string filter);

        /// <summary>
        /// Add new visit
        /// </summary>
        /// <param name="visit">visit information</param>
        /// <returns>visitId</returns>
        Task<int> AddAsync(Visit visit);

        /// <summary>
        /// Update visit
        /// </summary>
        /// <param name="visit">visit information</param>
        Task UpdateAsync(Visit visit);

        /// <summary>
        /// Delete visit
        /// </summary>
        /// <param name="visitId">visit to delete</param>
        Task DeleteAsync(int visitId);


    }
}
