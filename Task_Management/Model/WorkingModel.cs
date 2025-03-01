using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class WorkingModel
    {
        [JsonPropertyName("txnId")]
        public int? txnId { get; set; }

        [JsonPropertyName("userId")]
        public int? userId { get; set; }

        [JsonPropertyName("workingDesc")]
        public string? workingDesc { get; set; }

        [JsonPropertyName("workingDate")]
        public string workingDate { get; set; } = DateTime.Now.ToString();

        [JsonPropertyName("createdAt")]
        public string? createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? updatedAt { get; set; }
        [JsonPropertyName("workingNote")]

        public string? workingNote { get; set; }
        [JsonPropertyName("userName")]
        public string ? userName { get; set; }
        public bool updateFlag { get; set; } = false;

    }
}
