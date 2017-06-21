namespace MyCompany.Visitors.Web.Controllers
{
    using MyCompany.Visitors.Data.Repositories;
    using MyCompany.Visitors.Model;
    using MyCompany.Visitors.Web.Infraestructure.Security;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Visits Controller
    /// </summary>
    [RoutePrefix("api/visitors")]
    [MyCompanyAuthorization]
    public class VisitorsController : ApiController
    {
        private readonly IVisitorRepository _visitorRepository = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="visitorRepository">IVisitorRepository dependency</param>
        public VisitorsController(IVisitorRepository visitorRepository)
        {
            if (visitorRepository == null)
                throw new ArgumentNullException("visitorRepository");

            _visitorRepository = visitorRepository;
        }

        /// <summary>
        /// Get visitor by Id
        /// </summary>
        /// <param name="visitorId"></param>
        /// <param name="pictureType">PictureType</param>
        /// <returns>Visitor</returns>
        [WebApiOutputCacheAttribute()]
        [Route("{visitorId:int:min(1)}/{pictureType:int:range(1,2)}")]
        [Route("~/noauth/api/visitors/{visitorId:int:min(1)}/{pictureType:int:range(1,2)}")]
        public async Task<Visitor> Get(int visitorId, PictureType pictureType)
        {
            return await _visitorRepository.GetCompleteInfoAsync(visitorId, pictureType);
        }

        /// <summary>
        /// Get Visitors
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pictureType">PictureType</param>
        /// <param name="pageSize">Size of page</param>
        /// <param name="pageCount">Size Count</param>
        /// <returns>List of visitors</returns>
        [WebApiOutputCacheAttribute()]
        public async Task<IEnumerable<Visitor>> GetVisitors(string filter, PictureType pictureType, int pageSize, int pageCount)
        {
            return await _visitorRepository.GetVisitorsAsync(filter, pictureType, pageSize, pageCount);
        }

        /// <summary>
        /// Get Visitor Count
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <returns>Number of visitors</returns>
        [WebApiOutputCacheAttribute()]
        [Route("count")]
        [Route("~/noauth/api/visitors/count")]
        public async Task<int> GetCount(string filter)
        {
            return await _visitorRepository.GetCountAsync(filter);
        }

        /// <summary>
        /// Add new visitor.
        /// </summary>
        /// <param name="visitor">visitor information</param>
        /// <returns>visitorId</returns>
        [HttpPost]
        [WebApiOutputCacheAttribute(false, true)]
        public async Task<int> Add(Visitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return await _visitorRepository.AddAsync(visitor);
        }

        /// <summary>
        /// Update visitor
        /// </summary>
        /// <param name="visitor">visitor information</param>
        [HttpPut]
        [WebApiOutputCacheAttribute(false, true)]
        public async Task Update(Visitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            await _visitorRepository.UpdateAsync(visitor);
        }

        /// <summary>
        /// Delete visitor
        /// </summary>
        /// <param name="visitorId">visitorId</param>
        [WebApiOutputCacheAttribute(false, true)]
        [Route("{visitorId:int:min(1)}")]
        [Route("~/noauth/api/visitors/{visitorId:int:min(1)}")]
        [HttpDelete]
        public async Task Delete(int visitorId)
        {
            await _visitorRepository.DeleteAsync(visitorId);
        }
    }
}