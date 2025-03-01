using LkDataConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Task_Management.Classes;
using Task_Management.Model;

namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        Connection _Connection = new Connection();
        ApiResponse Resp = new ApiResponse();
       // Validation validation = new Validation();

        DataAccess _dataAccess = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();

        [HttpGet]
        [Route("GetAllTeam")]
        public IActionResult GetAllTeam()
        {
            try
            {
                string query = $"select * from Team_mst ORDER BY teamName ASC";

                var result = _Connection.bindmethod(query);
                DataTable Table = result._DataTable;

                if (Table == null)
                {
                    Resp.statusCode = StatusCodes.Status200OK;
                    Resp.message = $"No Team Found ";
                    Resp.isSuccess = true;

                    return Ok(Resp);

                }


                var TeamList = new List<TeamModel>();
                foreach (DataRow row in Table.Rows)
                {
                    TeamList.Add(new TeamModel
                    {
                        teamId = Convert.ToInt32(row["teamId"]),
                        teamName = row["teamName"].ToString(),
                        tmDescription = row["tmDescription"].ToString(),                    
                        createdAt = Convert.ToDateTime(row["createdAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
                        updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("dd-MM-yyyy HH:mm:ss")



                    });
                }






                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"Team fetched successfully ";
                Resp.apiResponse = TeamList;
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

        [Route("AddEditTeam")]
        public IActionResult AddEditTeam([FromBody] TeamModel team)
        {
            try
            {





                if (_dataAccess.CheckNullValues(team.teamName))
                {
                    Resp.statusCode = StatusCodes.Status204NoContent;
                    Resp.message = $"Team Name Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);

                }
                team.teamName = _dataAccess.ConvertLetterCase(new LetterCasePerameter
                {
                    caseType = "titlecase",
                    column = team.teamName
                });
                if (!_dataAccess.CheckNullValues(team.teamId.ToString()) && team.teamId > 0 && team.updateFlag == true)

                {
                    var teamExistsQuery = $"SELECT COUNT(*) FROM Team_Mst WHERE teamId = {team.teamId}";
                    int teamExists =
                       (int)Connection.ExecuteScalar(teamExistsQuery);

                    if (teamExists == 0)
                    {
                        Resp.statusCode = StatusCodes.Status404NotFound;
                        Resp.message = $"Team ID does not exist.";
                        return StatusCode(StatusCodes.Status404NotFound, Resp);
                    }

                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Team_Mst",
                        fields = new[] { "teamName" },
                        values = new[] { team.teamName },
                        idField = "teamId",
                        idValue = team.teamId.ToString()
                    };

                    if (_dataAccess.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"Team already exists.";
                        Resp.dup = true;
                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }


                    team.updatedAt = DateTime.Now.ToString();
                    _query = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = team,
                        tableName = "Team_Mst",
                        id = (int)team.teamId,
                        idPropertyName = "teamId"

                    });

                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = "Team  Updated Successfully";

                    }

                }
                else
                {



                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Team_Mst",
                        fields = new[] { "teamName" },
                        values = new[] { team.teamName }
                    };

                    if (_dataAccess.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"Team  already exists.";
                        Resp.dup = true;


                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }

                    _query = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = team,
                        tableName = "Team_Mst",

                    });



                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = $"Team Added Successfully";
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
        [Route("DeleteTeam/{teamId}")]
        public IActionResult DeleteTeam(int teamId)
        {
            try
            {
                var teamExists = $"SELECT COUNT(*) FROM Team_Mst WHERE teamId = {teamId} ";
                int result = Convert.ToInt32(Connection.ExecuteScalar(teamExists));


                if (result == 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Team ID does not exist.";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);

                }

                string checkQuery = $"SELECT COUNT(*) AS recordCount FROM Team_Member_Mst WHERE teamId = {teamId}";


                int teamIdInTmMember = Convert.ToInt32(Connection.ExecuteScalar(checkQuery));
                if (teamIdInTmMember > 0)
                {
                    Resp.statusCode = StatusCodes.Status208AlreadyReported;
                    Resp.message = $"Can't delete Exists in another table";

                    return StatusCode(StatusCodes.Status208AlreadyReported, Resp);


                }
                string deleteRoleQuery = $"Delete from Team_Mst where teamId='{teamId}'";

                Connection.ExecuteNonQuery(deleteRoleQuery);
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "Team  Deleted successfully";
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
        [Route("GetAllTeamMember")]
        public IActionResult GetAllTeamMember()
        {
            try { 
 string query = @"SELECT 
    tmm.tmId,
    tm.teamName,
tm.teamId,
 u.userId,
    u.userName,
    r.roleName,
 r.roleId,
    tmm.tmStatus,
    tmm.assignedAt,
    tmm.createdAt,
    tmm.updatedAt
FROM Team_Member_Mst tmm
JOIN User_Mst u ON tmm.userId = u.userId
JOIN Role_Mst r ON tmm.roleId = r.roleId
JOIN Team_Mst tm ON tmm.teamId = tm.teamId ";

                var result = _Connection.bindmethod(query);
                DataTable Table = result._DataTable;

                if (Table == null)
                {
                    Resp.statusCode = StatusCodes.Status200OK;
                    Resp.message = $"No Team Member Found ";
                    Resp.isSuccess = true;

                    return Ok(Resp);

                }


                var TeamMemberList = new List<TeamModel>();
                foreach (DataRow row in Table.Rows)
                {
                    TeamMemberList.Add(new TeamModel
                    {
                        tmId = Convert.ToInt32(row["tmId"]),
                        teamId = Convert.ToInt32(row["teamId"]),
                        teamName = row["teamName"].ToString(),
                        roleId = Convert.ToInt32(row["roleId"]),
                        roleName = row["roleName"].ToString(),
                        userId = Convert.ToInt32(row["userId"]),
                        userName = row["userName"].ToString(),
                        createdAt = Convert.ToDateTime(row["createdAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
                        updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("dd-MM-yyyy HH:mm:ss")



                    });
                }
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"Team Member fetched successfully ";
                Resp.apiResponse = TeamMemberList;
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

        [Route("AddEditTeamMember")]
        public IActionResult AddEditTeamMember([FromBody] TeamModel team)
        {
            try
            {


                if (_dataAccess.CheckNullValues(team.teamId.ToString())|| _dataAccess.CheckNullValues(team.userId.ToString()) || _dataAccess.CheckNullValues(team.roleId.ToString()))
                {
                    Resp.statusCode = StatusCodes.Status204NoContent;
                    Resp.message = $"Team Id ,User Id ,Role Id Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);

                }
              
                if (!_dataAccess.CheckNullValues(team.tmId.ToString()) && team.tmId > 0 && team.updateFlag == true)

                {
                    var teamExistsQuery = $"SELECT COUNT(*) FROM Team_Member_Mst WHERE tmId = {team.tmId}";
                    int teamExists =
                       (int)Connection.ExecuteScalar(teamExistsQuery);

                    if (teamExists == 0)
                    {
                        Resp.statusCode = StatusCodes.Status404NotFound;
                        Resp.message = $"Tm ID does not exist.";
                        return StatusCode(StatusCodes.Status404NotFound, Resp);
                    }

                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Team_Member_Mst",
                        fields = new[] { "teamId", "userId", "roleId" },
                        values = new[] { team.teamId.ToString(),team.userId.ToString(),team.roleId.ToString() },
                        idField = "tmId",
                        idValue = team.tmId.ToString(),
                        andFlag= true
                    };

                    if (_dataAccess.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"Team Member already exists.";
                        Resp.dup = true;
                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }


                    team.updatedAt = DateTime.Now.ToString();
                    _query = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = team,
                        tableName = "Team_Member_Mst",
                        id = (int)team.tmId,
                        idPropertyName = "tmId"

                    });

                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = "Team  Updated Successfully";

                    }

                }
                else
                {




                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Team_Member_Mst",
                        fields = new[] { "teamId", "userId", "roleId" },
                        values = new[] { team.teamId.ToString(), team.userId.ToString(), team.roleId.ToString() },
                        andFlag=true

                       
                        
                    };

                    if (_dataAccess.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"Team  already exists.";
                        Resp.dup = true;


                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }

                    _query = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = team,
                        tableName = "Team_Member_Mst",

                    });



                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = $"Team Added Successfully";
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
        [Route("deleteTeamMember/{tmId}")]
        public IActionResult deleteTeamMember(int tmId)
        {
            try
            {
                var teamExists = $"SELECT COUNT(*) FROM Team_Member_Mst WHERE tmId = {tmId} ";
                int result = Convert.ToInt32(Connection.ExecuteScalar(teamExists));


                if (result == 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Team ID does not exist.";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);

                }

            
                string deleteRoleQuery = $"Delete from Team_Member_Mst where tmId='{tmId}'";

                Connection.ExecuteNonQuery(deleteRoleQuery);
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "Team  Deleted successfully";
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
