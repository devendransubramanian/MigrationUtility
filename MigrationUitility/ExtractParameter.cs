using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
    public class ExtractParameter
    {
        public string Name { get; set; }
        public bool IsNull { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
    }
}
