using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class WhereModel : ComparisonModel
    {
        public bool UseOr { get; set; }
    }
}
