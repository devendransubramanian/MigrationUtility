using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
    public class ExtractTableInfo
    {
        public string Tablename { get; set; }
        public ExtractTableSchemaInfo TableSchemaInformation { get; set; }
        public string LastModifiedDateColumn { get; set; }

        public List<string> PrimaryKey { get; set; }

        public object SourceConnectionProvider { get; set; }
        public string SourceTableName { get; set; }

        public string OperationType { get; set; }
        public List<ExtractParameter> Parameters { get; set; }
    }
}
