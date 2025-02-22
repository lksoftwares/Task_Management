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
        public DateTime? createdAt { get; set; } 

        [JsonPropertyName("updatedAt")]
        public DateTime? updatedAt { get; set; } 
    }
}
