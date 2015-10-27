using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSLQ2Oracle.DTOs
{
    public class Table
    {
        public string Name { get; set; }
        public string PrimaryKey { get; set; }
        public bool IsPrimaryKeyAutoIncrement { get; set; }
        public List<TableColumn> Columns { get; set; }

        public Table()
        {
            this.Columns = new List<TableColumn>();
        }
    }
}
