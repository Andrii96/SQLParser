using SqlParser.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser
{
    public class SqlScript
    {
        public static  List<string> GetSelectments(string sqlScript)
        {
            var pattern = @"\s*(.*?)(union|$)";
            var matches = sqlScript.GetMatchWithPattern(pattern,System.Text.RegularExpressions.RegexOptions.IgnoreCase|System.Text.RegularExpressions.RegexOptions.Singleline);
            var selectments = new List<string>();
            while (matches.Success)
            {
                var result = matches.Groups[1].Value;
                if (!string.IsNullOrEmpty(result))
                {
                    selectments.Add(result);
                }
                matches = matches.NextMatch();
            }
            return selectments;
        }
    }
}
