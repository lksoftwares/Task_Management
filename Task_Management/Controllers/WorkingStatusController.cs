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
    public class WorkingStatusController : ControllerBase
    {
        ApiResponse Resp = new ApiResponse();


        private ConnectionClass _connection;
        DataAccess _dc = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();
        public WorkingStatusController(ConnectionClass connection)
        {
            _connection = connection;
            Connection.Connect();
            Connection.ConnectionStr = _connection.GetSqlConnection().ConnectionString;
        }
        [HttpGet]
        [Route("GetWorkingStatus")]
        public IActionResult GetWorkingStatus()
        {
            try
            {
                string query = $"select * from Daily_Working_Txn ";
                var connection = new Connection();
                var result = connection.bindmethod(query);
                DataTable Table = result._DataTable;
                if (Table == null)
                {
                    Resp.statusCode = StatusCodes.Status200OK;
                    Resp.message = $"No Data Found ";
                    Resp.isSuccess = true;

                    return Ok(Resp);

                }


                var workingSatusList = new List<WorkingStatusModel>();
                foreach (DataRow row in Table.Rows)
                {
                    workingSatusList.Add(new WorkingStatusModel
                    {
                        txnId = Convert.ToInt32(row["txnId"]),
                        userId = Convert.ToInt32(row["userId"]),

                        workingDesc = row["workingDesc"].ToString(),
                        workingDate = Convert.ToDateTime(row["workingDate"]),
                        createdAt = Convert.ToDateTime(row["createdAt"]),
                        updatedAt = Convert.ToDateTime(row["updatedAt"])


                    });
                }

                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = $"Daily Working  fetched successfully ";
                Resp.apiResponse = workingSatusList;
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

        [Route("SaveWorkingStatus")]
        public IActionResult SaveWorkingStatus([FromBody] WorkingStatusModel workingSatatusModel)
        {
            try
            {

                insertupdateTestclass insertupdateTestclass = new insertupdateTestclass();



                if (String.IsNullOrEmpty(workingSatatusModel.workingDesc))
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Description Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);
                }

               
                if (workingSatatusModel.txnId != null && !string.IsNullOrEmpty(workingSatatusModel.txnId.ToString()) && workingSatatusModel.txnId > 0)

                {
                    var txnExistsQuery = $"SELECT COUNT(*) FROM Daily_Working_Txn WHERE txnId = {workingSatatusModel.txnId}";
                    int txnExists =
                        (int)(Connection.ExecuteScalar(txnExistsQuery));

                    if (txnExists == 0)
                    {
                        Resp.statusCode = StatusCodes.Status404NotFound;
                        Resp.message = $"Transaction ID does not exist.";
                        return StatusCode(StatusCodes.Status404NotFound, Resp);
                    }
                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Daily_Working_Txn",
                        fields = new[] { "userId", "workingDate" },
                        values = new[] { workingSatatusModel.userId.ToString(),workingSatatusModel.workingDate.ToString() },
                        idField = "txnId",
                        idValue = workingSatatusModel.txnId.ToString()
                    };

                    if (_dc.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $" Already Working Reported ";
                        Resp.dup = true;
                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }



                    workingSatatusModel.updatedAt = DateTime.Now;
                    _query = insertupdateTestclass.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = workingSatatusModel,
                        tableName = "Daily_Working_Txn",
                        id = (int)workingSatatusModel.txnId,
                        idPropertyName = "txnId"

                    });
                    // _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", (int)role.roleId, "roleId");
                    Resp.message = "Daily Working Updated Successfully";
                }
                else
                {

                    var duplicacyParameter = new CheckDuplicacyPerameter
                    {
                        tableName = "Daily_Working_Txn",
                        fields = new[] { "userId", "workingDate" },
                        values = new[] { workingSatatusModel.userId.ToString(), workingSatatusModel.workingDate.ToString()}
                    };

                    if (_dc.CheckDuplicate(duplicacyParameter))
                    {
                        Resp.statusCode = StatusCodes.Status208AlreadyReported;
                        Resp.message = $"Already Working Reported";
                        Resp.dup = true;


                        return StatusCode(StatusCodes.Status208AlreadyReported, Resp);
                    }


                    _query = insertupdateTestclass.InsertOrUpdateEntity(new InsertUpdatePerameters
                    {
                        entity = workingSatatusModel,
                        tableName = "Daily_Working_Txn",

                    });



                    // _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", -1);

                    Resp.message = $"Daily Working Added Successfully";

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
        [Route("deleteWorkingStatus/{txnId}")]
        public IActionResult deleteWorkingStatus(int txnId)
        {
            try
            {
                var roleExists = $"SELECT COUNT(*) FROM Daily_Working_Txn WHERE txnId = {txnId} ";
                int result = Convert.ToInt32(Connection.ExecuteScalar(roleExists));


                if (result == 0)
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Txn ID does not exist.";

                    return StatusCode(StatusCodes.Status404NotFound, Resp);

                }
                // ------functionality not delete record due to exists in another table -----------------
                //string checkQuery = $"";


                //int roleIdInUser = Convert.ToInt32(Connection.ExecuteScalar(checkQuery));
                //if (roleIdInUser > 0)
                //{
                //    Resp.statusCode = StatusCodes.Status404NotFound;
                //    Resp.message = $"Can't delete Exists in another table";

                //    return StatusCode(StatusCodes.Status404NotFound, Resp);


                //}
                string deleteRoleQuery = $"Delete from Daily_Working_Txn where txnId='{txnId}'";

                Connection.ExecuteNonQuery(deleteRoleQuery);
                Resp.statusCode = StatusCodes.Status200OK;
                Resp.message = "Working Status Deleted successfully";
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
