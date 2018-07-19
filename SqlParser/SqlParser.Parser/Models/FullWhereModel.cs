using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class FullWhereModel
    {
        public WhereGroup Group { get; set; }
        public FullWhereModel Parent { get; set; }

       
    }
}
