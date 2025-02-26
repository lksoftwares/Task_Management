using LkDataConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using Task_Management.Classes;
using Task_Management.Model;

namespace Task_Management.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class WorkingStatusController : ControllerBase
    {
        Connection _Connection = new Connection();
        ApiResponse Resp = new ApiResponse();
        Validation validation = new Validation();

        DataAccess _dc = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();
      
        [HttpGet]
        [Route("GetWorkingStatus")]
        public IActionResult GetWorkingStatus()
        {
            try
            {
                string query = $"select DW.*,U.userName from Daily_Working_Txn DW join User_Mst U ON  DW.userId = U.userID order by U.userName ";
              

                var result = _Connection.bindmethod(query);
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
                        workingNote = row["workingNote"].ToString(),
                        workingDesc = row["workingDesc"].ToString(),
                        userName = row["userName"].ToString(),
                        workingDate = Convert.ToDateTime(row["workingDate"]).ToString("dd-MM-yyyy HH:mm:ss"),
                        createdAt = Convert.ToDateTime(row["createdAt"]).ToString("dd-MM-yyyy HH:mm:ss"),
                        updatedAt = Convert.ToDateTime(row["updatedAt"]).ToString("dd-MM-yyyy HH:mm:ss")


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

        [Route("AddWorkingStatus")]
        public IActionResult AddWorkingStatus([FromBody] WorkingStatusModel workingSatatusModel)
        {
            try
            {
                insertupdateTestclass insertupdateTestclass = new insertupdateTestclass();



                if (validation.CheckNullValues(workingSatatusModel.workingDesc))
                {
                    Resp.statusCode = StatusCodes.Status404NotFound;
                    Resp.message = $"Description Can't be Blank Or Null";

                    return StatusCode(StatusCodes.Status204NoContent, Resp);
                }


                _query = insertupdateTestclass.InsertOrUpdateEntity(new InsertUpdatePerameters
                {

                    entity = workingSatatusModel,
                    tableName = "Daily_Working_Txn",
                   

                }) ;


                //if (_query.QueryErrorMessage != null )
                if (!validation.CheckNullValues(_query.QueryErrorMessage) )

                {
                    Resp.message = _query.QueryErrorMessage;
                }
                else
                {
                    Resp.message = $"Daily Working Added Successfully";
                }
                    // _query = _dc.InsertOrUpdateEntity(role, "Role_Mst", -1);

                 

                
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
