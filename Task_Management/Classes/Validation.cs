using LkDataConnection;
using System.Data;
using System.Data.Common;

namespace Task_Management.Classes
{
    public class Validation
    {
        
            public string ConvertLetterCase(LetterCasePerameter letterCasePerameter)
            {
                if (string.IsNullOrEmpty(letterCasePerameter.column))
            {
                    return letterCasePerameter.column; 
                }

                string transformedValue;

                switch (letterCasePerameter.caseType.ToLower())
                {
                    case "titlecase":
                        transformedValue = new System.Globalization.CultureInfo("en-US", false)
                            .TextInfo.ToTitleCase(letterCasePerameter.column.ToLower());
                        break;

                    case "uppercase":
                        transformedValue = letterCasePerameter.column.ToUpper();
                        break;

                    case "lowercase":
                        transformedValue = letterCasePerameter.column.ToLower();
                        break;

                    default:
                        transformedValue = letterCasePerameter.column;
                        break;
                }

                return transformedValue;
            }
        public bool CheckNullValues(string columnName)
        {
            if (columnName == null || string.IsNullOrEmpty(columnName.ToString()))
            {
                return true;
            }
            return false;
        }

        public bool CheckDuplicate(checkDuplicacyper duplicacyPerameter)
        {
            string logicalOperator = duplicacyPerameter.OrAndFlag ? "AND" : "OR";

            string conditions = string.Join($" {logicalOperator} ", Enumerable.Range(0, duplicacyPerameter.fields.Length)
                .Select(i => $"{duplicacyPerameter.fields[i]} = '{duplicacyPerameter.values[i]}'"));

            string query = $"SELECT * FROM {duplicacyPerameter.tableName} WHERE ({conditions})";

            if (!string.IsNullOrEmpty(duplicacyPerameter.idField) && duplicacyPerameter.idValue != null)
            {
                query += $" AND {duplicacyPerameter.idField} != '{duplicacyPerameter.idValue}'";
            }
            var connection = new Connection();
            var result = connection.bindmethod(query);
            DataTable Table = result._DataTable;
          

            return Table.Rows.Count > 0;
        }
    }
 


    public class LetterCasePerameter
    {
        public string caseType { get; set; }
        public string column { get; set; }
    }


   public class checkDuplicacyper
    {
        public string tableName { get; set; }

        public string[] fields { get; set; }

        public string[] values { get; set; }

        public string idField { get; set; } = null;


        public string idValue { get; set; } = null;
        public bool OrAndFlag { get; set; } = false;


    }
}
