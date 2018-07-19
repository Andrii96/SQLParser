using SqlParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlParser.Parser.Helpers
{
    public static class Extensions
    {
        public static Match GetMatchWithPattern(this string value, string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(value);
            return match.Success ? match : null;
        }

        public static void AddWhereStatements(this WhereGroup group, List<WhereModel> whereModel)
        {
            if (group.WhereStatements == null)
            {
                group.WhereStatements = new List<WhereModel>();
            }
            whereModel.ForEach(where => group.WhereStatements.Add(where));
        }

        public static void AddWhereStatement(this WhereGroup group, WhereModel whereModel)
        {
            if(group.WhereStatements == null)
            {
                group.WhereStatements = new List<WhereModel>();
            }
            group.WhereStatements.Add(whereModel);
        }

        public static void AddNewGroup(this WhereGroup group, WhereGroup whereGroup)
        {
            if (group.WhereGroups == null)
            {
                group.WhereGroups= new List<WhereGroup>();
            }
            group.WhereGroups.Add(whereGroup);
        }
    }
}
