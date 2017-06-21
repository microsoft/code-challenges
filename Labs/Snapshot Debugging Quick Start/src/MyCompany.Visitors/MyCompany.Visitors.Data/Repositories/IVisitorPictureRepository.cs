namespace MyCompany.Visitors.Data.Repositories
{
    using MyCompany.Visitors.Model;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Base contract for visitorPicture repository
    /// </summary>
    public interface IVisitorPictureRepository
        :IDisposable
    {
        /// <summary>
        /// Gets the specified visitor id.
        /// </summary>
        /// <param name="visitorId">The visitor id.</param>
        /// <param name="pictureType">Type of the picture.</param>
        /// <returns></returns>
        Task<VisitorPicture> GetAsync(int visitorId, PictureType pictureType);

        /// <summary>
        /// Add new visitorPicture
        /// </summary>
        /// <param name="visitorPicture">visitorPicture information</param>
        /// <returns>visitorPictureId</returns>
        Task<int> AddAsync(VisitorPicture visitorPicture);

        /// <summary>
        /// Update visitorPicture
        /// </summary>
        /// <param name="visitorPicture">visitorPicture information</param>
        Task UpdateAsync(VisitorPicture visitorPicture);

        /// <summary>
        /// Adds or update the visitorPicture.
        /// </summary>
        /// <param name="visitorId">The visitor id.</param>
        /// <param name="pictureType">Type of the picture.</param>
        /// <param name="content">The content.</param>
        Task AddOrUpdateAsync(int visitorId, PictureType pictureType, byte[] content);

        /// <summary>
        /// Delete visitorPicture
        /// </summary>
        /// <param name="visitorPictureId">visitorPicture to delete</param>
        Task DeleteAsync(int visitorPictureId);

    }
}
