using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class QueryTableModel
    {
        public long TableId { get; set; }

        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public List<SelectedColumnModel> SelectedColumns { get; set; }

        public List<JoinModel> Joins { get; set; }

        public List<WhereGroup> Filters { get; set; }

        public List<string> ExplicitFilters { get; set; }

        public List<string> GroupByColumns { get; set; }

        public List<OrderByModel> Sortings { get; set; }

        public List<FunctionModel> Functions { get; set; }
    }
}
