using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class CalculatedColumnEntity
    {
        public string Action { get; set; }

        public CalculatedColumnEntity LeftOp { get; set; }

        public CalculatedColumnEntity RightOp { get; set; }

        public string Value { get; set; }

        public CalculatedEntityType EntityType { get; set; }

        public List<ReplaceIfCalculationModel> ReplaceIfOptions { get; set; }
    }
}
