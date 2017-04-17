using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace JobSearch.SearchModels
{
    [SerializePropertyNamesAsCamelCase]
    public class JobResult
    {
        public Guid Id { get; set; }
        [JsonProperty("job_id")]
        public double? JobId { get; set; }
        public string Agency { get; set; }
        [JsonProperty("posting_type")]
        public string PostingType { get; set; }
        [JsonProperty("num_of_positions")]
        public string NumOfPositions { get; set; }
        [JsonProperty("business_title")]
        public string BusinessTitle { get; set; }
        [JsonProperty("civil_service_title")]
        public string CivilServiceTitle { get; set; }
        [JsonProperty("title_code_no")]
        public string TitleCodeNo { get; set; }
        public string Level { get; set; }
        [JsonProperty("salary_range_from")]
        public int SalaryRangeFrom { get; set; }
        [JsonProperty("salary_range_to")]
        public int SalaryRangeTo { get; set; }
        [JsonProperty("salary_frequency")]
        public string SalaryFrequency { get; set; }
        [JsonProperty("work_location")]
        public string WorkLocation { get; set; }
        [JsonProperty("division_work_unit")]
        public string DivisionWorkUnit { get; set; }
        [JsonProperty("job_description")]
        public string JobDescription { get; set; }
        [JsonProperty("minimum_qual_requirements")]
        public string MinimumQualRequirements { get; set; }
        [JsonProperty("preferred_skills")]
        public string PreferredSkills { get; set; }
        [JsonProperty("additional_information")]
        public string AdditionalInformation { get; set; }
        [JsonProperty("to_apply")]
        public string ToApply { get; set; }
        [JsonProperty("hours_per_shift")]
        public string HoursPerShift { get; set; }
        [JsonProperty("recruitment_contact")]
        public string RecruitmentContact { get; set; }
        [JsonProperty("residency_requirement")]
        public string ResidencyRequirement { get; set; }
        [JsonProperty("posting_date")]
        public DateTime? PostingDate { get; set; }
        [JsonProperty("post_until")]
        public DateTime? PostUntil { get; set; }
        [JsonProperty("posting_updated")]
        public DateTime? PostingUpdated { get; set; }
        [JsonProperty("process_date")]
        public DateTime? ProcessDate { get; set; }
        [JsonProperty("geo_location")]
        public GeographyPoint GeoLocation { get; set; }
    }
}
