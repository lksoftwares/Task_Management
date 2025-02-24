using LkDataConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Task_Management.Classes;
using Task_Management.Model;

namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
          //private ResponseModel Resp = new ResponseModel();
        ApiResponse Resp = new ApiResponse();

     
        private ConnectionClass _connection;
        DataAccess _dc = new DataAccess();
       SqlQueryResult _query = new SqlQueryResult();
        public RolesController(ConnectionClass connection)
        {
            _connection = connection;
             Connection.Connect();
             Connection.ConnectionStr = _connection.GetSqlConnection().ConnectionString;

        }

        [HttpGet]
        [Route("GetAllRole")]
        public IActionResult GetAllRole()
        {
            try
            {
                string query = $"select * from Role_Mst ORDER BY roleName ASC";
                var connection = new  Connection();
                var result =  connection.bindmethod(query);
                DataTable Table = result._DataTable;
                if (Table == null)
                {
                    Resp.statusCode = StatusCodes.Status200OK;
                    Resp.message = $"No Role Found ";
                    Resp.isSuccess = true;

                    return Ok(Resp);

                }
               

                var RoleList = new List<RolesModel>();
                foreach (DataRow row in Table.Rows)
                {
                    RoleList.Add(new RolesModel
                    {
                        roleId = Convert.ToInt32(row["roleId"]),
                        roleName = row["roleName"].ToString(),
                        roleStatus = Convert.ToBoolean(row["roleStatus"]),
                        createdAt = Convert.ToDateTime(row["createdAt"]).ToString("MM-dd-yyyy HH:mm:ss"),
                        updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("MM-dd-yyyy HH:mm:ss")
                        //createdAt = Convert.ToDateTime(row["createdAt"]),
                        //updatedAt = Convert.ToDateTime(row["updatedAt"])


                    });
                }

             



        // DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");

        Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"Role fetched successfully ";
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
        [HttpPost]

        [Route("SaveRole")]
        public IActionResult SaveRole([FromBody] RolesModel role)
        {
            try
            {
               
                insertupdateTestclass insertupdateTestclass = new insertupdateTestclass();
               


                if (String.IsNullOrEmpty(role.roleName))
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"RoleName Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);
                }

                if (role.roleName != null || !string.IsNullOrEmpty(role.roleName))
                {
                    role.roleName = new LetterCase().ConvertLetterCase(new LetterCasePerameter
                    {
                        caseType = "uppercase",
                        column = role.roleName
                    });
                }
                if (role.roleId != null && !string.IsNullOrEmpty(role.roleId.ToString()) && role.roleId > 0)

                {
                    var roleExistsQuery = $"SELECT COUNT(*) FROM Role_Mst WHERE roleId = {role.roleId}";
                    int roleExists = 
                        (int)(Connection.ExecuteScalar(roleExistsQuery));

                    if (roleExists == 0)
                    {
                        Resp.statusCode = StatusCodes.Status404NotFound;
                        Resp.message = $"Role ID does not exist.";
                        return StatusCode(StatusCodes.Status404NotFound, Resp);
                    }

                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Role_Mst",
                        fields = new[] { "roleName" },
                        values = new[] { role.roleName },
                        idField = "roleId",
                        idValue = role.roleId.ToString()
                    };

                    if (_dc.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"RoleName already exists.";
                        Resp.dup = true;
                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }

                    role.updatedAt = DateTime.Now.ToString();
                    _query = insertupdateTestclass.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = role,
                        tableName = "Role_Mst",
                        id = (int)role.roleId,
                        idPropertyName = "roleId"

                    });
                    // _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", (int)role.roleId, "roleId");
                    Resp.message = "RoleName Updated Successfully";
                }
                else
                {

                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Role_mst",
                        fields = new[] { "roleName" },
                        values = new[] { role.roleName }
                    };

                    if (_dc.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"RoleName already exists.";
                        Resp.dup = true;


                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }



                    _query = insertupdateTestclass.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = role,
                        tableName = "Role_Mst",

                    });



                    // _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", -1);

                    Resp.message = $"RoleName Added Successfully";
                   
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

        [HttpPost]

        [Route("AddRole")]
        public IActionResult AddRole([FromBody] RolesModel role)
        {
            try
            {

                var duplicacyParameter = new CheckDuplicacyPerameter
                {
                    tableName = "Role_mst",
                    fields = new[] { "roleName" },
                    values = new[] { role.roleName }
                };

                if (_dc.CheckDuplicate(duplicacyParameter))
                {
                    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    Resp.message = $"RoleName already exists.";
                    Resp.dup = true;

                    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                }
                if (String.IsNullOrEmpty(role.roleName))
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"RoleName Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);
                }


                if (role.roleName != null || !string.IsNullOrEmpty(role.roleName))
                {
                    role.roleName = new LetterCase().ConvertLetterCase(new LetterCasePerameter
                    {
                        caseType = "uppercase",
                        column = role.roleName
                    });
                }




                _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", -1);

                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"RoleName Added Successfully";
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

        [HttpPut]
        [Route("updateRole/{roleId}")]

        public IActionResult UpdateRole(int roleId, [FromBody] RolesModel role)
        {
            try
            {
                var connection = new Connection();

                var roleExists = $"SELECT COUNT(*) FROM Role_Mst WHERE roleId = {roleId} ";
                int result = Convert.ToInt32(Connection.ExecuteScalar(roleExists));


                if (result == 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Role ID does not exist.";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);
                }

                var duplicacyParameter = new CheckDuplicacyPerameter
                {
                    tableName = "Role_Mst",
                    fields = new[] { "roleName" },
                    values = new[] { role.roleName },
                    idField = "roleId",
                    idValue = roleId.ToString()
                };



                if (_dc.CheckDuplicate(duplicacyParameter))
                {
                    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    Resp.message = $"RoleName already exists.";
                    Resp.dup = true;

                    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);

                }
                if (String.IsNullOrEmpty(role.roleName))
                {
                    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    Resp.message = $"RoleName Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);

                }
             
                if (role.roleName != null || !string.IsNullOrEmpty(role.roleName))
                {
                    role.roleName = new LetterCase().ConvertLetterCase(new LetterCasePerameter
                    {
                        caseType = "lowercase",
                        column = role.roleName
                    });
                }

                role.updatedAt = DateTime.Now.ToString();
                _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", roleId, "roleId");
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "RoleName Updated Successfully";
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
        [Route("deleteRole/{roleId}")]
        public IActionResult DeleteRole(int roleId)
        {
       try
            {
                var roleExists = $"SELECT COUNT(*) FROM Role_Mst WHERE roleId = {roleId} ";
                int result = Convert.ToInt32( Connection.ExecuteScalar(roleExists));


                if (result == 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Role ID does not exist.";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);

                }
                // ------functionality not delete record due to exists in another table -----------------
                string checkQuery = $"";


                int roleIdInUser = Convert.ToInt32(Connection.ExecuteScalar(checkQuery));
                if (roleIdInUser > 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Can't delete Exists in another table";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);


                }
                string deleteRoleQuery = $"Delete from Role_Mst where roleId='{roleId}'";

                 Connection.ExecuteNonQuery(deleteRoleQuery);
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "Role Name Deleted successfully";
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
