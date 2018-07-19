using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class ReplaceIfCalculationModel
    {
        public string Comparison { get; set; }

        public string CompareTo { get; set; }

        public CalculatedColumnEntity ReplaceWith { get; set; }
    }
}
