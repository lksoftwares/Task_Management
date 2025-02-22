﻿using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class UsersModel
    {
        [JsonPropertyName("userId")]
        public int? userId { get; set; }

        [JsonPropertyName("userName")]
        public string? userName { get; set; } 

        [JsonPropertyName("userEmail")]
        public string? userEmail { get; set; } 

        [JsonPropertyName("userPassword")]
        public string? userPassword { get; set; } 

        [JsonPropertyName("userStatus")]
        public bool userStatus { get; set; } = true;

        [JsonPropertyName("createdAt")]
        public DateTime? createdAt { get; set; } 

        [JsonPropertyName("updatedAt")]
        public DateTime? updatedAt { get; set; } 

    }
}
