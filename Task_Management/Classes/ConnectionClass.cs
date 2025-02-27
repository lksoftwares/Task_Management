using LkDataConnection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Task_Management.Classes
{
    public class ConnectionClass
    {
        private string _ConnectionString;

        private SqlConnection _connection;
        private IConfiguration configuration1;
       
        public ConnectionClass(IConfiguration configuration)
        {
            configuration1 = configuration;

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

        public dynamic tokenParam()
        {
            string Issuer = configuration1["Jwt:Issuer"];
            string Audience = configuration1["Jwt:Audience"];
            string key = configuration1["Jwt:Key"];
            object tokenper = new
            {
                Issuer,
                Audience,
                key
            };
            return (tokenper);

        }


        
        

    }
    }

