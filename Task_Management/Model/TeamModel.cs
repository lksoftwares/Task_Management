using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class TeamModel
    {
        [JsonPropertyName("teamId")]
        public int? teamId { get; set; }

        [JsonPropertyName("teamName")]
        public string? teamName { get; set; }

        [JsonPropertyName("tmDescription")]
        public string? tmDescription { get; set; }


        [JsonPropertyName("tmId")]
        public int? tmId { get; set; }

        [JsonPropertyName("userId")]
        public int? userId { get; set; }

        [JsonPropertyName("roleId")]
        public int? roleId { get; set; }

        [JsonPropertyName("tmstatus")]
        public bool? tmstatus { get; set; } 

        [JsonPropertyName("assignedAt")]
        public string? assignedAt { get; set; }

        [JsonPropertyName("createdAt")]
        public string? createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? updatedAt { get; set; }

        public bool? updateFlag { get; set; }
        [JsonPropertyName("roleName")]

        public string ? roleName { get; set; }
        [JsonPropertyName("userName")]

        public string? userName { get; set; }

    }
}
