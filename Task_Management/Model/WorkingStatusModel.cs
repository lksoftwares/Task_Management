using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class WorkingStatusModel
    {
        [JsonPropertyName("txnId")]
        public int? txnId { get; set; }

        [JsonPropertyName("userId")]
        public int? userId { get; set; }

        [JsonPropertyName("workingDesc")]
        public string? workingDesc { get; set; }

        [JsonPropertyName("workingDate")]
        public DateTime workingDate { get; set; } = DateTime.Now;

        [JsonPropertyName("createdAt")]
        public DateTime? createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? updatedAt { get; set; } 

    }
}
