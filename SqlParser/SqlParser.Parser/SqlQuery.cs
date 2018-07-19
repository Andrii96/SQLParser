using SqlParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser
{
    public class SqlQueryParser
    {
        #region Properties
        public SqlCommand SqlCommand { get; private set; }
        #endregion

        #region Constructor
        public SqlQueryParser(SqlCommand command)
        {
            SqlCommand = command;
        }
        #endregion

        public List<QueryTableModel> ParseToQueryTableModelList()
        {
            var joins = SqlCommand.GetJoins();
            var fromTable = SqlCommand.GetFromTable();
            var where = SqlCommand.GetWhere();
            var hasGroupBy = SqlCommand.HasGroupBy;
            var orderBys = SqlCommand.GetOrderBys();

            var firstQuery = new QueryTableModel
            {
                Filters = new List<WhereGroup> { where },
                Joins = joins,
                TableId = fromTable.Id,
                TableName = fromTable.Name,
                TableSchema = fromTable.Schema,
                ExplicitFilters = new List<string>(),
                SelectedColumns = GetColumnsFromTable(fromTable.Name, fromTable.TableAlias),
                GroupByColumns = new List<string>(),
                Sortings = new List<OrderByModel>(),
                Functions = new List<FunctionModel>()
            };
            firstQuery.Sortings = GetOrderBysForTable(orderBys, firstQuery.SelectedColumns);
            List<QueryTableModel> queryTableModels = new List<QueryTableModel>{ firstQuery };
            
            foreach (var table in SqlCommand.AllTables)
            {
                var query = new QueryTableModel
                {
                    SelectedColumns = GetColumnsFromTable(table.Name, table.TableAlias),
                    TableId = table.Id,
                    TableName = table.Name,
                    TableSchema = table.Schema,
                    Filters = new List<WhereGroup>(),
                    ExplicitFilters = new List<string>(),
                    Joins = new List<JoinModel>(),
                    GroupByColumns = new List<string>(),
                    Sortings = new List<OrderByModel>(),
                    Functions = new List<FunctionModel>()
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

        #region Helpers
        private List<SelectedColumnModel> GetColumnsFromTable(string tableName, string tableAlias)
        {//TODO: Check method logic
            var a = SqlCommand.GetSelectedColumns();
            return SqlCommand.GetSelectedColumns()
                             .Where(column => column.TableName.Trim(' ') == tableAlias.Trim(' '))
                             .Select(column => SelectedColumnModel.Parse(column.ColumnName))
                             .ToList();
        }

        private List<string> GetGroupByForTable(List<SelectedColumnModel> selectedColumns)
        {
            return selectedColumns.Select(col => col.Alias).ToList();
        }

        private List<OrderByModel> GetOrderBysForTable(List<OrderByModel> allOrderBys, List<SelectedColumnModel> columns)
        {
            var orderByList = allOrderBys.Where(orderBy => columns.Find(col => col.Alias == orderBy.OrderByAlias) != null).ToList();
            return orderByList;
        }
        #endregion

    }
}
