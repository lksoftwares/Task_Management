using LkDataConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using Task_Management.Model;

namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]

    public class AudioCheckController : ControllerBase
    {
        private SqlConnection _connection;


        Connection _Connection = new Connection();
        ApiResponse Resp = new ApiResponse();
        DataAccess _dataAccess = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();
        [HttpPost]
        [Route("AddAudio")]
        public async Task<IActionResult> AddEditTeamMember([FromForm] audiomodel audio)
        {
            try
            {

                if (audio.AudioFile != null && audio.AudioFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await audio.AudioFile.CopyToAsync(ms);
                       audio.AudioData= ms.ToArray();

                        //  Console.WriteLine($"jkhi: {Convert.ToBase64String(fileBytes)}");

                        //  audio.AudioData.SqlDbType.VarBinary).Value = filesbyte;
                        //string newstr = Convert.ToBase64String(filesbyte);
                    }
                }
                string connectionString = "Server=localhost;Database=Task_Management;User Id=sa;Password=1; max pool size = 20000000;TrustServerCertificate=True;Connect Timeout=30000;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = $"INSERT INTO Audio_Mst (AudioName, AudioData) VALUES ('{audio.AudioName}', @AudioData)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@AudioData", SqlDbType.VarBinary).Value = audio.AudioData;

                        int result = await command.ExecuteNonQueryAsync();

                        if (result > 0)
                        {
                            return Ok(new { message = "Audio Added Successfully", isSuccess = true });
                        }
                        else
                        {
                            return StatusCode(500, new { message = "Failed to insert audio", isSuccess = false });
                        }
                    }
                }


                //_query = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                //{
                //    entity = audio,
                //    tableName = "Audio_Mst",

                //});



                //if (_query.QueryErrorMessage != null)
                //{
                //    Resp.message = _query.QueryErrorMessage;
                //}
                //else
                //{
                //    Resp.message = $"Audio Added Successfully";
                //}


                //Resp.statusCode = StatusCodes.Status200OK;

                //Resp.isSuccess = true;

                //return StatusCode(StatusCodes.Status200OK, Resp);

            }
            catch (Exception ex)
            {

                Resp.statusCode = StatusCodes.Status500InternalServerError;
                Resp.message = ex.Message;


                return StatusCode(StatusCodes.Status500InternalServerError, Resp);
            }
        }
        [HttpGet]
        [Route("GetAudio/{id}")]
        public async Task<IActionResult> GetAudio(int id)
        {
            try
            {
                string connectionString = "Server=localhost;Database=Task_Management;User Id=sa;Password=1; max pool size = 20000000;TrustServerCertificate=True;Connect Timeout=30000;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT AudioName, AudioData FROM Audio_Mst WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                string audioName = reader.GetString(0);
                                byte[] audioData = (byte[])reader["AudioData"];

                                return File(audioData, "audio/mpeg", audioName);
                            }
                        }
                    }
                }

                return NotFound(new { message = "Audio not found", isSuccess = false });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, isSuccess = false });
            }
        }

    }
    public class audiomodel
    {
        public int? Id { get; set; }
        public string? AudioName { get; set; }
        public byte[]? AudioData { get; set; }
        public IFormFile? AudioFile { get; set; }
    }
}
