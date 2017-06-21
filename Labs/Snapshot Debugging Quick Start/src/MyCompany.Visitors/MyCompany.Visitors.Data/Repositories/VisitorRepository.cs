namespace MyCompany.Visitors.Data.Repositories
{
    using MyCompany.Visitors.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The visitor repository implementation
    /// </summary>
    public class VisitorRepository : IVisitorRepository
    {
        private readonly MyCompanyContext _context;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>
        public VisitorRepository(MyCompanyContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            _context = context;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <param name="visitorId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></returns>
        public async Task<Visitor> GetAsync(int visitorId)
        {
            return await _context.Visitors
                .FindAsync(visitorId);
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></returns>
        public async Task<IEnumerable<Visitor>> GetAllAsync()
        {
            return await _context.Visitors
                .ToListAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <param name="visitorId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></returns>
        public async Task<Visitor> GetCompleteInfoAsync(int visitorId, PictureType pictureType)
        {
            var result = await _context.Visitors
                .Where(q => q.VisitorId == visitorId)
                .Select(v => new
                {
                    Visitor = v,
                    VisitorPictures = v.VisitorPictures.Where(vp => vp.PictureType == pictureType),
                    Visits = v.Visits,
                    VisitsEmployee = v.Visits.Select(ve => ve.Employee),
                    VisitsEmployeePictures = v.Visits.Select(ve => ve.Employee).SelectMany(vee => vee.EmployeePictures.Where(vpe => vpe.PictureType == pictureType))
                })
                .ToListAsync();

            return result.Select(v => BuildVisitor(v.Visitor)).FirstOrDefault();;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <param name="pageSize"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <param name="pageCount"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></returns>
        public async Task<IEnumerable<Visitor>> GetVisitorsAsync(string filter, PictureType pictureType, int pageSize, int pageCount)
        {
            var results = await _context.Visitors
                .Where(q =>
                    String.IsNullOrEmpty(filter) ||
                    (q.FirstName + " " + q.LastName).Contains(filter) ||
                    q.Company.Contains(filter) ||
                    q.Email.Contains(filter))
                .Select(v => new
                {
                    Visitor = v,
                    VisitorPictures = v.VisitorPictures.Where(vp => vp.PictureType == pictureType),
                    Visits = v.Visits,
                    VisitsEmployee = v.Visits.Select(ve => ve.Employee),
                    VisitsEmployeePictures = v.Visits.Select(ve => ve.Employee).SelectMany(vee => vee.EmployeePictures.Where(vpe => vpe.PictureType == pictureType))
                })
                .OrderBy(v => v.Visitor.FirstName)
                .Skip(pageSize * pageCount)
                .Take(pageSize)
                .ToListAsync();

            return results.Select(v => BuildVisitor(v.Visitor));
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></returns>
        public async Task<int> GetCountAsync(string filter)
        {
            return await _context.Visitors.CountAsync(q => String.IsNullOrEmpty(filter) ||
                                                q.FirstName.Contains(filter) ||
                                                q.LastName.Contains(filter) ||
                                                (q.FirstName + " " + q.LastName).Contains(filter) ||
                                                q.Company.Contains(filter));
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <param name="visitor"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></returns>
        public async Task<int> AddAsync(Visitor visitor)
        {
            visitor.CreatedDateTime = DateTime.UtcNow;
            visitor.LastModifiedDateTime = DateTime.UtcNow;

            _context.Visitors.Add(visitor);

            await _context.SaveChangesAsync();

            return visitor.VisitorId;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <param name="visitor"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        public async Task UpdateAsync(Visitor visitor)
        {
            visitor.LastModifiedDateTime = DateTime.UtcNow;

            _context.Entry<Visitor>(visitor)
                .State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/>
        /// </summary>
        /// <param name="visitorId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitorRepository"/></param>
        public async Task DeleteAsync(int visitorId)
        {
            var visitor = await _context.Visitors
                .FindAsync(visitorId);

            if (visitor != null)
            {
                _context.Visitors
                    .Remove(visitor);

                await _context.SaveChangesAsync();
            }
        }

        private Visitor BuildVisitor(Visitor visitor)
        {
            //the idea is remove reference for improve 
            //client work without $ref

            if (!visitor.Email.Contains("@"))  //for quickstart
            {
                throw new InvalidOperationException("Email validation failed");
            }

            var created = new Visitor
            {
                VisitorId = visitor.VisitorId,
                FirstName = visitor.FirstName,
                LastName = visitor.LastName,
                Company = visitor.Company,
                Email = visitor.Email,
                CreatedDateTime = visitor.CreatedDateTime,
                LastModifiedDateTime = visitor.LastModifiedDateTime,
                PersonalId = visitor.PersonalId,
                Position = visitor.Position,
                VisitorPictures = (visitor.VisitorPictures != null) ? visitor.VisitorPictures.Select(vp => new VisitorPicture()
                {
                    VisitorPictureId = vp.VisitorPictureId,
                    Content = vp.Content,
                    PictureType = vp.PictureType,
                    VisitorId = vp.VisitorId
                }).ToList()
                : null,
            };

            if (visitor.Visits != null && visitor.Visits.Any())
            {
                created.LastVisit = visitor.Visits.Where(x => x.VisitDateTime < DateTime.UtcNow)
                     .OrderByDescending(x => x.VisitDateTime)
                     .Select(BuildVisit)
                     .FirstOrDefault();

                created.Visits = visitor.Visits.Where(x => x.VisitDateTime >= DateTime.UtcNow)
                    .OrderBy(x => x.VisitDateTime)
                    .Take(4)
                    .Select(BuildVisit)
                    .ToList();
            }

            return created;
        }

        private Visit BuildVisit(Visit visit)
        {
            //the idea is remove reference for improve 
            //client work without $ref
            var created = new Visit
            {
                VisitId = visit.VisitId,
                VisitDateTime = visit.VisitDateTime,
                Employee = new Employee()
                {
                    FirstName = visit.Employee.FirstName,
                    LastName = visit.Employee.LastName,
                    JobTitle = visit.Employee.JobTitle,
                    Email = visit.Employee.Email,
                    EmployeePictures = (visit.Employee.EmployeePictures != null) ? visit.Employee.EmployeePictures.Select(ep => new EmployeePicture()
                    {
                        PictureType = ep.PictureType,
                        EmployeePictureId = ep.EmployeePictureId,
                        EmployeeId = ep.EmployeeId,
                        Content = ep.Content
                    }).ToList() : null
                }
            };

            return created;
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
