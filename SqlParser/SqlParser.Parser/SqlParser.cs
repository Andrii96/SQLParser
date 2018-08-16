﻿using SqlParser.Parser.Helpers;
using SqlParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlParser.Parser
{
    public class SqlParser
    {

        #region fields
        #endregion
        #region Properties
        public string SqlScript { get; private set; }

        public List<TableInfoModel> AllTables { get; private set; }

        public bool HasGroupBy => SqlScript.GetMatchWithPattern(@"group by") != null;

        public bool Distinct => SqlScript.GetMatchWithPattern(@"distinct") != null;

        public TableInfoModel FromTable { get; private set; }

        public List<ExtendedFunctionModel> Functions { get; } = new List<ExtendedFunctionModel>();
        #endregion

        #region Constructor

        public SqlParser(string sqlScript)
        {
            SqlScript = sqlScript;
            AllTables = GetTables();
            FromTable = GetFromTable();
        }
        #endregion

        #region Methods
        
        private TableInfoModel GetFromTable()
        {
            var table = GetFromTableName();
            return TableInfoModel.Parse(table.Item1,table.Item2);
        }

        public List<ColumnModel> GetSelectedColumns()
        {
            return GetAllSelectedColumns(); 
        }

        public List<FullJoinModel> GetJoins()
        {
            var joinsString = GetJoinsString();
            var allTables = new List<TableInfoModel>();
            allTables.AddRange(AllTables);
            allTables.Add(FromTable);
            return joinsString.Select(join => JoinModel.Parse(join,allTables)).ToList();
        }

        public WhereGroup GetWhere()
        {
            var whereString = GetWhereString();
            var allTables = new List<TableInfoModel>();
            allTables.AddRange(AllTables);
            allTables.Add(FromTable);
            return WhereGroup.ToWhereGroup(whereString,allTables);
        }

        public List<OrderByModel> GetOrderBys()
        {
            var orderBys = GetOrderBysString();
            return orderBys.Select(orderBy => OrderByModel.Parse(orderBy)).ToList();
        }
        #endregion

        #region Helpers

        
       
        private List<ColumnModel> GetAllSelectedColumns()
        {
            var matches =  SqlScript.GetMatchWithPattern(@"Select ((.|\n)*)from");
            if (matches != null)
            {
                var columns = matches.Groups[1].Value.Split(',').Select(c => c.Trim(' ', '\r', '\n')).ToList();
                var columnsList = new List<ColumnModel>();
                columns.ForEach(column =>
                {
                    var functionPattern = @"(.*?)\(\s*(\w+)\.(\w+)\s*\)\s*as\s*(\w+)";
                    var functionMatches = column.GetMatchWithPattern(functionPattern);
                    if (functionMatches == null)
                    {
                        var splitedColumn = column.Split('.');
                        if (splitedColumn.Length > 1)
                        {
                            columnsList.Add( new ColumnModel
                            {
                                ColumnAlias = splitedColumn[1],
                                TableName = splitedColumn[0]
                            });
                        }
                    }
                    else
                    {
                        var functionModel = new ExtendedFunctionModel
                        {
                            Function = new FunctionModel
                            {
                                Alias = functionMatches.Groups[4].Value,
                                Function = functionMatches.Groups[1].Value,
                                ColumnAlias = functionMatches.Groups[3].Value
                            },
                            TableId = GetTableId(functionMatches.Groups[2].Value)
                        };
                        Functions.Add(functionModel);
                    }
                    //return new ColumnModel();
                });

                return columnsList;
            }
            return null;
        }

        private Tuple<string,string> GetFromTableName()
        {
            var fromString = SqlScript.GetMatchWithPattern(@"from (.*?)(?=join|\n)");

            var matches = fromString.Groups[1].Value.GetMatchWithPattern(@"\s*(\w+)\s*(.*?)\s(\w*)");
            if (matches != null)
            {
                var tableName = matches.Groups[1].Value;
                var aliasName = matches.Groups[2].Value.Trim(' ').ToLower()=="as"? matches.Groups[3].Value:matches.Groups[2].Value.Trim(' ');
                if(aliasName == string.Empty)
                {
                    aliasName = tableName;
                }
                return new Tuple<string, string>(tableName, aliasName);
            }
            return null;
        }

        private List<BaseJoin> GetJoinsString()
        {
            List<BaseJoin> joinStatements = new List<BaseJoin>();
            var matches = SqlScript.GetMatchWithPattern(@"(right|left|inner|full)\s*join (.*?)(?=join|where|\n)");
            if (matches != null)
            {
                while (matches.Success)
                {
                    joinStatements.Add(new BaseJoin
                    {
                        JoinType = matches.Groups[1].Value,
                        JoinStatement = matches.Groups[2].Value

                    });
                    matches = matches.NextMatch();
                }
            }
            return joinStatements;
        }

        private List<TableInfoModel> GetTables()
        {
            var list = new List<TableInfoModel>();
            var joins = GetJoinsString();
            var pattern = @"\s*(\w+)\s*(.*?)on";
            foreach (var join in joins)
            {
                var match = join.JoinStatement.GetMatchWithPattern(pattern);
                if (match != null)
                {
                    var tableName = match.Groups[1].Value.Trim(' ');
                    var tableAlias = match.Groups[2].Value.Trim(' ');
                    if(tableAlias == string.Empty)
                    {
                        tableAlias = tableName;
                    }
                    list.Add(TableInfoModel.Parse(tableName, tableAlias));
                }
            }
            return list;
        }

        private string GetWhereString()
        {
            var matches = SqlScript.GetMatchWithPattern(@"where\s*(.*?)(?=group\s*by|order\s*by|\n|$)");
            if (matches != null)
            {
                return matches.Groups[1].Value;
            }
            return null;
        }

        private List<string> GetOrderBysString()
        {
            var pattern = @"order by (.*?)(?=offset|\n|$)";
            var matches = SqlScript.GetMatchWithPattern(pattern);
            if (matches != null)
            {
                return new List<string>(matches.Groups[1].Value.Split(','));
            }
            return new List<string>();
        }

        private long GetTableId(string tableAlias)
        {
            var allTables = new List<TableInfoModel>();
            allTables.AddRange(AllTables);
            allTables.Add(FromTable);
            var table = allTables.FirstOrDefault(t => t.TableAlias.Trim(' ') == tableAlias.Trim(' '));
            return table != null ? table.Id : 0;
        }
        #endregion
    }
}
