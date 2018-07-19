using Newtonsoft.Json;
using SqlParser.Parser;
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
            var command = new SqlCommand(sqlScript);
            var sqlQueryParser = new SqlQueryParser(command);
            var queries = sqlQueryParser.ParseToQueryTableModelList();
            var pagedQuery = new PagedQueryModel
            {
                Query = new QueryModel
                {
                    TableQueries = queries,
                    CalculatedColumns = new List<CalculatedColumnModel>(),
                    ColorRules = new List<ColorRuleModel>(),
                    SelectDistinct = command.Distinct,
                },
                PagingModel = new PaginationModel
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            var json = JsonConvert.SerializeObject(pagedQuery);
            var filePath = Path.GetDirectoryName(args[0]);
            var fileName = Path.GetFileNameWithoutExtension(args[0]) + "PagedQueryModel.txt";
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
