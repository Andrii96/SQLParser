using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class TableViewModel
    {
        private static Random rand = new Random();
        public string Name { get; set; }
        
        public string Schema { get; set; }

        public long Id { get; set; }

        public override string ToString() => $"{Schema}.{Name}";

        public static TableViewModel Parse(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return null;
            }
            return new TableViewModel
            {
                Id = rand.Next(1000000, 9999999),
                Name = tableName,
                Schema = "dbo"
            };
        }
    }
}
