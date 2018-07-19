using SqlParser.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class SelectedColumnModel
    {
        public string ColumnName { get; set; }
        public string Alias { get; set; }

        public override string ToString() => $"Name:{ColumnName},Alias:{Alias}";

        #region Methods
        public static SelectedColumnModel Parse(string column)
        {
            var matches = column.GetMatchWithPattern(@"\s*(.*?) as (\w*)");
            return new SelectedColumnModel
            {
                Alias = matches != null ? matches.Groups[2].Value.Trim(' ') : column,
                ColumnName = matches != null ? matches.Groups[1].Value.Trim(' ') : column
            };
        }
        #endregion

    }
}
