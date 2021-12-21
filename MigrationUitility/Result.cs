using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
    public class Result
    {
        public Result()
        {
            DataTable = new DataTable();
        }

        public bool Status
        {
            get;
            set;
        }

        public DataTable DataTable
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }

        public object ReturnValue
        {
            get;
            set;
        }
    }
}
