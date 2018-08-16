using SqlParser.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public delegate List<WhereModel> ConvertToWhereList(string groupString, List<TableInfoModel> allTables);
    public class WhereGroup
    {
        public List<WhereModel> WhereStatements { get; set; }

        public List<WhereGroup> WhereGroups { get; set; }

        public bool IsExternal { get; set; } = false;

        public static WhereGroup ToWhereGroup(string whereString,List<TableInfoModel> allTables, ConvertToWhereList converter=null)
        {
            if (string.IsNullOrEmpty(whereString))
            {
                return new WhereGroup() { WhereGroups = new List<WhereGroup>(),WhereStatements = new List<WhereModel>()};
            }
            var whereStringArray = whereString.ToCharArray();
            FullWhereModel parent = null;
            FullWhereModel current = null;
            for(int i = 0; i < whereStringArray.Length-1; i++)
            {
                if(whereStringArray[i] == '(')
                {
                    parent = current;
                    current = new FullWhereModel() { Parent = parent,Group=new WhereGroup() { WhereGroups = new List<WhereGroup>(), WhereStatements = new List<WhereModel>() } };
                    continue;
                }
                else if(whereStringArray[i] == ')')
                {
                    if (current.Parent != null)
                    {
                        current.Parent.Group.AddNewGroup(current.Group);
                        current = current.Parent;
                    }
                    else
                    {
                        var group = current.Group;
                        current = new FullWhereModel { Parent = null,Group = new WhereGroup() { WhereGroups = new List<WhereGroup>(), WhereStatements = new List<WhereModel>() } };
                        current.Group.AddNewGroup(group);
                    }
                }
                else
                {
                    var groupEnd = i;
                    while(groupEnd < whereStringArray.Length && whereStringArray[groupEnd]!='(' && whereStringArray[groupEnd] != ')' )
                    {
                        groupEnd++;
                    }
                    var groupString = whereString.Substring(i, groupEnd - i);
                    if (groupString.Trim(' ','\r') == string.Empty)
                    {
                        i = groupEnd - 1;
                        continue;
                    }
                    bool isOr = groupString.Trim(' ') == "OR";
                    bool isAnd = groupString.Trim(' ') == "AND";
                    if (isOr || isAnd)
                    {
                        var lastGroup = current.Group.WhereGroups.LastOrDefault();
                        while (lastGroup.WhereGroups.Count > 0)
                        {
                            lastGroup = lastGroup.WhereGroups.LastOrDefault();
                        }
                        lastGroup.WhereStatements.LastOrDefault().UseOr = isOr;
                        i = groupEnd - 1;
                        continue;
                    }
                    var convertMethod = converter ?? ToWhereModelList;
                    var wheres = convertMethod(groupString, allTables);
                    if (current == null)
                    {
                        current = new FullWhereModel { Parent = parent,Group = new WhereGroup() { WhereGroups = new List<WhereGroup>(), WhereStatements=new List<WhereModel>()} };
                        parent = current;
                    }
                    current.Group.AddWhereStatements(wheres);
                    i = groupEnd - 1;
                } 
            }
            return current.Group;
        }

        private static List<WhereModel> ToWhereModelList(string groupString, List<TableInfoModel> allTables)
        {
            var list = new List<WhereModel>();
            if (string.IsNullOrEmpty(groupString))
            {
                return null;
            }
            string pattern = @"(.*?)\.\W*(\w+)\s*(IS\s*NOT|IS|LIKE|<=|>=|[<,>,=]|<>)\s*('.*?'|(\w+\.\w+)|\w+)\s*(\w*)";
            var matches = groupString.GetMatchWithPattern(pattern);
            
            while (matches.Success)
            {
                var tableName = GetTableNameFromString(matches.Groups[1].Value);
                var columnName = matches.Groups[2].Value;
                var comparison = matches.Groups[3].Value;
                var useOr = matches.Groups[6].Value.ToUpper() == "OR";
                var whereModel = new WhereModel
                {
                    TableId = allTables.FirstOrDefault(t => t.TableAlias.Trim(' ') == tableName.Trim(' ')).Id,
                    ColumnAlias = columnName,
                    Comparison = comparison,
                    UseOr = useOr,
                    CompareWithColumn = new ColumnModel()
                };
                if (string.IsNullOrEmpty(matches.Groups[5].Value))
                {
                    var value = matches.Groups[4].Value;
                    whereModel.Value = value;
                }
                else
                {
                    var tableColumn = matches.Groups[5].Value;
                    var tableColumnInfo = tableColumn.Split('.');
                    var tableTo = allTables.FirstOrDefault(t => t.TableAlias == tableColumnInfo[0]);
                    var column = new ColumnModel
                    {
                        ColumnAlias = tableColumnInfo[1],
                        TableId = tableTo.Id
                    };
                    whereModel.CompareWithColumn = column;
                }
               
                
                list.Add(whereModel);
                matches = matches.NextMatch();
            }
            var trimmedWhereString = groupString.TrimStart(' ');
            var lastWhere = list.LastOrDefault();
            if (lastWhere != null)
            {
                lastWhere.UseOr = trimmedWhereString.StartsWith("OR");
            }
            return list;
        }

        private static string GetTableNameFromString(string str)
        {
            var strChars = str.ToCharArray();
             Array.Reverse(strChars);
            var tableNameString = string.Empty;
            for(int i=0; i < strChars.Length; i++)
            {
                if (!char.IsLetterOrDigit(strChars[i]))
                {
                    break;
                }
                tableNameString += strChars[i];
            }

            return new String(tableNameString.Reverse().ToArray());
        }
        
    }
}
