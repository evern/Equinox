using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDatabase
{
    public static class SQLHelper
    {
        public static string GetSQLQueryConcatenation(List<string> items)
        {
            if (items == null)
                return string.Empty;

            string sqlQueryConcatenation = string.Empty;
            foreach (string item in items)
            {
                sqlQueryConcatenation += string.Concat("'", item, "', ");
            }

            if (sqlQueryConcatenation.Length < 2)
                return string.Empty;

            sqlQueryConcatenation = sqlQueryConcatenation.Substring(0, sqlQueryConcatenation.Length - 2);
            return sqlQueryConcatenation;
        }
    }
}
