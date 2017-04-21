using System.Collections.Generic;

namespace Microsoft.CodeChallenges.AzureSearch.Lab.SearchModels
{
    public class FacetGroup
    {
        public string FacetName { get; set; }
        public string FacetDisplayName { get; set; }
        public IList<FacetSelection> FacetValues { get; set; }
    }
}
