using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
   public class ExtractTableSchemaInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string DataTypeMaxLength { get; set; }
        public string NumericPrecision { get; set; }
        public string NumericScale { get; set; }
        public List<ExtractTableSchemaInfo> ColumnSchemaInfoCollection { get; set; }
        public string ParentSchemaName { get; set; }
        public int InnerArrayCount { get; set; }
        public Guid fieldid { get; set; }

    }
}
