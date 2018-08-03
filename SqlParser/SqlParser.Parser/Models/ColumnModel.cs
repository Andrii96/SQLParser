using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class ColumnModel
    {
        public string ColumnAlias { get; set; }
        public long TableId { get; set; }
        public string TableName { get; set;}
    }
}
