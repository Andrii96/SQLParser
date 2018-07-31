using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser.Parser.Models
{
    public class FullJoinModel
    {
        public string FromTable { get; set; }
        public JoinModel Join { get; set; }
    }
}
