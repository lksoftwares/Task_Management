using LkDataConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Task_Management.Classes;
using Task_Management.Model;

namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        ApiResponse Resp = new ApiResponse();
        Validation validation = new Validation();
        EncryptDecrypt EncryptPassword = new EncryptDecrypt();
        Connection _Connection = new Connection();
        private ConnectionClass _connection;
        DataAccess _dc = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();
        public UserController(ConnectionClass connection)
        {
            _connection = connection;
            Connection.ConnectionStr = _connection.GetSqlConnection().ConnectionString;
            Connection.Connect();

        }
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                string query = $"select * from User_Mst ORDER BY userName ASC";
               
                
                var result = _Connection.bindmethod(query);
                DataTable Table = result._DataTable;
                if (Table == null)
                {
                    Resp.statusCode = StatusCodes.Status200OK;
                    Resp.message = $"No User Found ";
                    Resp.isSuccess = true;

                    return Ok(Resp);

                }


                var RoleList = new List<UsersModel>();
                foreach (DataRow row in Table.Rows)
                {
                    RoleList.Add(new UsersModel
                    {
                        userId = Convert.ToInt32(row["userId"]),
                        userName = row["userName"].ToString(),
                        userPassword = row["userPassword"].ToString(),
                        userStatus = Convert.ToBoolean(row["userStatus"]),
                        createdAt = Convert.ToDateTime(row["createdAt"]),
                        updatedAt = Convert.ToDateTime(row["updatedAt"])


                    }); ;
                }



                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"User fetched successfully ";
                Resp.apiResponse = RoleList;
                Resp.isSuccess = true;

                return Ok(Resp);
            }
            catch (Exception ex)
            {

                Resp.statusCode = StatusCodes.Status500InternalServerError;
                Resp.message = ex.Message;

                return StatusCode(StatusCodes.Status500InternalServerError, Resp);

            }

        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UsersModel user)
        {
            try
            {
                string hashedPassword = EncryptPassword.Encrypt("ABC", user.userPassword);

                string query = $@"
            SELECT U.*, R.roleId, R.roleName 
            FROM User_Mst U
            JOIN UserRole_Mst UR ON U.userId = UR.userId
            JOIN Role_Mst R ON UR.roleId = R.roleId
            WHERE U.userEmail = '{user.userEmail}' 
              AND U.userPassword = '{hashedPassword}' 
              AND R.roleId = '{user.roleId}'";

                var result = _Connection.bindmethod(query);
                DataTable table = result._DataTable;
                DataRow userData = table.Rows.Count > 0 ? table.Rows[0] : null;

                if (userData == null)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = "Invalid credentials or role mismatch.";
                    return StatusCode(StatusCodes.Status404NotFound, Resp);
                }

                if (!(bool)userData["userStatus"])
                {
                    Resp.statusCode = StatusCodes.Status403Forbidden;
                    Resp.message = "User is not active. Please contact the administrator.";
                    return StatusCode(StatusCodes.Status403Forbidden, Resp);
                }

                WebToken _web = new WebToken();
                UserDetails _userdetails = new UserDetails();
                List<KeyDetails> lst1 = new List<KeyDetails>
        {
            new KeyDetails { KeyName = "userId", KeyValue = userData["userId"].ToString() },
            new KeyDetails { KeyName = "userName", KeyValue = userData["userName"].ToString() },
            new KeyDetails { KeyName = "roleId", KeyValue = userData["roleId"].ToString() },
            new KeyDetails { KeyName = "roleName", KeyValue = userData["roleName"].ToString() }
        };

                _userdetails.ListKeydetails = lst1;

                string token = _web.GenerateToken(new WebTokenValidationParameters
                {
                    ValidIssuer = "http://localhost:5266/",
                    ValidAudience = "http://localhost:5266/",
                    IssuerSigningKey = "2Fsk5LBU5j1DrPldtFmLWeO8uZ8skUzwhe3ktVimUE8l=",
                }, new UserDetails
                {
                    ListKeydetails = _userdetails.ListKeydetails
                });

                Console.WriteLine($"Here is the token {token}");

                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "User logged in successfully";
                Resp.isSuccess = true;
                Resp.apiResponse = new
                {
                    token,
                    User_Id = userData["userId"],
                    User_Name = userData["userName"],
                    Role_Id = userData["roleId"],
                    Role_Name = userData["roleName"]
                };

                return StatusCode(StatusCodes.Status200OK, Resp);
            }
            catch (Exception ex)
            {
                Resp.statusCode = StatusCodes.Status500InternalServerError;
                Resp.message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, Resp);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromForm] UsersModel user)
        {

            if (user.userPassword != null)
            {

                string hashedPassword = EncryptPassword.Encrypt("ABC", user.userPassword);

                user.userPassword = hashedPassword;
            }
        ;
            try
            {
                insertupdateTestclass insertupdateTestclass = new insertupdateTestclass();

                if (validation.CheckNullValues(user.userName)|| validation.CheckNullValues(user.userEmail)|| validation.CheckNullValues(user.userPassword))
                {
                    Resp.statusCode = StatusCodes.Status204NoContent;
                    Resp.message = $"User Email ,Name,Password can't be blank ";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);

                }
                var duplicacyParameter = new CheckDuplicacyPerameter
                {
                    tableName = "User_mst",
                    fields = new[] { "userName","userEmail" },
                    values = new[] { user.userName,user.userEmail }
                };

                if (_dc.CheckDuplicate(duplicacyParameter))
                {
                    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    Resp.message = $"User already exists.";
                    Resp.dup = true;


                    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                }

                _query = insertupdateTestclass.InsertOrUpdateEntity(new InsertUpdatePerameters
                {
                    entity = user,
                    tableName = "user_Mst",

                });

                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "User Register successfully";
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


    }
}
