using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSLQ2Oracle.DTOs
{
    public class TableColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsNullable { get; set; }
        public string Value { get; set; }
    }
}
