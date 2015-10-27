using CommandLine;
using CommandLine.Text;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSLQ2Oracle
{
    class Program
    {
        class Options
        {
            [Option('c', "connectionstring", Required = true,
              HelpText = "SQL Server connection string is required")]
            public string SourceConnectionString { get; set; }

            [Option('p', "path", Required = true,
              HelpText = "Directory path for extracted data is required")]
            public string Path { get; set; }


            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var builder = new SqlConnectionStringBuilder();
                try
                {
                    builder = new SqlConnectionStringBuilder(options.SourceConnectionString);

                }
                catch
                {
                    Console.WriteLine("Please verify that the connection string is in correct format, also please encapsulate it with qoutes");
                }
                Server myServer = new Server(builder.DataSource);
                myServer.ConnectionContext.LoginSecure = true;
                myServer.ConnectionContext.Connect();
                var tableStructureExtractor = new TableStructureExtractor();

                Database sourceDatabase = null;

                List<Table> tables = new List<Table>();
                foreach (Database myDatabase in myServer.Databases)
                {
                    if (myDatabase.Name != builder.InitialCatalog)
                        continue;
                    sourceDatabase = myDatabase;
                }
                if (sourceDatabase == null)
                {
                    Console.WriteLine(String.Format("Database {0} not found on server:{1}", builder.InitialCatalog, builder.DataSource));
                    return;
                }

                tableStructureExtractor.CreateFileWithCreateStatements(sourceDatabase, options.Path);
            }
        }
    }
}
