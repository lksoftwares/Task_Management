using LkDataConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using Task_Management.Model;
using System.Web;
using System.Net;
using System.Net.Sockets;


namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]

    public class AudioCheckController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private SqlConnection _connection;


        Connection _Connection = new Connection();
        ApiResponse Resp = new ApiResponse();
        DataAccess _dataAccess = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();
        public AudioCheckController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;

        }
        [HttpPost]
        [Route("AddAudio")]
        public async Task<IActionResult> AddAudio([FromForm] audiomodel audio)
        {
            try
            {

                if (audio.AudioFile != null && audio.AudioFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await audio.AudioFile.CopyToAsync(ms);
                        audio.AudioData = ms.ToArray();

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



        [HttpPost]
        [Route("SaveAudio")]
        public async Task<IActionResult> SaveAudio([FromForm] audiomodel audio)
        {
            try
            {

                //if (!Directory.Exists("D:\\vs2022Repos\\Task_Management\\Task_Management\\savefiles"))
                //    Directory.CreateDirectory("D:\\vs2022Repos\\Task_Management\\Task_Management\\savefiles");

                //string fileName = Guid.NewGuid().ToString() + Path.GetExtension(audio.AudioFile.FileName);
                //string filePath = Path.Combine("D:\\vs2022Repos\\Task_Management\\Task_Management\\savefiles\\", fileName);

                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    await audio.AudioFile.CopyToAsync(stream);
                //}
                string savedFileName = DataAccess.SaveImage("D:\\vs2022Repos\\Task_Management\\Task_Management\\savefiles\\", audio.AudioFile);


                var audioData = new audiomodel
                {
                    AudioName = audio.AudioName,
                    AudioPath = savedFileName
                };

                var queryResult = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                {
                    entity = audioData,
                    tableName = "Audio_Mst1",
                    imgFolderpath = "D:\\vs2022Repos\\Task_Management\\Task_Management\\savefiles\\"
                });



                if (_query.QueryErrorMessage != null)
                {
                    Resp.message = _query.QueryErrorMessage;
                }
                else
                {
                    Resp.message = $"Audio Added Successfully";
                }


                Resp.statusCode = StatusCodes.Status200OK;

                Resp.isSuccess = true;

                return StatusCode(StatusCodes.Status200OK, Resp);

            }
            catch (Exception ex)
            {

                Resp.statusCode = StatusCodes.Status500InternalServerError;
                Resp.message = ex.Message;


                return StatusCode(StatusCodes.Status500InternalServerError, Resp);
            }
        }
        [HttpGet]
        [Route("getip")]
        public IActionResult GetIP()
        {
            string ip = Dns.GetHostEntry(Dns.GetHostName())
                   .AddressList
                   .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                   .ToString();

            return Ok($"IP Address is: {ip}");
            //string hostName = Dns.GetHostName();
            //Console.WriteLine("Host Name: " + hostName);

            //var ipAddresses = Dns.GetHostEntry(hostName).AddressList;

            //string ip = ipAddresses
            //            .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //            .FirstOrDefault()?.ToString();

            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = ipAddresses
            //            .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            //            .FirstOrDefault()?.ToString();
            //}

            //Console.WriteLine("IP Address is: " + ip);
            //return Ok("IP Address is: " + ip);
        }

        //[HttpGet]
        //[Route("getip")]
        //public IActionResult GetIP()
        //{
        //    string hostName = Dns.GetHostName();
        //    Console.WriteLine(hostName);

        //    // Get the IP from GetHostByName method of dns class. 
        //    string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
        //    Console.WriteLine("IP Address is : " + IP);
        //    return Ok("IP Address is : " + IP);
        //    // var clientIp = HttpContext.Connection.RemoteIpAddress;
        //    //var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        //    //if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        //    //{
        //    //    var forwardedIp = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
        //    //    var ipList = forwardedIp.Split(',');
        //    //    remoteIpAddress = ipList[0]; 
        //    //}
        //    // string userRequest = System.Web.HttpContext.Current.Request.UserHostAddress;

        //    //   var ip = httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].ToString();
        //    //  string ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
        //    //  ?? HttpContext.Connection.RemoteIpAddress?.ToString();
        //    //   var hostName = Dns.GetHostName();
        //    //var ip =   Dns.GetHostByName(hostName).AddressList[0].ToString();
        //    //   return Ok(new { ip = ip});

        //    //return Ok(remoteIpAddress);


        //}




    }
    public class audiomodel
    {
        public int? Id { get; set; }
        public string? AudioName { get; set; }
        public byte[]? AudioData { get; set; }
        public IFormFile? AudioFile { get; set; }
        public string? AudioPath { get; set; }
    }
}
