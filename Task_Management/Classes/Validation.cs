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

    }
 


    public class LetterCasePerameter
    {
        public string caseType { get; set; }
        public string column { get; set; }
    }


}
