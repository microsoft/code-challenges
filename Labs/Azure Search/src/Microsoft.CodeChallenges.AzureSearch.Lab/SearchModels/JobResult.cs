using System;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace Microsoft.CodeChallenges.AzureSearch.Lab.SearchModels
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

        [JsonIgnore]
        public string[] HighlighWords { get; set; }
        [JsonIgnore]
        public HighlightControl BusinessTitleDisplay
        {
            get
            {
                return new HighlightControl(BusinessTitle, HighlighWords);
            }
        }
        [JsonIgnore]
        public HighlightControl AgencyDisplay
        {
            get
            {
                return new HighlightControl(Agency, HighlighWords);
            }
        }
        [JsonIgnore]
        public HighlightControl DivisionWorkUnitDisplay
        {
            get
            {
                return new HighlightControl(DivisionWorkUnit, HighlighWords);
            }
        }
        [JsonIgnore]
        public HighlightControl JobDescriptionDisplay
        {
            get
            {
                return new HighlightControl(JobDescription, HighlighWords);
            }
        }
    }

    public class HighlightControl
    {
        public string[] HighlightWords { get; set; }
        public string Value { get; set; }

        public HighlightControl(string value, string[] highlighWords)
        {
            HighlightWords = highlighWords;
            Value = value;
        }
    }
}
