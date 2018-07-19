using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class ColorRuleModel:ComparisonModel
    {
        public string TableName { get; set; }

        public string Color { get; set; }
    }
}
