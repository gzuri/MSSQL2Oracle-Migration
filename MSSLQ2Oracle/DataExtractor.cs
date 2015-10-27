using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace MSSLQ2Oracle
{
    public class DataExtractor
    {
        string mConnectionString;

        public DataExtractor(string connectionString)
        {
            mConnectionString = connectionString;
        }

        static string SqlReadyValue(string sqlBuiltString, Column column, int columnIndex, SqlDataReader reader)
        {
            if (columnIndex > 0)
                sqlBuiltString += ", ";

            if (reader[columnIndex] == null || String.IsNullOrEmpty(reader[columnIndex].ToString()))
                return sqlBuiltString += "NULL";


            switch (column.DataType.SqlDataType)
            {
                case SqlDataType.Int:
                    sqlBuiltString += reader.GetInt32(columnIndex);
                    break;
                case SqlDataType.SmallInt:
                    sqlBuiltString += reader.GetInt16(columnIndex);
                    break;
                case SqlDataType.BigInt:
                    sqlBuiltString += reader.GetInt64(columnIndex);
                    break;
                case SqlDataType.Float:
                    try
                    {
                        float floatNumber = reader.GetFloat(columnIndex);
                        sqlBuiltString += floatNumber.ToString(CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        double doubleNumber = reader.GetDouble(columnIndex);
                        sqlBuiltString += doubleNumber.ToString(CultureInfo.InvariantCulture);

                    }
                    break;



                case SqlDataType.Decimal:
                    decimal decimalNumber = reader.GetDecimal(columnIndex);
                    sqlBuiltString += decimalNumber.ToString(CultureInfo.InvariantCulture);
                    break;

                case SqlDataType.DateTime2:
                case SqlDataType.DateTime:
                    var date = reader.GetDateTime(columnIndex);
                    sqlBuiltString += String.Format("TO_DATE('{0}', 'yyyy-mm-dd hh24:mi:ss')", date.ToString("yy-MM-dd HH:mm:ss"));
                    break;
                case SqlDataType.Bit:
                    var bitData = reader.GetBoolean(columnIndex);
                    sqlBuiltString += bitData == true ? "1" : "0";
                    break;
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarChar:
                    sqlBuiltString += "'" + reader.GetString(columnIndex) + "'";
                    break;
                case SqlDataType.UniqueIdentifier:
                    sqlBuiltString += "'" + reader.GetGuid(columnIndex).ToString() + "'";
                    break;
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                    sqlBuiltString += "NULL";
                    break;
            }

            return sqlBuiltString;
        }

        void ProcesDbRecord(Table table, SqlDataReader reader, StreamWriter sw)
        {
            var columnValues = String.Empty;
            var columnNames = String.Empty;

            for (var i = 0; i < table.Columns.Count; i++)
            {
                columnNames += String.Format(" {0},", Helpers.ConvertNaming(table.Columns[i].Name));
                columnValues = SqlReadyValue(columnValues, table.Columns[i], i, reader);
            }

            columnValues = columnValues.Trim(',');
            columnNames = columnNames.Trim(',');

            sw.WriteLine("INSERT INTO {0} ({1}) VALUES ({2});", Helpers.ConvertNaming(table.Name), columnNames, columnValues);

        }

        void ExtractDataForTable(Table table, StreamWriter sw)
        {
            using (var conn = new SqlConnection(mConnectionString))
            {
                conn.Open();
                var command = new SqlCommand(String.Format("SELECT * FROM {0}", table.Name), conn);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ProcesDbRecord(table, reader, sw);
                    }
                }
            }
        }

        void ProcessTable(Table table, string destDirPath)
        {
            if (!Directory.Exists(destDirPath))
                Directory.CreateDirectory(destDirPath);

            var filePath = Path.Combine(destDirPath, table.Name + ".sql");
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (StreamWriter sw = File.CreateText(filePath))
            {
                ExtractDataForTable(table, sw);
            }
        }


        public void CreateDBInserts(string connectionString, Database db, string destDirPath)
        {
            Stopwatch timer = Stopwatch.StartNew();
            foreach (Table table in db.Tables)
            {
                Console.Write("Processing table {0} ", table.Name);
                
                ProcessTable(table, destDirPath);
                Console.Write("done in {0} ms", timer.ElapsedMilliseconds);
                timer.Restart();
                Console.Write(Environment.NewLine);

            }
        }

    }
}
