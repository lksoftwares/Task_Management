using System.Text.Json.Serialization;

namespace Task_Management.Model
{
    public class ResponseModel
    {

        public int statusCode { get; set; }

        public string message { get; set; }

        public object apiResponse { get; set; }

        public bool isSuccess { get; set; } = false;
        public bool dup { get; set; } = false;
    }
}
