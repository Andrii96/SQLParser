using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class CalculatedColumnModel
    {
        public string Alias { get; set; }

        public CalculatedColumnEntity Calculation { get; set; }
    }
}
