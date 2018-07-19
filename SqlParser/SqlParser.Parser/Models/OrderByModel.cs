using SqlParser.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class OrderByModel
    {
        public string OrderByAlias { get; set; }

        public string Direction { get; set; }

        public static OrderByModel Parse(string orderByString)
        {
            var pattern = @"(.*?)\.(\w+)\s*(\w*)";
            var orderByMatches = orderByString.GetMatchWithPattern(pattern);
            if (orderByMatches != null)
            {
                return new OrderByModel
                {
                    Direction = orderByMatches.Groups[3].Value.Trim(' ') == string.Empty ? "ASC" : orderByMatches.Groups[2].Value.Trim(' '),
                    OrderByAlias = orderByMatches.Groups[2].Value
                };
            }
            return null;
        }
    }
}
