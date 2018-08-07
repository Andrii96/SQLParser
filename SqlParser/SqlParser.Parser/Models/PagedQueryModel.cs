using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class PagedQueryModel
    {
        public List<QueryModel> Queries { get; set; }

        public PaginationModel PagingModel { get; set; }
    }
}
