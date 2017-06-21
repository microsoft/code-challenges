namespace MyCompany.Visitors.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MyCompany.Visitors.Model;
    using System.Data.Entity;
    using System.Threading.Tasks;

    /// <summary>
    /// The visit repository implementation
    /// </summary>
    public class VisitRepository
        : IVisitRepository
    {
        private readonly MyCompanyContext _context;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">the context dependency</param>
        public VisitRepository(MyCompanyContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            _context = context;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="visitId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<Visit> GetAsync(int visitId)
        {
            return await _context.Visits
                .FindAsync(visitId);
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<IEnumerable<Visit>> GetAllAsync()
        {
            return await _context.Visits
                .ToListAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="visitId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<Visit> GetCompleteInfoAsync(int visitId, PictureType pictureType)
        {
            var result = await _context.Visits
                    .Where(q => q.VisitId == visitId)
                    .Select(v => new
                    {
                        Visit = v,
                        Visitor = v.Visitor,
                        VisitorImages = v.Visitor.VisitorPictures.Where(vp => vp.PictureType == pictureType || pictureType == PictureType.All),
                        Employee = v.Employee,
                        EmployeePictures = v.Employee.EmployeePictures.Where(ep => ep.PictureType == pictureType || pictureType == PictureType.All)
                    })
                    .ToListAsync();

            return result.Select(v => BuildVisit(v.Visit))
                .FirstOrDefault();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pageSize"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pageCount"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="dateFilter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="toDate"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<IEnumerable<Visit>> GetVisitsAsync(string filter, PictureType pictureType, int pageSize, int pageCount, DateTime? dateFilter, DateTime? toDate)
        {
            DateTime? maxDate = null;

            if (dateFilter != null)
            {
                maxDate = toDate ?? dateFilter.Value.Date.AddDays(1);
                dateFilter = DateTime.SpecifyKind(dateFilter.Value, DateTimeKind.Utc);
                maxDate = DateTime.SpecifyKind(maxDate.Value, DateTimeKind.Utc);
            }

            var results = await _context.Visits
                .Where(q => String.IsNullOrEmpty(filter) || q.Comments.Contains(filter) || (q.Visitor.FirstName + " " + q.Visitor.LastName).Contains(filter) || (q.Visitor.Email.Contains(filter)))
                .Where(q => dateFilter == null || q.VisitDateTime >= dateFilter && q.VisitDateTime < maxDate)
                .Select(v => new
                {
                    Visit = v,
                    Visitor = v.Visitor,
                    VisitorImages = v.Visitor.VisitorPictures.Where(vp => vp.PictureType == pictureType || pictureType == PictureType.All),
                    Employee = v.Employee,
                    EmployeePictures = v.Employee.EmployeePictures.Where(ep => ep.PictureType == pictureType || pictureType == PictureType.All)

                })
                .OrderBy(v => v.Visit.VisitDateTime)
                .Skip(pageSize * pageCount)
                .Take(pageSize)
                .ToListAsync();

            var x = results.Select(v => BuildVisit(v.Visit)).ToList();
            return x;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pageSize"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pageCount"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="dateFilter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns></returns>
        public async Task<IEnumerable<Visit>> GetVisitsFromDateAsync(string filter, PictureType pictureType, int pageSize, int pageCount, DateTime dateFilter)
        {
            dateFilter = DateTime.SpecifyKind(dateFilter, DateTimeKind.Utc);

            var results = await _context.Visits
                .Where(q => String.IsNullOrEmpty(filter) || q.Comments.Contains(filter) || (q.Visitor.FirstName + " " + q.Visitor.LastName).Contains(filter) || (q.Visitor.Email.Contains(filter)))
                .Where(q => q.VisitDateTime >= dateFilter)
                .Select(v => new
                {
                    Visit = v,
                    Visitor = v.Visitor,
                    VisitorImages = v.Visitor.VisitorPictures.Where(vp => vp.PictureType == pictureType),
                    Employee = v.Employee,
                    EmployeePictures = v.Employee.EmployeePictures.Where(ep => ep.PictureType == pictureType)

                })
                .OrderBy(v => v.Visit.VisitDateTime)
                .Skip(pageSize * pageCount)
                .Take(pageSize)
                .ToListAsync();

            return results.Select(v => BuildVisit(v.Visit));
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="employeeEmail"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pictureType"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pageSize"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="pageCount"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<IEnumerable<Visit>> GetUserVisitsAsync(string employeeEmail, string filter, PictureType pictureType, int pageSize, int pageCount)
        {
            var dateFilter = DateTime.UtcNow.AddHours(-1);

            var result = await _context.Visits
                    .Where(q => q.Employee.Email == employeeEmail
                    &&
                    (String.IsNullOrEmpty(filter) || q.Comments.Contains(filter) || (q.Visitor.FirstName + " " + q.Visitor.LastName).Contains(filter))
                    &&
                    q.VisitDateTime >= dateFilter
                    )
                    .OrderBy(q => q.VisitDateTime)
                    .Select(v => new
                    {
                        Visit = v,
                        Visitor = v.Visitor,
                        VisitorImages = v.Visitor.VisitorPictures.Where(vp => vp.PictureType == pictureType),
                        Employee = v.Employee,
                        EmployeePictures = v.Employee.EmployeePictures.Where(ep => ep.PictureType == pictureType)

                    })
                    .Skip(pageSize * pageCount)
                    .Take(pageSize)
                    .ToListAsync();
                    

            return result.Select(v => BuildVisit(v.Visit));
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="dateFilter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="toDate"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<int> GetCountAsync(string filter, DateTime? dateFilter, DateTime? toDate)
        {            
            DateTime? maxDate = null;
            if (dateFilter != null)
            {
                maxDate = toDate ?? dateFilter.Value.Date.AddDays(1);
                dateFilter = DateTime.SpecifyKind(dateFilter.Value, DateTimeKind.Utc);
                maxDate = DateTime.SpecifyKind(maxDate.Value, DateTimeKind.Utc);

            }

            return await _context.Visits
                .Where(q => String.IsNullOrEmpty(filter) || q.Comments.Contains(filter) || (q.Visitor.FirstName + " " + q.Visitor.LastName).Contains(filter))
                .Where(q => dateFilter == null || q.VisitDateTime >= dateFilter && q.VisitDateTime < maxDate)
                .OrderBy(q => q.VisitDateTime)
                .CountAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="dateFilter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<int> GetCountFromDateAsync(string filter, DateTime dateFilter)
        {
            dateFilter = DateTime.SpecifyKind(dateFilter, DateTimeKind.Utc);

            return await _context.Visits
                .Where(q => String.IsNullOrEmpty(filter) || q.Comments.Contains(filter) || (q.Visitor.FirstName + " " + q.Visitor.LastName).Contains(filter))
                .Where(q => q.VisitDateTime >= dateFilter)
                .CountAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="employeeIdentity"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <param name="filter"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<int> GetUserCountAsync(string employeeIdentity, string filter)
        {
            var dateFilter = DateTime.UtcNow.AddHours(-1);

            return await _context.Visits
                .Where(q =>
                    q.Employee.Email == employeeIdentity
                    &&
                    (String.IsNullOrEmpty(filter) || q.Comments.Contains(filter) || (q.Visitor.FirstName + " " + q.Visitor.LastName).Contains(filter)))
                .Where(q => q.VisitDateTime >= dateFilter)
                .OrderBy(q => q.VisitDateTime)
                .CountAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="visit"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        /// <returns><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></returns>
        public async Task<int> AddAsync(Visit visit)
        {
            visit.CreatedDateTime = DateTime.UtcNow;
            visit.Status = VisitStatus.Pending;

            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();

            return visit.VisitId;
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="visit"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        public async Task UpdateAsync(Visit visit)
        {
            var visittoUpdate = await _context.Visits.FirstOrDefaultAsync(q => q.VisitId == visit.VisitId);

            _context.Entry<Visit>(visittoUpdate)
                .CurrentValues
                .SetValues(visit);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// <see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/>
        /// </summary>
        /// <param name="visitId"><see cref="MyCompany.Visitors.Data.Repositories.IVisitRepository"/></param>
        public async Task DeleteAsync(int visitId)
        {
            var visit = await _context.Visits.FirstOrDefaultAsync(q => q.VisitId == visitId);
            if (visit != null)
            {
                _context.Visits.Remove(visit);

                await _context.SaveChangesAsync();
            }
        }

        Visit BuildVisit(Visit visit)
        {
            var created = new Visit()
            {
                VisitId = visit.VisitId,
                CreatedDateTime = visit.CreatedDateTime,
                VisitDateTime = visit.VisitDateTime,
                Comments = visit.Comments,
                EmployeeId = visit.EmployeeId,
                HasCar = visit.HasCar,
                Plate = visit.Plate,
                Status = visit.Status,
                VisitorId = visit.VisitorId,
                Visitor = new Visitor()
                {
                    VisitorId = visit.Visitor.VisitorId,
                    FirstName = visit.Visitor.FirstName,
                    LastName = visit.Visitor.LastName,
                    Company = visit.Visitor.Company,
                    Email = visit.Visitor.Email,
                    CreatedDateTime = visit.CreatedDateTime,
                    LastModifiedDateTime = visit.Visitor.LastModifiedDateTime,
                    VisitorPictures = (visit.Visitor.VisitorPictures != null) ?
                            visit.Visitor.VisitorPictures
                            .Select(vp => new VisitorPicture()
                            {
                                PictureType = vp.PictureType,
                                VisitorPictureId = vp.VisitorPictureId,
                                VisitorId = vp.VisitorId,
                                Content = vp.Content
                            }).ToList() : null
                },
                Employee = new Employee()
                {
                    EmployeeId = visit.Employee.EmployeeId,
                    FirstName = visit.Employee.FirstName,
                    LastName = visit.Employee.LastName,
                    JobTitle = visit.Employee.JobTitle,
                    Email = visit.Employee.Email,
                    TeamId = visit.Employee.TeamId,
                    EmployeePictures = (visit.Employee.EmployeePictures != null) ? 
                            visit.Employee.EmployeePictures
                            .Select(ep =>  new EmployeePicture()
                            {
                                Employee = null,
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
