using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.Visitors.Model
{
    /// <summary>
    /// CRM Integration data
    /// </summary>
    public class CRMData
    {
        /// <summary>
        /// VisitorId
        /// </summary>
        public int VisitorId { get; set; }
        /// <summary>
        /// Number of CRMLeads
        /// </summary>
        public int CRMLeads { get; set; }
        /// <summary>
        /// Name of Account Manager
        /// </summary>
        public string CRMAccountManager { get; set; }
    }
}
