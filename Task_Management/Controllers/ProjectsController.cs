using LkDataConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Task_Management.Model;

namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        Connection _Connection = new Connection();
        ApiResponse Resp = new ApiResponse();
        DataAccess _dataAccess = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();
        [HttpGet]
        [Route("GetAllProject")]
        public IActionResult GetAllProject()
        {
            try
            {
                string query = $"select * from Project_Mst where projectStatus = 1  ";

            

                var result = _Connection.bindmethod(query);
                DataTable Table = result._DataTable;
                if (Table == null)
                {
                    Resp.statusCode = StatusCodes.Status200OK;
                    Resp.message = $"No Project Found ";
                    Resp.isSuccess = true;

                    return Ok(Resp);

                }


                var projectList = new List<ProjectModel>();
                foreach (DataRow row in Table.Rows)
                {
                    projectList.Add(new ProjectModel
                    {

                        projectId = Convert.ToInt32(row["projectId"]),
                        projectName = row["projectName"].ToString(),
                        createdBy = Convert.ToInt32(row["createdBy"]),
                        updatedBy = Convert.ToInt32(row["updatedBy"]),
                        projectDescription = row["projectDescription"].ToString(),
                        projectStatus = Convert.ToBoolean(row["projectStatus"]),
                        createdAt = Convert.ToDateTime(row["createdAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
                        updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("dd-MM-yyyy HH:mm:ss")



                    });
                }






                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"Projects fetched successfully ";
                Resp.apiResponse = projectList;
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

        [Route("AddEditProject")]
        public IActionResult AddEditProject([FromBody] ProjectModel project)
        {
            try
            {

                if (_dataAccess.CheckNullValues(project.projectName))
                {
                    Resp.statusCode = StatusCodes.Status204NoContent;
                    Resp.message = $"Project Name Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);

                }
                project.projectName = _dataAccess.ConvertLetterCase(new LetterCasePerameter
                {
                    caseType = "titlecase",
                    column = project.projectName
                });
                if (!_dataAccess.CheckNullValues(project.projectId.ToString()) && project.projectId > 0 && project.updateFlag == true)

                {
                    var projectExistsQuery = $"SELECT COUNT(*) FROM Project_Mst WHERE projectId = {project.projectId}";
                    int roleExists =
                        (int)(Connection.ExecuteScalar(projectExistsQuery));

                    if (roleExists == 0)
                    {
                        Resp.statusCode = StatusCodes.Status404NotFound;
                        Resp.message = $"Project ID does not exist.";
                        return StatusCode(StatusCodes.Status404NotFound, Resp);
                    }

                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Project_Mst",
                        fields = new[] { "projectName" },
                        values = new[] { project.projectName },
                        idField = "projectId",
                        idValue = project.projectId.ToString()
                    };

                    if (_dataAccess.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"Project Name already exists.";
                        Resp.dup = true;
                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }


                    project.updatedAt = DateTime.Now.ToString();
                    _query = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = project,
                        tableName = "Project_Mst",
                        id = (int)project.projectId,
                        idPropertyName = "projectId"

                    });

                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = "Project  Updated Successfully";

                    }

                }
                else
                {



                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Project_mst",
                        fields = new[] { "projectName" },
                        values = new[] { project.projectName }
                    };

                    if (_dataAccess.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"Project already exists.";
                        Resp.dup = true;


                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }

                    _query = _dataAccess.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = project,
                        tableName = "Project_Mst",

                    });



                    if (_query.QueryErrorMessage != null)
                    {
                        Resp.message = _query.QueryErrorMessage;
                    }
                    else
                    {
                        Resp.message = $"Project Added Successfully";
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
        [Route("DeleteProject/{projectId}")]
        public IActionResult DeleteProject(int projectId)
        {
            try
            {
                var projectExists = $"SELECT COUNT(*) FROM Project_Mst WHERE projectId = {projectId} ";
                int result = Convert.ToInt32(Connection.ExecuteScalar(projectExists));


                if (result == 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Project ID does not exist.";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);

                }

             
                string deleteProjectQuery = $"Delete from Project_Mst where projectId ='{projectId}'";

                Connection.ExecuteNonQuery(deleteProjectQuery);
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "Project  Deleted successfully";
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
