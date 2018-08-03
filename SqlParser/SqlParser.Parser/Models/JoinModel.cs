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

        public static FullJoinModel Parse(string joinStatement, List<TableInfoModel> allTables)
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
                var joinModel = new JoinModel
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
                                CompareWithColumn=new ColumnModel
                                {
                                    ColumnAlias= matches.Groups[6].Value,
                                    TableId = tableTo.Id,
                                    //TableName = tableTo.Name
                                },
                                Comparison = "="
                            }
                        },
                        IsExternal = false,
                        WhereGroups = new List<WhereGroup>()
                    }
                };

                return new FullJoinModel
                {
                    FromTable = matches.Groups[3].Value,
                    Join = joinModel
                };

            }
            return null;
        }

        #region Helpers

        //private static List<WhereModel> ToWhereModelList(string groupString, List<TableInfoModel> allTables)
        //{
        //    var wheresList = new List<WhereModel>();
        //    if (string.IsNullOrEmpty(groupString))
        //    {
        //        return null;
        //    }
        //    var matches = groupString.GetMatchWithPattern(@"\W*(\w+).(\w+)\W+(\w+).(\w+)\s*(\w*)");
        //    while (matches.Success)
        //    {
        //        var tableName = GetTableNameFromString(matches.Groups[1].Value);
        //        var whereModel = new WhereModel
        //        {

        //            ColumnAlias = matches.Groups[2].Value,
        //            CompareTo = matches.Groups[4].Value,
        //            Comparison = "=",
        //            UseOr = matches.Groups[5].Value == "OR"
        //        };
        //        wheresList.Add(whereModel);
        //        matches = matches.NextMatch();

        //    }
        //    var trimmedWhereString = groupString.TrimStart(' ');
        //    var lastWhere = wheresList.LastOrDefault();
        //    if (lastWhere != null)
        //    {
        //        lastWhere.UseOr = trimmedWhereString.StartsWith("OR");
        //    }
        //    return wheresList;

        //}


        //private static string GetTableNameFromString(string str)
        //{
        //    var strChars = str.ToCharArray();
        //    Array.Reverse(strChars);
        //    var tableNameString = string.Empty;
        //    for (int i = 0; i < strChars.Length; i++)
        //    {
        //        if (!char.IsLetterOrDigit(strChars[i]))
        //        {
        //            break;
        //        }
        //        tableNameString += strChars[i];
        //    }

        //    return new String(tableNameString.Reverse().ToArray());
        //}
        #endregion
    }
}
