using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class UsersRoleModel
    {
        [JsonPropertyName("userId")]
        public int? userId { get; set; }
        [JsonPropertyName("roleId")]

        public int? roleId { get; set; }

        [JsonPropertyName("userName")]
        public string? userName { get; set; } 

        [JsonPropertyName("userEmail")]
        public string? userEmail { get; set; } 

        [JsonPropertyName("userPassword")]
        public string? userPassword { get; set; } 

        [JsonPropertyName("userStatus")]
        public bool? userStatus { get; set; } 

        [JsonPropertyName("createdAt")]
        public string? createdAt { get; set; } 

        [JsonPropertyName("updatedAt")]
        public string? updatedAt { get; set; }
        public bool? updateFlag { get; set; } = false;
        public string ? userRole { get; set; }
        public List<userRoleData> ? userRoles { get; set; }
        [JsonPropertyName("userRoleId")]
        public int? userRoleId { get; set; }

    }
    public class userRoleData
    {
        public int roleId { get; set; }
        public string roleName { get; set; }

        [JsonPropertyName("userRoleId")]
        public int? userRoleId { get; set; }

    }
}
