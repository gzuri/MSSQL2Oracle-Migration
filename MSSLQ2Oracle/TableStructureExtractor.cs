using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSSLQ2Oracle.DTOs;
using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace MSSLQ2Oracle
{
    public class TableStructureExtractor
    {
        List<string> CreateTableStatements(List<DTOs.Table> tables)
        {
            var lines = new List<string>();

            foreach (var table in tables)
            {
                var createLine = "CREATE TABLE " + table.Name + " ( ";
                var columnIndex = 0;
                foreach (var column in table.Columns)
                {
                    createLine += column.Name + " " + column.Type;
                    if (!column.IsNullable && columnIndex != 0)
                        createLine += "NOT NULL";
                    ++columnIndex;
                    createLine += ", ";
                }

                createLine += String.Format("PRIMARY KEY ({0}) );", table.PrimaryKey);
                lines.Add(createLine);
            }

            return lines;

        }

        List<DTOs.Table> GetTables(Microsoft.SqlServer.Management.Smo.Database database)
        {
            var tables = new List<DTOs.Table>();
            foreach (Microsoft.SqlServer.Management.Smo.Table dbTable in database.Tables)
            {
                var table = new DTOs.Table
                {
                    Name = Helpers.ConvertNaming(dbTable.Name)
                };

                tables.Add(table);

                foreach (Microsoft.SqlServer.Management.Smo.Column dbColumn in dbTable.Columns)
                {
                    DataTable tb = dbColumn.EnumForeignKeys();

                    var sqlType = String.Empty;
                    switch (dbColumn.DataType.SqlDataType)
                    {
                        case SqlDataType.BigInt:
                        case SqlDataType.Int:
                        case SqlDataType.SmallInt:
                            sqlType = "NUMBER(12)";
                            break;
                        case SqlDataType.DateTime2:
                        case SqlDataType.DateTime:
                            sqlType = "DATE";
                            break;
                        case SqlDataType.NVarChar:
                        case SqlDataType.NVarCharMax:
                        case SqlDataType.VarChar:
                        case SqlDataType.UniqueIdentifier:
                            sqlType = "varchar2(255)";
                            break;
                        case SqlDataType.Float:
                            sqlType = "NUMBER(12,5)";
                            break;
                        case SqlDataType.Bit:
                            sqlType = "NUMBER(1,0)";
                            break;
                        case SqlDataType.Decimal:
                            sqlType = "NUMBER(12, 2)";
                            break;
                        case SqlDataType.VarBinary:
                        case SqlDataType.VarBinaryMax:
                            sqlType = "BLOB";
                            break;
                        default:
                            break;
                    }
                    if (dbColumn.InPrimaryKey)
                        table.PrimaryKey = Helpers.ConvertNaming(dbColumn.Name);

                    table.Columns.Add(new TableColumn
                    {
                        IsNullable = dbColumn.Nullable,
                        Name = Helpers.ConvertNaming(dbColumn.Name),
                        Type = sqlType
                    });
                }
            }
            return tables;
        }

        public void CreateFileWithCreateStatements(Database sourceDb, string path)
        {
            var tables = this.GetTables(sourceDb);
            var sql = CreateTableStatements(tables);
            Helpers.WriteStatementsToFile(sql, path, "CreateStatements.sql");
        }

    }
}
