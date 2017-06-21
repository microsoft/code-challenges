namespace MyCompany.Visitors.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Caching;
    using System.Web.Configuration;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using MyCompany.Common.CrossCutting;

    /// <summary>
    /// Action Filter to cache responses
    /// </summary>
    public class WebApiOutputCacheAttribute : ActionFilterAttribute
    {
        // cache length in seconds
        private int _timespan;
        // true to enable cache
        private bool _cacheEnabled = false;
        // true if the cache depends on the caller user
        private readonly bool _dependsOnIdentity;
        // cache repository
        private static readonly ObjectCache WebApiCache = MemoryCache.Default;
        private readonly SecurityHelper _securityHelper;
        private readonly bool _invalidateCache;

        /// <summary>
        /// Constructor
        /// </summary>
        public WebApiOutputCacheAttribute() : this(true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependsOnIdentity"></param>
        public WebApiOutputCacheAttribute(bool dependsOnIdentity) : this(dependsOnIdentity, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependsOnIdentity"></param>
        /// <param name="invalidateCache">true to invalidate cache object</param>
        public WebApiOutputCacheAttribute(bool dependsOnIdentity, bool invalidateCache)
        {
            _securityHelper = new SecurityHelper();
            _dependsOnIdentity = dependsOnIdentity;
            _invalidateCache = invalidateCache;

            ReadConfig();
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (_cacheEnabled)
            {
                if (filterContext != null)
                {
                    if (IsCacheable(filterContext))
                    {
                        string _cachekey = string.Join(":", new string[] 
                        { 
                            filterContext.Request.RequestUri.OriginalString,
                            filterContext.Request.Headers.Accept.FirstOrDefault().ToString(), 
                        });

                        if (_dependsOnIdentity)
                            _cachekey = _cachekey.Insert(0, _securityHelper.GetUser());

                        if (WebApiCache.Contains(_cachekey))
                        {
                            TraceManager.TraceInfo(String.Format("Cache contains key: {0}", _cachekey));

                            var val = (string)WebApiCache.Get(_cachekey);
                            if (val != null)
                            {
                                filterContext.Response = filterContext.Request.CreateResponse();
                                filterContext.Response.Content = new StringContent(val);
                                var contenttype = (MediaTypeHeaderValue)WebApiCache.Get(_cachekey + ":response-ct");
                                if (contenttype == null)
                                    contenttype = new MediaTypeHeaderValue(_cachekey.Split(':')[1]);
                                filterContext.Response.Content.Headers.ContentType = contenttype;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentNullException("actionContext");
                }
            }
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            try
            {
                if (_cacheEnabled)
                {
                    if (WebApiCache != null)
                    {
                        string _cachekey = string.Join(":", new string[] 
                        { 
                            filterContext.Request.RequestUri.OriginalString,
                            filterContext.Request.Headers.Accept.FirstOrDefault().ToString(), 
                        });

                        if (_dependsOnIdentity)
                            _cachekey = _cachekey.Insert(0, _securityHelper.GetUser());

                        if (filterContext.Response != null && filterContext.Response.Content != null)
                        {
                            string body = filterContext.Response.Content.ReadAsStringAsync().Result;

                            if (WebApiCache.Contains(_cachekey))
                            {
                                WebApiCache.Set(_cachekey, body, DateTime.Now.AddSeconds(_timespan));
                                WebApiCache.Set(_cachekey + ":response-ct", filterContext.Response.Content.Headers.ContentType, DateTime.Now.AddSeconds(_timespan));
                            }
                            else
                            {
                                WebApiCache.Add(_cachekey, body, DateTime.Now.AddSeconds(_timespan));
                                WebApiCache.Add(_cachekey + ":response-ct", filterContext.Response.Content.Headers.ContentType, DateTime.Now.AddSeconds(_timespan));
                            }
                        }
                    }
                }

                if (_invalidateCache)
                {
                    CleanCache();
                }
            }
            catch (Exception ex)
            {
                TraceManager.TraceError(ex);
            }
        }

        /// <summary>
        /// Removes all items from the cache
        /// </summary>
        private static void CleanCache()
        {
            if (WebApiCache != null)
            {
                List<string> keyList = WebApiCache.Select(w => w.Key).ToList();
                foreach (string key in keyList)
                {
                    WebApiCache.Remove(key);
                }
            }
        }

        private void ReadConfig()
        {
            if (!Boolean.TryParse(WebConfigurationManager.AppSettings["CacheEnabled"], out _cacheEnabled))
                _cacheEnabled = false;

            if (!Int32.TryParse(WebConfigurationManager.AppSettings["CacheTimespan"], out _timespan))
                _timespan = 1800; // seconds
        }

        private bool IsCacheable(HttpActionContext ac)
        {
            if (_timespan > 0)
            {
                if (ac.Request.Method == HttpMethod.Get)
                    return true;
            }
            else
            {
                throw new InvalidOperationException("Wrong Arguments");
            }
            return false;
        }
    }
}