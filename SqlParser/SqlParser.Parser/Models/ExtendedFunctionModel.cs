using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class ExtendedFunctionModel
    {
        public FunctionModel Function { get; set; }
        public long TableId { get; set; }
    }
}
