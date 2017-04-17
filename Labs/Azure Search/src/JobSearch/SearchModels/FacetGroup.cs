using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace JobSearch.SearchModels
{
    public class FacetGroup
    {
        public string FacetName { get; set; }
        public string FacetDisplayName { get; set; }
        public IList<FacetSelection> FacetValues { get; set; }
    }
}
