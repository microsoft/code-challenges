using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabWeb.Models
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

    }
}
