namespace MyCompany.Visitors.Web.Controllers
{
    using MyCompany.Visitors.Data.Repositories;
    using MyCompany.Visitors.Model;
    using MyCompany.Visitors.Web.Hubs;
    using MyCompany.Visitors.Web.Infraestructure.Security;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Visits Controller
    /// </summary>
    [RoutePrefix("api/visits")]
    [MyCompanyAuthorization]
    public class VisitsController : ApiController
    {
        private readonly IVisitRepository _visitRepository = null;
        private readonly ISecurityHelper _securityHelper = null;
        private readonly IEmployeeRepository _employeeRepository = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="visitRepository">IVisitRepository dependency</param>
        /// <param name="securityHelper">ISecurityHelper dependency</param>
        /// <param name="employeeRepository">IEmployeeRepository dependency</param>
        public VisitsController(IVisitRepository visitRepository, ISecurityHelper securityHelper, IEmployeeRepository employeeRepository)
        {
            if (visitRepository == null)
                throw new ArgumentNullException("visitRepository");

            if (securityHelper == null)
                throw new ArgumentNullException("securityHelper");

            if (employeeRepository == null)
                throw new ArgumentNullException("employeeRepository");

            _visitRepository = visitRepository;
            _securityHelper = securityHelper;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Get expense by Id
        /// </summary>
        /// <param name="visitId"></param>
        /// <param name="pictureType">PictureType</param>
        /// <returns></returns>
        [WebApiOutputCacheAttribute()]
        [Route("{visitId:int:min(1)}/{pictureType:int:range(1,3)}")]
        [Route("~/noauth/api/visits/{visitId:int:min(1)}/{pictureType:int:range(1,3)}")]
        public async Task<Visit> Get(int visitId, PictureType pictureType)
        {
            return await _visitRepository.GetCompleteInfoAsync(visitId, pictureType);
        }

        /// <summary>
        /// Get Visits
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <param name="dateFilter">Date filter</param>
        /// <param name="toDate">To date filter</param>
        /// <returns>List of visits</returns>
        [WebApiOutputCacheAttribute()]
        [Route("company")]
        [Route("~/noauth/api/visits/company")]
        public async Task<IEnumerable<Visit>> GetVisits(string filter, PictureType pictureType, int pageSize, int pageCount, DateTimeOffset? dateFilter, DateTimeOffset? toDate)
        {
            DateTime? fromDateTime = dateFilter.HasValue?dateFilter.Value.DateTime: default(DateTime?);
            DateTime? toDateTime = toDate.HasValue ? toDate.Value.DateTime : default(DateTime?);

            return await _visitRepository.GetVisitsAsync(filter, pictureType, pageSize, pageCount, fromDateTime, toDateTime);
        }

        /// <summary>
        /// Get Visits
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <param name="dateFilter">Date filter</param>
        /// <returns>List of visits</returns>
        [WebApiOutputCacheAttribute()]
        [Route("company/fromdate")]
        [Route("~/noauth/api/visits/company/fromdate")]
        public async Task<IEnumerable<Visit>> GetVisitsFromDate(string filter, PictureType pictureType, int pageSize, int pageCount, DateTimeOffset dateFilter)
        {
            return await _visitRepository.GetVisitsFromDateAsync(filter, pictureType, pageSize, pageCount, dateFilter.DateTime);
        }

        /// <summary>
        /// Get Visits
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <param name="dateFilter">Date to filter</param>
        /// <returns>List of visits</returns>
        [WebApiOutputCacheAttribute()]
        [Route("user")]
        [Route("~/noauth/api/visits/user")]
        public async Task<IEnumerable<Visit>> GetUserVisits(string filter, PictureType pictureType, int pageSize, int pageCount, DateTime dateFilter)
        {
            var identity = _securityHelper.GetUser();
            return await _visitRepository.GetUserVisitsAsync(identity, filter, pictureType, pageSize, pageCount);
        }

        /// <summary>
        /// Get Visit Count
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="dateFilter">DateFilter</param>
        /// <param name="toDate">To date filter</param>
        /// <returns>Number of visits</returns>
        [WebApiOutputCacheAttribute()]
        [Route("company/count")]
        [Route("~/noauth/api/visits/company/count")]
        public async Task<int> GetCount(string filter, DateTime? dateFilter, DateTime? toDate)
        {
            return await _visitRepository.GetCountAsync(filter, dateFilter, toDate);
        }

        /// <summary>
        /// Get Visit Count from Date
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="dateFilter">DateFilter</param>
        /// <returns>Number of visits from a Date</returns>
        [WebApiOutputCacheAttribute()]
        [Route("company/count/fromdate")]
        [Route("~/noauth/api/visits/company/count/fromdate")]
        public async Task<int> GetCountFromDate(string filter, DateTime dateFilter)
        {
            return await _visitRepository.GetCountFromDateAsync(filter, dateFilter);
        }

        /// <summary>
        /// Get Visit Count
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="dateFilter">Date to filter</param>
        /// <returns>Number of visits</returns>
        [WebApiOutputCacheAttribute(true)]
        [Route("user/count")]
        [Route("~/noauth/api/visits/user/count")]
        public async Task<int> GetUserCount(string filter, DateTime dateFilter)
        {
            var identity = _securityHelper.GetUser();
            return await _visitRepository.GetUserCountAsync(identity, filter);
        }

        /// <summary>
        /// Add new  visit
        /// </summary>
        /// <param name="visit">visit information</param>
        /// <returns>visitId</returns>
        [WebApiOutputCacheAttribute(false, true)]
        public async Task<int> Add(Visit visit)
        {
            if (visit == null)
                throw new ArgumentNullException("visit");

            var visitId = await _visitRepository.AddAsync(visit);
            var addedVisit = await _visitRepository.GetCompleteInfoAsync(visitId, PictureType.All);
            VisitorsNotificationHub.NotifyVisitAdded(addedVisit);
            return visitId;
        }

        /// <summary>
        /// Update visit
        /// </summary>
        /// <param name="visit">visit information</param>
        [HttpPut]
        [WebApiOutputCacheAttribute(false, true)]
        public async Task Update(Visit visit)
        {
            if (visit == null)
                throw new ArgumentNullException("visit");

            await _visitRepository.UpdateAsync(visit);
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="visitId">Id to update</param>
        /// <param name="status">VisitStatus</param>
        [Route("{visitId}/update/{status}")]
        [Route("~/noauth/api/visits/{visitId}/update/{status}")]
        [HttpPut]
        [WebApiOutputCacheAttribute(false, true)]
        public async Task UpdateStatus(int visitId, VisitStatus status)
        {
            var visit = await _visitRepository.GetCompleteInfoAsync(visitId, PictureType.Small);
            if (visit != null)
            {
                visit.Status = status;
                await _visitRepository.UpdateAsync(visit);

                var employee = await _employeeRepository.GetCompleteInfoAsync(visit.EmployeeId, PictureType.Small);
                if (employee != null && !String.IsNullOrWhiteSpace(employee.Email) && status == VisitStatus.Arrived)
                    VisitorsNotificationHub.NotifyVisitArrived(visit, employee.Email);
            }
        }

        /// <summary>
        /// Delete visit
        /// </summary>
        /// <param name="visitId">visitId</param>
        [WebApiOutputCacheAttribute(false, true)]
        [Route("{visitId:int:min(1)}")]
        [Route("~/noauth/api/visits/{visitId:int:min(1)}")]
        [HttpDelete]
        public async Task Delete(int visitId)
        {
            await _visitRepository.DeleteAsync(visitId);
        }
    }
}
