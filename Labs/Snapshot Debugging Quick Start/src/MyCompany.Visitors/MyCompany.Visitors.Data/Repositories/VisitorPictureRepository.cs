namespace MyCompany.Visitors.Data.Repositories
{
    using MyCompany.Visitors.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The visitorPicture repository implementation
    /// </summary>
    public class VisitorPictureRepository : IVisitorPictureRepository
    {
        private readonly MyCompanyContext _context;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>
        public VisitorPictureRepository(MyCompanyContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            _context = context;
        }


        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/>
        /// </summary>
        /// <param name="visitorId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/></returns>
        public Task<VisitorPicture> GetAsync(int visitorId, PictureType pictureType)
        {
            return _context.VisitorPictures
                .FirstOrDefaultAsync(q => q.VisitorId == visitorId && q.PictureType == pictureType);
        }


        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/>
        /// </summary>
        /// <param name="visitorPicture"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/></returns>
        public async Task<int> AddAsync(VisitorPicture visitorPicture)
        {
            if (visitorPicture == null) throw new ArgumentNullException("visitorPicture");

            _context.VisitorPictures
                .Add(visitorPicture);

            await _context.SaveChangesAsync();

            return visitorPicture.VisitorPictureId;

        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/>
        /// </summary>
        /// <param name="visitorPicture"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/></param>
        public async Task UpdateAsync(VisitorPicture visitorPicture)
        {
            if (visitorPicture == null) throw new ArgumentNullException("visitorPicture");

            _context.Entry<VisitorPicture>(visitorPicture)
                .State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository" />
        /// </summary>
        /// <param name="visitorId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository" /></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository" /></param>
        /// <param name="content"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository" /></param>
        public async Task AddOrUpdateAsync(int visitorId, PictureType pictureType, byte[] content)
        {
            var visitorPicturetoUpdate = await _context.VisitorPictures
                .FirstOrDefaultAsync(q => q.VisitorId == visitorId && q.PictureType == pictureType);

            if (visitorPicturetoUpdate != null)
            {
                visitorPicturetoUpdate.Content = content;
            }
            else
            {
                _context.VisitorPictures.Add(new VisitorPicture()
                {
                    VisitorId = visitorId,
                    PictureType = pictureType,
                    Content = content
                });
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/>
        /// </summary>
        /// <param name="visitorPictureId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorPictureRepository"/></param>
        public async Task DeleteAsync(int visitorPictureId)
        {
            var visitorPicture = await _context.VisitorPictures
                .FindAsync(visitorPictureId);

            if (visitorPicture != null)
            {
                _context.VisitorPictures
                    .Remove(visitorPicture);

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
