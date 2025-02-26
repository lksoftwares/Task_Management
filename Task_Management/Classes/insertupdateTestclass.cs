using LkDataConnection;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace Task_Management.Classes
{
    public class insertupdateTestclass
    {
        public SqlQueryResult InsertOrUpdateEntity(InsertUpdatePerameter insertUpdatePerameters)
        {
            try
            {
                // Dynamically get the properties of the object at runtime
                PropertyInfo[] properties = insertUpdatePerameters.entity.GetType().GetProperties();
                // A dictionary named parameterDictionary is created to store parameters for the SQL query.
                Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();

                PropertyInfo idProperty = null;
                bool isUpdate = false;

                if (insertUpdatePerameters.id != null && insertUpdatePerameters.id != -1)
                {
                    isUpdate = true;
                }


                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttributes(typeof(SkipInsertAttribute), true).Length > 0)
                        continue;

                    object value = property.GetValue(insertUpdatePerameters.entity);

                    if (value == null)
                        continue;
                    // conntinue if update feild have update idPropertyName
                    if (isUpdate && property.Name == insertUpdatePerameters.idPropertyName)
                    {
                        continue;
                    }
                    if (property.Name == "updateFlag") 
                        continue;
                    if (property.PropertyType == typeof(IFormFile))
                    {
                        // Handle file upload separately
                        var fileProperty = (IFormFile)value;
                        if (fileProperty != null && fileProperty.Length > 0)
                        {
                            string imagePath = SaveImage(insertUpdatePerameters.imgFolderpath, fileProperty);
                            parameterDictionary.Add(property.Name, imagePath);

                        }
                        continue;
                    }
                    var columnMapping = property.GetCustomAttribute<ColumnMappingAttribute>();
                    string columnName = columnMapping != null ? columnMapping.ColumnName : property.Name;
                    if (!isUpdate || (isUpdate && property != idProperty))
                        parameterDictionary.Add(columnName, value);

                }

                string query;
                if (isUpdate)
                {
                    // Construct update query
                    string setClause = string.Join(", ", parameterDictionary.Keys.Select(p => $"{p} = @{p}"));
                    query = $"UPDATE {insertUpdatePerameters.tableName} SET {setClause} WHERE {insertUpdatePerameters.idPropertyName} = @{insertUpdatePerameters.idPropertyName}";
                    parameterDictionary.Add(insertUpdatePerameters.idPropertyName, insertUpdatePerameters.id); // Add ID parameter for update
                }
                else
                {
                    // Construct insert query
                    string columns = string.Join(", ", parameterDictionary.Keys);
                    string parameters = string.Join(", ", parameterDictionary.Keys.Select(p => "@" + p));
                    query = $"INSERT INTO {insertUpdatePerameters.tableName} ({columns}) VALUES ({parameters})";
                }

                // Execute the query with parameter values
                SqlCommand cmd = AddSqlCommandParameters(query, parameterDictionary);
                SqlQueryResult _SqlQueryResult = Connection.ExecuteNonQuery(cmd);
                return (_SqlQueryResult);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting or updating entity into table {insertUpdatePerameters.tableName}: {ex.Message}");
            }
        }
        public static string SaveImage(string FolderPath, IFormFile image)
        {
            string text = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string path = Path.Combine(FolderPath, text);
            using (FileStream target = new FileStream(path, FileMode.Create))
            {
                image.CopyTo(target);
            }

            return text ?? "";
        }
        public SqlCommand AddSqlCommandParameters(string query, Dictionary<string, object> parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(query);
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                sqlCommand.Parameters.AddWithValue("@" + parameter.Key, parameter.Value);
            }

            return sqlCommand;
        }
    }
    public class InsertUpdatePerameter
    {
        public object entity { get; set; }
        public string tableName { get; set; }
        public int id { get; set; } = -1;
        public string idPropertyName { get; set; } = null;
        public string imgFolderpath { get; set; } = null;

    }
}
