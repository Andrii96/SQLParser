using SqlParser.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class JoinModel
    {
        public TableViewModel ToTable { get; set; }

        public WhereGroup JoinCondition { get; set; }

        public static JoinModel Parse(string joinStatement,List<TableInfoModel> allTables)
        {
            if (string.IsNullOrEmpty(joinStatement))
            {
                return null;
            }
            var matches = joinStatement.GetMatchWithPattern(@"(\w+)(.*?)on\s+(\w+).(\w+)\W+(\w+).(\w+)");
            if (matches != null)
            {
                var tableTo = allTables.FirstOrDefault(table =>
                {
                    if (matches.Groups[2].Value.Trim(' ') != string.Empty)
                    {
                        return table.Name == matches.Groups[1].Value && table.TableAlias.Trim(' ') == matches.Groups[2].Value.Trim(' ');
                    }
                    return table.Name == matches.Groups[1].Value;
                });
                return new JoinModel
                {
                    ToTable = new TableViewModel
                    {
                        Name = tableTo.Name,
                        Schema = tableTo.Schema,
                        Id = tableTo.Id
                    },
                    JoinCondition = new WhereGroup
                    {
                        WhereStatements = new List<WhereModel>
                        {
                            new WhereModel
                            {
                                ColumnAlias = matches.Groups[4].Value,
                                CompareTo = matches.Groups[6].Value,
                                Comparison = "="
                            }
                        },
                        IsExternal = false,
                        WhereGroups = new List<WhereGroup>()
                    }
                };
              
            }
            return null;
        }
    }
}
