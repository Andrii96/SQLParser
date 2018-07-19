using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class TableInfoModel : TableViewModel { 
        public string TableAlias { get; set; }

        public static TableInfoModel Parse(string tableName, string tableAlias)
        {
            var table = Parse(tableName);
            return new TableInfoModel
            {
                Id = table.Id,
                Name = table.Name,
                Schema = table.Schema,
                TableAlias = tableAlias
            };
        }
    }
}
