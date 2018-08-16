using Newtonsoft.Json;
using SqlParser.Parser;
using SqlParser.Parser.Helpers;
using SqlParser.Parser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Please, specify sql file path via command line.");
                return;
            }
            var sqlScript = File.ReadAllText(args[0]);
            var selectments = SqlScript.GetSelectments(sqlScript);
            var queriesModelList = new List<QueryModel>();
            foreach (var select in selectments)
            {
                var selectParser = new SqlParser.Parser.SqlParser(select);
                var queries = selectParser.ParseToQueryTableModelList();
                var queryModel = new QueryModel
                {
                    TableQueries = queries,
                    CalculatedColumns = new List<CalculatedColumnModel>(),
                    SelectDistinct = selectParser.Distinct,
                    Filters = new List<WhereGroup> { selectParser.GetWhere() }
                };
                queriesModelList.Add(queryModel);
            }
           
            var pagedQuery = new PagedQueryModel
            {
                Queries = queriesModelList,
                PagingModel = new PaginationModel
                {
                    PageNumber = 1,
                    PageSize = 10
                },
                ColorRules = new List<ColorRuleModel>()
            };

            var json = JsonConvert.SerializeObject(pagedQuery);
            var filePath = Path.GetDirectoryName(args[0]);
            var fileName = Path.GetFileNameWithoutExtension(args[0]) + "JsonFormat.txt";
            var fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                 File.Create(fullPath).Close();

            }
            File.WriteAllText(fullPath,json);
            Console.WriteLine($"All results are in file {fullPath}");
            Console.ReadKey();
        }
    }
}
