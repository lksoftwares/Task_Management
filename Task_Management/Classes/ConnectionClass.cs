using LkDataConnection;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Task_Management.Classes
{
    public class ConnectionClass
    {
        private string _ConnectionString;

        private SqlConnection _connection;
        public ConnectionClass(IConfiguration configuration)
        {


            EncryptDecrypt _lkencr = new EncryptDecrypt();


            string encryptedServer = configuration.GetConnectionString("server");
            string encryptedUser = configuration.GetConnectionString("user");
            string encryptedPassword = configuration.GetConnectionString("password");
            string encryptedDatabase = configuration.GetConnectionString("database");
            //string ecryptedServer = _lkencr.Encrypt("ABC", encryptedServer);
            //string ecryptedUser = _lkencr.Encrypt("ABC", encryptedUser);
            //string ecryptedPassword = _lkencr.Encrypt("ABC", encryptedPassword);
            //string ecryptedDatabase = _lkencr.Encrypt("ABC", encryptedDatabase);


            string decryptedServer = _lkencr.Decrypt("ABC", encryptedServer);
            string decryptedUser = _lkencr.Decrypt("ABC", encryptedUser);
            string decryptedPassword = _lkencr.Decrypt("ABC", encryptedPassword);
            string decryptedDatabase = _lkencr.Decrypt("ABC", encryptedDatabase);

            string connectionString = configuration.GetConnectionString("dbcs");
            _ConnectionString = connectionString
                .Replace("$server", decryptedServer)
                .Replace("$user", decryptedUser)
                .Replace("$password", decryptedPassword)
                .Replace("$database", decryptedDatabase);
            _connection = new SqlConnection(_ConnectionString);



        }
        public SqlConnection GetSqlConnection()
        {
            return _connection;
        }
        public object ExecuteScalar(string query)
        {
            
                SqlCommand command = new SqlCommand(query, _connection);
                if (_connection.State != ConnectionState.Open)
                {
                    try
                    {
                        _connection.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database connection failed.", ex);
                    }
                }
                return command.ExecuteScalar();

            }
        

    }
    }

