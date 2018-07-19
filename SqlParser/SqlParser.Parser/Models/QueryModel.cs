using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class QueryModel
    {
        public List<QueryTableModel> TableQueries { get; set; }

        public bool SelectDistinct { get; set; }

        public bool ShowDetails { get; set; } = true;

        public List<CalculatedColumnModel> CalculatedColumns { get; set; }

        public List<ColorRuleModel> ColorRules { get; set; }

        public object TablePositionData { get; set; }
    }
}
