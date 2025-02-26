﻿using LkDataConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Task_Management.Classes;
using Task_Management.Model;

namespace Task_Management.Controllers
{
  //  [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        Connection _Connection = new Connection();
        ApiResponse Resp = new ApiResponse();
        Validation validation = new Validation();
     
        DataAccess _dc = new DataAccess();
       SqlQueryResult _query = new SqlQueryResult();
       
        [HttpGet]
        [Route("GetAllRole")]
        public IActionResult GetAllRole()
        {
            try
            {
                string query = $"select * from Role_Mst ORDER BY roleName ASC";
                
                var result = _Connection.bindmethod(query);
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
                        createdAt = Convert.ToDateTime(row["createdAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
                        updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("dd-MM-yyyy HH:mm:ss")
                      


                    });
                }

             




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

        [Route("AddEditRole")]
        public IActionResult AddEditRole([FromBody] RolesModel role)
        {
            try
            {
               
                insertupdateTestclass insertupdateTestclass = new insertupdateTestclass();


              
                if (validation.CheckNullValues(role.roleName))
                {
                    Resp.statusCode = StatusCodes.Status204NoContent;
                    Resp.message = $"RoleName Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);

                }
                role.roleName = validation.ConvertLetterCase(new LetterCasePerameter
                {
                    caseType = "titlecase",
                    column = role.roleName
                });
                if (!validation.CheckNullValues(role.roleId.ToString()) && role.roleId > 0 && role.updateFlag == true)

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

                    //var duplicacyParameter = new CheckDuplicacyPerameter
                    //{
                    //    tableName = "Role_Mst",
                    //    fields = new[] { "roleName" },
                    //    values = new[] { role.roleName },
                    //    idField = "roleId",
                    //    idValue = role.roleId.ToString()
                    //};

                    //if (_dc.CheckDuplicate(duplicacyParameter))
                    //{
                    //    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    //    Resp.message = $"RoleName already exists.";
                    //    Resp.dup = true;
                    //    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    //}
                    var duplicacyParameter = new checkDuplicacyper
                    {
                        tableName = "Role_Mst",
                        fields = new[] { "roleName" },
                        values = new[] { role.roleName },
                        idField = "roleId",
                        idValue = role.roleId.ToString()
                    };

                    if (validation.CheckDuplicate(duplicacyParameter))
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

                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = "Role Name Updated Successfully";

                    }
                    // _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", (int)role.roleId, "roleId");
                }
                else
                {

                    //var duplicacyParameter = new CheckDuplicacyPerameter
                    //{
                    //    tableName = "Role_mst",
                    //    fields = new[] { "roleName" },
                    //    values = new[] { role.roleName }
                    //};

                    //if (_dc.CheckDuplicate(duplicacyParameter))
                    //{
                    //    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    //    Resp.message = $"RoleName already exists.";
                    //    Resp.dup = true;


                    //    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    //}

                    var duplicacyParameter = new checkDuplicacyper
                    {
                        tableName = "Role_mst",
                        fields = new[] { "roleName" },
                        values = new[] { role.roleName }
                    };

                    if (validation.CheckDuplicate(duplicacyParameter))
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
                    if (_query.QueryErrorMessage!=null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = $"RoleName Added Successfully";
                    }

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
        [Route("DeleteRole/{roleId}")]
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
             
                string checkQuery = $"SELECT COUNT(*) AS recordCount FROM User_Role_Mst WHERE roleId = {roleId}";


                int roleIdInUser = Convert.ToInt32(Connection.ExecuteScalar(checkQuery));
                if (roleIdInUser > 0)
                {
                    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    Resp.message = $"Can't delete Exists in another table";

                    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);


                }
                string deleteRoleQuery = $"Delete from Role_Mst where roleId='{roleId}'";

                 Connection.ExecuteNonQuery(deleteRoleQuery);
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "Role  Deleted successfully";
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
