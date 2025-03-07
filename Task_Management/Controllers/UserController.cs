﻿using LkDataConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Sockets;
using System.Net;
using Task_Management.Classes;
using Task_Management.Model;

namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;


        }

        ApiResponse Resp = new ApiResponse();
      //  Validation validation = new Validation();
        EncryptDecrypt EncryptDecryptPassword = new EncryptDecrypt();
        Connection _Connection = new Connection();
        DataAccess _dc = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();

        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {


                string query = $"SELECT    usr.*,    r.roleId,    r.roleName FROM    User_Mst usr LEFT JOIN     User_Role_Mst ur ON usr.userId = ur.userId LEFT JOIN    Role_Mst r ON ur.roleId = r.roleId ORDER BY    usr.userName ASC";

                var result = _Connection.bindmethod(query);
                DataTable Table = result._DataTable;
                if (Table == null)
                {
                    Resp.statusCode = StatusCodes.Status200OK;
                    Resp.message = $"No User Found ";
                    Resp.isSuccess = true;

                    return Ok(Resp);

                }


                var UserList = new List<UsersRoleModel>();
                foreach (DataRow row in Table.Rows)
                {
                    int userId = Convert.ToInt32(row["userId"]);
                    var existingUser = UserList.FirstOrDefault(u => u.userId == userId);
                    if (existingUser == null)
                    {
                        existingUser = new UsersRoleModel
                        {
                            userId = userId,
                            userName = row["userName"].ToString(),
                            userPassword = EncryptDecryptPassword.Decrypt("ABC",
                        row["userPassword"].ToString()),
                            userEmail = row["userEmail"].ToString(),

                            userStatus = Convert.ToBoolean(row["userStatus"]),
                            createdAt = Convert.ToDateTime(row["createdAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
                            updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
                            userRoles = new List<userRoleData>()
                        };
                        UserList.Add(existingUser);
                }
                    if (row["roleId"] != DBNull.Value)
                    {
                        existingUser.userRoles?.Add(new userRoleData
                        {
                            roleId = Convert.ToInt32(row["roleId"]),
                            roleName = row["roleName"].ToString()
                        });
                    }
                   
                }



                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"User fetched successfully ";
                Resp.apiResponse = UserList;
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
        public IActionResult Login(UsersRoleModel user)
        {
            try
            {
                string hashedPassword = EncryptDecryptPassword.Encrypt("ABC", user.userPassword);

                string query = $@"
            SELECT usr.*, R.roleId, R.roleName 
            FROM User_Mst usr
            JOIN User_Role_Mst UR ON usr.userId = UR.userId
            JOIN Role_Mst R ON UR.roleId = R.roleId
            WHERE usr.userEmail = '{user.userEmail}' 
              AND usr.userPassword = '{hashedPassword}' 
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
                WebTokenDetails _tokenDetails = new WebTokenDetails();
                List<KeyDetails> lst1 = new List<KeyDetails>
        {
            new KeyDetails { KeyName = "userId", KeyValue = userData["userId"].ToString() },
            new KeyDetails { KeyName = "userName", KeyValue = userData["userName"].ToString() },
            new KeyDetails { KeyName = "roleId", KeyValue = userData["roleId"].ToString() },
            new KeyDetails { KeyName = "roleName", KeyValue = userData["roleName"].ToString() }
            ,
            new KeyDetails { KeyName = "deviceId", KeyValue = user.deviceId },
        };

                _userdetails.ListKeydetails = lst1;

                string token = _web.GenerateToken(new WebTokenValidationParameters
                {
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = _configuration["Jwt:Key"],
                   
                }, new UserDetails
                {
                    ListKeydetails = _userdetails.ListKeydetails
                });
                _tokenDetails.Token = token;
                _tokenDetails.TokenKeyName = "deviceId";
                
                  _tokenDetails = _web.ExtractTokenInformation(_tokenDetails);
                //_tokendetails.TokenKeyName = "Role_Id";
                //_tokendetails = _web.ExtractTokenInformation(_tokendetails);
                Console.WriteLine($"Here is the token {token}");
                Console.WriteLine($"Token Details {_tokenDetails}");
              var ip = Dns.GetHostEntry(Dns.GetHostName())
                   .AddressList
                   .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                   .ToString();
              //  user.ipAddress = ip;
                _query = _dc.InsertOrUpdateEntity(new InsertUpdatePerameters
                {
                    entity = user,
                    tableName= "UserLog"

                });
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "User logged in successfully";
                Resp.isSuccess = true;
                Resp.apiResponse = new
                {
                    _tokenDetails.Token,
                   // token,
                    User_Id = userData["userId"],
                    User_Name = userData["userName"],
                    Role_Id = userData["roleId"],
                    Role_Name = userData["roleName"],
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



        //[AllowAnonymous]
        //[HttpPost]
        //[Route("Login")]
        //public IActionResult Login(UsersRoleModel user)
        //{
        //    try
        //    {
        //        string hashedPassword = EncryptDecryptPassword.Encrypt("ABC", user.userPassword);

        //        string query = $@"
        //    SELECT usr.*, R.roleId, R.roleName 
        //    FROM User_Mst usr
        //    JOIN User_Role_Mst UR ON U.userId = UR.userId
        //    JOIN Role_Mst R ON UR.roleId = R.roleId
        //    WHERE usr.userEmail = '{user.userEmail}' 
        //      AND usr.userPassword = '{hashedPassword}' 
        //      AND R.roleId = '{user.roleId}'";

        //        var result = _Connection.bindmethod(query);
        //        DataTable table = result._DataTable;
        //        DataRow userData = table.Rows.Count > 0 ? table.Rows[0] : null;

        //        if (userData == null)
        //        {
        //            Resp.statusCode = StatusCodes.Status404NotFound;
        //            Resp.message = "Invalid credentials or role mismatch.";
        //            return StatusCode(StatusCodes.Status404NotFound, Resp);
        //        }

        //        if (!(bool)userData["userStatus"])
        //        {
        //            Resp.statusCode = StatusCodes.Status403Forbidden;
        //            Resp.message = "User is not active. Please contact the administrator.";
        //            return StatusCode(StatusCodes.Status403Forbidden, Resp);
        //        }

        //        WebToken _web = new WebToken();
        //        UserDetails _userdetails = new UserDetails();
        //        List<KeyDetails> lst1 = new List<KeyDetails>
        //{
        //    new KeyDetails { KeyName = "userId", KeyValue = userData["userId"].ToString() },
        //    new KeyDetails { KeyName = "userName", KeyValue = userData["userName"].ToString() },
        //    new KeyDetails { KeyName = "roleId", KeyValue = userData["roleId"].ToString() },
        //    new KeyDetails { KeyName = "roleName", KeyValue = userData["roleName"].ToString() }
        //};

        //        _userdetails.ListKeydetails = lst1;

        //        string token = _web.GenerateToken(new WebTokenValidationParameters
        //        {
        //            ValidIssuer = _configuration["Jwt:Issuer"],
        //            ValidAudience = _configuration["Jwt:Audience"],
        //            IssuerSigningKey = _configuration["Jwt:Key"],
        //            //ValidIssuer = "http://localhost:5266/",
        //            //ValidAudience = "http://localhost:5266/",
        //            //IssuerSigningKey = "2Fsk5LBU5j1DrPldtFmLWeO8uZ8skUzwhe3ktVimUE8l=",
        //        }, new UserDetails
        //        {
        //            ListKeydetails = _userdetails.ListKeydetails
        //        });

        //        Console.WriteLine($"Here is the token {token}");

        //        Resp.statusCode = StatusCodes.Status200OK;
        //        Resp.message = "User logged in successfully";
        //        Resp.isSuccess = true;
        //        Resp.apiResponse = new
        //        {
        //            token,
        //            User_Id = userData["userId"],
        //            User_Name = userData["userName"],
        //            Role_Id = userData["roleId"],
        //            Role_Name = userData["roleName"]
        //        };

        //        return StatusCode(StatusCodes.Status200OK, Resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        Resp.statusCode = StatusCodes.Status500InternalServerError;
        //        Resp.message = ex.Message;
        //        return StatusCode(StatusCodes.Status500InternalServerError, Resp);
        //    }
        //}


        [HttpPost]
        [Route("AddEditUser")]
        public IActionResult AddEditUser([FromForm] UsersRoleModel user)
        {

        
            try
            {

                if (user.userPassword != null)
                {

                    string hashedPassword = EncryptDecryptPassword.Encrypt("ABC", user.userPassword);

                    user.userPassword = hashedPassword;

                }
                user.userName = _dc.ConvertLetterCase(new LetterCasePerameter
                {
                    caseType = "titlecase",
                    column = user.userName
                });
       

                if (_dc.CheckNullValues(user.userName)|| _dc.CheckNullValues(user.userEmail)|| _dc.CheckNullValues(user.userPassword))
                {
                    Resp.statusCode = StatusCodes.Status204NoContent;
                    Resp.message = $"User Email ,Name,Password can't be blank ";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);

                }
                if (!_dc.CheckNullValues(user.userId.ToString()) && user.userId > 0 && user.updateFlag == true)
                {
                    var userExistsQuery = $"SELECT COUNT(*) FROM User_Mst WHERE userId = {user.userId}";
                    int userExists =
                        (int)(Connection.ExecuteScalar(userExistsQuery));

                    if (userExists == 0)
                    {
                        Resp.statusCode = StatusCodes.Status404NotFound;
                        Resp.message = $"User ID does not exist.";
                        return StatusCode(StatusCodes.Status404NotFound, Resp);
                    }

                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "User_Mst",
                        fields = new[] { "userName", "userEmail" },
                        values = new[] { user.userName, user.userEmail },
                        idField = "userId",
                        idValue= user.userId.ToString(),
                        
                    };

                    if (_dc.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"User Name User Email already exists.";
                        Resp.dup = true;
                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }

                    user.updatedAt = DateTime.Now.ToString();
                    _query = _dc.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = user,
                        tableName = "User_Mst",
                        id = (int)user.userId,
                        idPropertyName = "userId",
                        

                    });

                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = "User  Updated Successfully";

                    }

                }
                else
                {
                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "User_Mst",
                        fields = new[] { "userName", "userEmail" },
                        values = new[] { user.userName, user.userEmail }
                    };

                    if (_dc.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"User already exists.";
                        Resp.dup = true;


                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }


                    _query = _dc.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = user,
                        tableName = "User_Mst",

                    });
                    Resp.message = "User Added successfully";
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

        [HttpDelete]
        [Route("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                var userExists = $"SELECT COUNT(*) FROM User_Mst WHERE userId = {userId} ";
                int result = Convert.ToInt32(Connection.ExecuteScalar(userExists));


                if (result == 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"User ID does not exist.";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);

                }
               
                string checkQuery = $"SELECT COUNT(*) AS recordCount FROM User_Role_Mst WHERE userId = {userId}";


                int userIdInUser = Convert.ToInt32(Connection.ExecuteScalar(checkQuery));
                if (userIdInUser > 0)
                {
                    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    Resp.message = $"Can't delete Exists in another table";

                    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);


                }
                string deleteUserQuery = $"Delete from User_Mst where userId='{userId}'";

                Connection.ExecuteNonQuery(deleteUserQuery);
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "User Deleted successfully";
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
