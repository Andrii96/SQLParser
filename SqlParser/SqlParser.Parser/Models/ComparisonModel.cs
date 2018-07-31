using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class ComparisonModel
    {
        public long TableId { get; set; }

        public string ColumnAlias { get; set; }

        public string Comparison { get; set; }

        public string CompareTo { get; set; }
    }
}
