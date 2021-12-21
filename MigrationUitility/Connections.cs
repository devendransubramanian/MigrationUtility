using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
    public class Connections
    {
        public SourceConnection SourceConnection { get; set; }
        public DestinationConnection DestinationConnection { get; set; }
    }

    public class DestinationConnection
    {
        public string ConnectionName { get; set; }
        public string ServerName { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }

    public class SourceConnection
    {
        public string ConnectionName { get; set; }
        public string ServerName { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }
}
