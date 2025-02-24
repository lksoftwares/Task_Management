using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class RolesModel
    {
        [JsonPropertyName("roleId")]
        public int? roleId { get; set; }

        [JsonPropertyName("roleName")]
        public string? roleName { get; set; } 

        [JsonPropertyName("roleStatus")]
        public bool roleStatus { get; set; } = true;

        [JsonPropertyName("createdAt")]
        public string? createdAt { get; set; } 

        [JsonPropertyName("updatedAt")]
        public string? updatedAt { get; set; }
    }
}
