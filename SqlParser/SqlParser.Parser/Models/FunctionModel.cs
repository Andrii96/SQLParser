using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class FunctionModel
    {
        public string ColumnAlias { get; set; }

        public string Function { get; set; }

        public string Alias { get; set; }
    }
}
