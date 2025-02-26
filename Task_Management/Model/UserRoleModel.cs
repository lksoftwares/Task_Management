using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class UserRoleModel
    {
        [JsonPropertyName("userRoleId")]
        public int? userRoleId { get; set; }

        [JsonPropertyName("userId")]
        public int? userId { get; set; }

        [JsonPropertyName("roleId")]
        public int? roleId { get; set; }

        [JsonPropertyName("createdAt")]
        public string? createdAt { get; set; } 

        [JsonPropertyName("updatedAt")]
        public string? updatedAt { get; set; } 

        [JsonPropertyName("userName")]
        public string? userName { get; set; }

        [JsonPropertyName("roleName")]
        public string? roleName { get; set; }
        public bool? updateFlag { get; set; }
    }
}
