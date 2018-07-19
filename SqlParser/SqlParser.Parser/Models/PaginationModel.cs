using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class PaginationModel
    {
        public long PageNumber { get; set; }

        public long PageSize { get; set; }
    }
}
