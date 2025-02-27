//using LkDataConnection;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Data;
//using Task_Management.Classes;
//using Task_Management.Model;

//namespace Task_Management.Controllers
//{
//   // [Authorize]
//    [Route("/[controller]")]
//    [ApiController]
//    public class UserRoleController : ControllerBase
//    {

//        Connection _Connection = new Connection();
//        ApiResponse Resp = new ApiResponse();
//        Validation validation = new Validation();

//        DataAccess _dc = new DataAccess();
//        SqlQueryResult _query = new SqlQueryResult();

//        //[HttpGet]
//        //[Route("GetAllUserRole")]
//        //public IActionResult GetAllUserRole()
//        //{
//        //    try
//        //    {
//        //        string query = $"select UR.*,r.roleName,U.userName from User_Role_Mst UR join User_Mst U ON UR.userId = U.userId join Role_Mst R ON UR.roleId = R.roleId order by UR.createdAt";

//        //        var result = _Connection.bindmethod(query);
//        //        DataTable Table = result._DataTable;

//        //        if (Table == null)
//        //        {
//        //            Resp.statusCode = StatusCodes.Status200OK;
//        //            Resp.message = $"No Data Found ";
//        //            Resp.isSuccess = true;

//        //            return Ok(Resp);

//        //        }


//        //        var UserRoleList = new List<UserRoleModel>();
//        //        foreach (DataRow row in Table.Rows)
//        //        {
//        //            UserRoleList.Add(new UserRoleModel
//        //            {
//        //                userRoleId= Convert.ToInt32(row["userRoleId"]),
//        //                roleId = Convert.ToInt32(row["roleId"]),
//        //                roleName = row["roleName"].ToString(),
//        //                userId = Convert.ToInt32(row["userId"]),
//        //                userName = row["userName"].ToString(),
//        //                createdAt = Convert.ToDateTime(row["createdAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
//        //                updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("dd-MM-yyyy HH:mm:ss")



//        //            });
//        //        }






//        //        Resp.statusCode = StatusCodes.Status200OK;
//        //        Resp.message = $"User Role fetched successfully ";
//        //        Resp.apiResponse = UserRoleList;
//        //        Resp.isSuccess = true;

//        //        return Ok(Resp);
//        //    }
//        //    catch (Exception ex)
//        //    {

//        //        Resp.statusCode = StatusCodes.Status500InternalServerError;
//        //        Resp.message = ex.Message;

//        //        return StatusCode(StatusCodes.Status500InternalServerError, Resp);

//        //    }

//        //}

       
//        [HttpPost]
//        [Route("AddEditUserRole")]
//        public IActionResult AddEditUserRole([FromBody] UserRoleModel userRole)
//        {
//            try
//            {

//                //insertupdateTestclass insertupdateTestclass = new insertupdateTestclass();



//                if (validation.CheckNullValues(userRole.userId.ToString())|| validation.CheckNullValues(userRole.roleId.ToString()))
//                {
//                    Resp.statusCode = StatusCodes.Status204NoContent;
//                    Resp.message = $"User and Role Can't be Blank Or Null";

//                    return StatusCode(StatusCodes.Status204NoContent, Resp);

//                }
               
//                if (!validation.CheckNullValues(userRole.userRoleId.ToString()) && userRole.userRoleId > 0 && userRole.updateFlag == true)

//                {
//                    var UserRoleExistsQuery = $"SELECT COUNT(*) FROM User_Role_Mst WHERE userRoleId = {userRole.userRoleId}";
//                    int UserRoleExists =
//                        (int)(Connection.ExecuteScalar(UserRoleExistsQuery));

//                    if (UserRoleExists == 0)
//                    {
//                        Resp.statusCode = StatusCodes.Status404NotFound;
//                        Resp.message = $"UserRole ID does not exist.";
//                        return StatusCode(StatusCodes.Status404NotFound, Resp);
//                    }
//                    var duplicacyParameter = new CheckDuplicacyPerameter
//                    {
//                        tableName = "User_Role_Mst",
//                        fields = new[] { "userId","roleId" },
//                        values = new[] { userRole.userId.ToString(),userRole.roleId.ToString() },
//                        idField = "userRoleId",
//                        idValue = userRole.userRoleId.ToString(),
//                        andFlag = true
                        
//                    };

//                    if (_dc.CheckDuplicate(duplicacyParameter))
//                    {
//                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
//                        Resp.message = $" This Role Already Assigned To The Same User";
//                        Resp.dup = true;
//                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
//                    }




//                    userRole.updatedAt = DateTime.Now.ToString();
//                    _query = _dc.InsertOrUpdateEntity(new InsertUpdatePerameters
//                    {
//                        entity = userRole,
//                        tableName = "User_Role_Mst",
//                        id = (int)userRole.userRoleId,
//                        idPropertyName = "userRoleId"

//                    });

//                    if (_query.QueryErrorMessage != null)
//                    {
//                        Resp.message = _query.QueryErrorMessage;
//                    }
//                    else
//                    {
//                        Resp.message = "User And Role  Updated Successfully";

//                    }
                  
//                }
//                else
//                {
//                    var duplicacyParameter = new CheckDuplicacyPerameter
//                    {
//                        tableName = "User_Role_Mst",
//                        fields = new[] { "userId", "roleId" },
//                        values = new[] { userRole.userId.ToString(), userRole.roleId.ToString() },
//                        andFlag = true
                        

//                    };

//                    if (_dc.CheckDuplicate(duplicacyParameter))
//                    {
//                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
//                        Resp.message = $" This Role Already Assigned To The Same User";
//                        Resp.dup = true;
//                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
//                    }





//                    _query = _dc.InsertOrUpdateEntity(new InsertUpdatePerameters
//                    {
//                        entity = userRole,
//                        tableName = "User_Role_Mst",

//                    });



            
//                    if (_query.QueryErrorMessage != null)
//                    {
//                        Resp.message = _query.QueryErrorMessage;
//                    }
//                    else
//                    {
//                        Resp.message = $"User And Role Added Successfully";
//                    }

//                }
//                Resp.statusCode = StatusCodes.Status200OK;

//                Resp.isSuccess = true;

//                return StatusCode(StatusCodes.Status200OK, Resp);

//            }
//            catch (Exception ex)
//            {

//                Resp.statusCode = StatusCodes.Status500InternalServerError;
//                Resp.message = ex.Message;


//                return StatusCode(StatusCodes.Status500InternalServerError, Resp);
//            }
//        }





//        [HttpDelete]
//        [Route("DeleteUserRole/{userRoleId}")]
//        public IActionResult DeleteUserRole(int userRoleId)
//        {
//            try
//            {
//                var userRoleExists = $"SELECT COUNT(*) FROM User_Role_Mst WHERE userRoleId = {userRoleId} ";
//                int result = Convert.ToInt32(Connection.ExecuteScalar(userRoleExists));


//                if (result == 0)
//                {
//                    Resp.statusCode = StatusCodes.Status404NotFound;
//                    Resp.message = $"UserRole ID does not exist.";

//                    return StatusCode(StatusCodes.Status404NotFound, Resp);

//                }

               
//                string deleteUserRoleQuery = $"Delete from User_Role_Mst where userRoleId='{userRoleId}'";

//                Connection.ExecuteNonQuery(deleteUserRoleQuery);
//                Resp.statusCode = StatusCodes.Status200OK;
//                Resp.message = "UserRole  Deleted successfully";
//                Resp.isSuccess = true;


//                return StatusCode(StatusCodes.Status200OK, Resp);


//            }
//            catch (Exception ex)
//            {
//                Resp.statusCode = StatusCodes.Status500InternalServerError;
//                Resp.message = ex.Message;


//                return StatusCode(StatusCodes.Status500InternalServerError, Resp);
//            }
//        }

//    }
//}
