﻿using SqlParser.Parser.Models;
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
        public static List<QueryTableModel> ParseToQueryTableModelList(this SqlParser sqlCommand)
        {
            var joins = sqlCommand.GetJoins();
            var fromTable = sqlCommand.FromTable;
            var hasGroupBy = sqlCommand.HasGroupBy;
            var orderBys = sqlCommand.GetOrderBys();
            var selectedColumns = sqlCommand.GetSelectedColumns();
            var firstQuery = new QueryTableModel
            {
                Joins = joins.Where(join => join.FromTable == fromTable.TableAlias).Select(join => join.Join).ToList(),
                Id = fromTable.Id,
                TableName = fromTable.Name,
                TableSchema = fromTable.Schema,
                ExplicitFilters = new List<string>(),
                SelectedColumns = GetColumnsFromTable(selectedColumns, fromTable.Name, fromTable.TableAlias),
                GroupByColumns = new List<string>(),
                Sortings = new List<OrderByModel>(),
                Functions = sqlCommand.Functions.Where(f => f.TableId == fromTable.Id).Select(f=>f.Function).ToList()
        };
            firstQuery.Sortings = GetOrderBysForTable(orderBys, firstQuery.SelectedColumns);
            List<QueryTableModel> queryTableModels = new List<QueryTableModel> { firstQuery };

            foreach (var table in sqlCommand.AllTables)
            {
                var query = new QueryTableModel
                {
                    SelectedColumns = GetColumnsFromTable(selectedColumns, table.Name, table.TableAlias),
                    Id = table.Id,
                    TableName = table.Name,
                    TableSchema = table.Schema,
                    ExplicitFilters = new List<string>(),
                    Joins = joins.Where(join => join.FromTable == table.TableAlias).Select(join => join.Join).ToList() ?? new List<JoinModel>(),
                    GroupByColumns = new List<string>(),
                    Sortings = new List<OrderByModel>(),
                    Functions = sqlCommand.Functions.Where(f=>f.TableId == table.Id).Select(f=>f.Function).ToList()
                };
                query.Sortings = GetOrderBysForTable(orderBys, query.SelectedColumns);
                queryTableModels.Add(query);
            }
            if (hasGroupBy)
            {
                queryTableModels.ForEach(query => query.GroupByColumns = GetGroupByForTable(query.SelectedColumns));
            }
            return queryTableModels;
        }
        public static Match GetMatchWithPattern(this string value, string pattern,RegexOptions options= RegexOptions.IgnoreCase)
        {
            var regex = new Regex(pattern, options);
           
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

        #region Helpers
        private static List<SelectedColumnModel> GetColumnsFromTable(List<ColumnModel> columns,string tableName, string tableAlias)
        {//TODO: Check method logic
            return columns.Where(column => column.TableName.Trim(' ') == tableAlias.Trim(' '))
                          .Select(column => SelectedColumnModel.Parse(column.ColumnAlias))
                          .ToList();
        }

        private static List<string> GetGroupByForTable(List<SelectedColumnModel> selectedColumns)
        {
            return selectedColumns.Select(col => col.Alias).ToList();
        }

        private static List<OrderByModel> GetOrderBysForTable(List<OrderByModel> allOrderBys, List<SelectedColumnModel> columns)
        {
            var orderByList = allOrderBys.Where(orderBy => columns.Find(col => col.Alias == orderBy.OrderByAlias) != null).ToList();
            return orderByList;
        }
        #endregion
    }
}
