using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class ProjectModel
    {
        [JsonPropertyName("projectId")]
        public int? projectId { get; set; }

        [JsonPropertyName("projectName")]
        public string? projectName { get; set; }

        [JsonPropertyName("projectDescription")]
        public string? projectDescription { get; set; }

        [JsonPropertyName("startDate")]
        public string? startDate { get; set; }

        [JsonPropertyName("endDate")]
        public string? endDate { get; set; }

        [JsonPropertyName("createdBy")]
        public int? createdBy { get; set; }

        [JsonPropertyName("updatedBy")]
        public int? updatedBy { get; set; }

        [JsonPropertyName("projectStatus")]
        public bool? projectStatus { get; set; } 

        [JsonPropertyName("createdAt")]
        public string? createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? updatedAt { get; set; }
        public bool? updateFlag { get; set; }
        public string? createdByUserName { get; set; }
        public string? updateByUserName { get; set; }



    }
}
