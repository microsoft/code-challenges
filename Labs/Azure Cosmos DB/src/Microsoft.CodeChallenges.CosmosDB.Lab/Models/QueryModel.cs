using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.CodeChallenges.CosmosDB.Lab.Models
{
    public class QueryModel
    {
        [Required]
        [Display(Name = "Query")]
        public string Query { get; set; }

        [Required]
        [Display(Name = "Documents")]
        public ICollection<string> Documents { get; set; }

        [Required]
        [Display(Name = "Count")]
        public int Count { get; set; }

        [Required]
        [Display(Name = "Error")]
        public string Error { get; set; }

        [Display(Name = "StatusCode")]
        public int StatusCode { get; set; }

        [Display(Name = "ResponseTime")]
        public long ResponseTime { get; set; }

    }
}
