namespace MyCompany.Visitors.Data.Repositories
{
    using System.Collections.Generic;
    using MyCompany.Visitors.Model;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Base contract for visitor repository
    /// </summary>
    public interface IVisitorRepository
        : IDisposable
    {
        /// <summary>
        /// Get visitor by Id
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        Task<Visitor> GetAsync(int visitorId);

        /// <summary>
        /// Get All visitors
        /// </summary>
        /// <returns>List of visitors</returns>
        Task<IEnumerable<Visitor>> GetAllAsync();


        /// <summary>
        /// Get visitor by Id
        /// </summary>
        /// <param name="visitorId"></param>
        /// <param name="pictureType">PictureType</param>
        /// <returns>Visitor</returns>
        Task<Visitor> GetCompleteInfoAsync(int visitorId, PictureType pictureType);

        /// <summary>
        /// Get Visitors
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <returns>List of visitors</returns>
        Task<IEnumerable<Visitor>> GetVisitorsAsync(string filter, PictureType pictureType, int pageSize, int pageCount);

        /// <summary>
        /// Get Visitor Count
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>Number of visitors</returns>
        Task<int> GetCountAsync(string filter);


        /// <summary>
        /// Add new visitor
        /// </summary>
        /// <param name="visitor">visitor information</param>
        /// <returns>visitorId</returns>
        Task<int> AddAsync(Visitor visitor);

        /// <summary>
        /// Update visitor
        /// </summary>
        /// <param name="visitor">visitor information</param>
        Task UpdateAsync(Visitor visitor);

        /// <summary>
        /// Delete visitor
        /// </summary>
        /// <param name="visitorId">visitor to delete</param>
        Task DeleteAsync(int visitorId);
    }
}
