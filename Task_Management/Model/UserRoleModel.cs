using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class UserRoleModel
    {
        [JsonPropertyName("userroleId")]
        public int? userroleId { get; set; }

        [JsonPropertyName("userId")]
        public int? userId { get; set; }

        [JsonPropertyName("roleId")]
        public int? roleId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime createdAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedAt")]
        public DateTime updatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("userName")]
        public string? userName { get; set; }

        [JsonPropertyName("roleName")]
        public string? roleName { get; set; }
    }
}
