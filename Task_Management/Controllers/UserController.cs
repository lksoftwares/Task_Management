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
    public class UserController : ControllerBase
    {
        ApiResponse Resp = new ApiResponse();


        private ConnectionClass _connection;
        DataAccess _dc = new DataAccess();
        SqlQueryResult _query = new SqlQueryResult();
        public UserController(ConnectionClass connection)
        {
            _connection = connection;
            Connection.Connect();
            Connection.ConnectionStr = _connection.GetSqlConnection().ConnectionString;
        }
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                string query = $"select * from User_Mst ORDER BY userName ASC";
                var connection = new Connection();
                var result = connection.bindmethod(query);
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

    }
}
