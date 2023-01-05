using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace MigrationUitility
{
    class SqlServerConnection
    {

        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public bool IntegratedSecurity { get; set; }
        public int Port { get; set; }
        public SqlConnection SqlConnection { get; set; }
        public SqlCommand SqlCommand { get; set; }
        public string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the configuration is SSL or not
        /// </summary>
        public bool IsEnableSSL { get; set; }

        public SqlServerConnection(Dictionary<string,object> sourceConnectionBuilder)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(sourceConnectionBuilder["ConnectionString"]?.ToString());
            if (sourceConnectionBuilder.ContainsKey("UserName") && !string.IsNullOrEmpty(sourceConnectionBuilder["UserName"]?.ToString()))
            {
                connectionStringBuilder.UserID = sourceConnectionBuilder["UserName"]?.ToString();
                connectionStringBuilder.Password = sourceConnectionBuilder["Password"]?.ToString();
                connectionStringBuilder.IntegratedSecurity = (bool)sourceConnectionBuilder["IntegratedSecurity"];
                connectionStringBuilder.Encrypt = (bool)sourceConnectionBuilder["IsEnableSSL"];
            }

            this.Server = connectionStringBuilder.DataSource;
            this.IntegratedSecurity = connectionStringBuilder.IntegratedSecurity;
            this.UserName = connectionStringBuilder.UserID;
            this.Password = connectionStringBuilder.Password;
            this.DatabaseName = connectionStringBuilder.InitialCatalog;
            this.IsEnableSSL = connectionStringBuilder.Encrypt;
            this.ConnectionString = connectionStringBuilder.ConnectionString;

        }

        internal List<TableSchema> GetAllSchema(string schema ="", string tablename="")
        {
            List<TableSchema> schematables = new List<TableSchema>();
            string query = GetAllSchemaTables();
            if (!string.IsNullOrEmpty(schema) && !string.IsNullOrEmpty(tablename))
            {
                query= string.Format("select schema_name(t.schema_id) as schema_name,t.name as table_name, t.create_date,t.modify_date from sys.tables t where schema_name(t.schema_id)='{0}' and t.name='{1}' order by table_name",schema.TrimStart('[').TrimEnd(']'), tablename.TrimStart('[').TrimEnd(']'));
            }             
                var com = GetSqlCommandInstance(query, CommandType.Text);
            using (SqlDataReader reader = SQLExecuteReader(com, this.SqlConnection))
            {
                while (reader.Read())
                {
                    TableSchema tables = new TableSchema();
                    tables.SchemaName = reader["schema_name"].ToString();
                    tables.TableName = reader["table_name"].ToString();
                    schematables.Add(tables);
                }
            }
            return schematables;
        }

        /// <summary>
        ///  Create and initailze the sql connection
        /// </summary>
        /// <returns>true or false</returns>
        public bool CreateCommandAndConnection()
        {
            if (this.SqlConnection == null)
            {
                this.SqlConnection = new SqlConnection(this.ConnectionString);
            }
            try
            {
                if (this.SqlConnection.State == ConnectionState.Closed)
                {
                    this.SqlConnection.Open();
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (this.SqlCommand == null)
            {
                this.SqlCommand = new SqlCommand() { Connection = this.SqlConnection };
            }
            return true;
        }
        public void Dispose()
        {
            if (this.SqlCommand != null)
            {
                this.SqlCommand.Dispose();
            }
            if (this.SqlConnection != null)
            {
                if (SqlConnection.State != ConnectionState.Closed || SqlConnection.State != ConnectionState.Broken)
                {
                    this.SqlConnection.Close();
                    this.SqlConnection.Dispose();
                }
            }
        }
        public Result GetTableSchema(string tableName, string schemaName)
        {
            var result = new Result();
            try
            {
                List<ExtractTableSchemaInfo> schemaDetails = new List<ExtractTableSchemaInfo>();
                var com = GetSqlCommandInstance(GetTableSchemaQuery(schemaName, tableName), CommandType.Text);
                using (SqlDataReader reader = SQLExecuteReader(com, this.SqlConnection))
                {
                    while (reader.Read())
                    {
                        schemaDetails.Add(new ExtractTableSchemaInfo
                        {
                            ColumnName = reader["COLUMN_NAME"].ToString(),
                            DataType = reader["DATA_TYPE"].ToString(),
                            DataTypeMaxLength = reader["CHARACTER_MAXIMUM_LENGTH"].ToString(),
                            NumericPrecision = reader["NUMERIC_PRECISION"].ToString(),
                            NumericScale = reader["NUMERIC_SCALE"].ToString(),                            
                           fieldid = Guid.NewGuid()
                        });
                    }
                }
                result.ReturnValue = schemaDetails;
                result.Status = true;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Exception = e;
            }
            return result;
        }


        /// <summary>
        ///  Get the Data information from Table
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="limit">Limit Value</param>
        /// <returns>Result Value</returns>
        public Result GetAllData(List<ExtractTableSchemaInfo> extractTableInfo,string sourceTablename, int? limit = null)
        {
            Result result = new Result();
            var dataTable = new DataTable();
            try
            {
                var command = GetSqlCommandInstance(GetSelectQuery(extractTableInfo, sourceTablename, limit), CommandType.Text);
                command.CommandTimeout = 600;
                using (SqlDataReader reader = SQLExecuteReader(command, this.SqlConnection))
                {
               
                    dataTable.Load(reader);
                   
                }
                result.ReturnValue = dataTable;
                result.Status = true;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Exception = e;
            }
            return result;
        }
        public string GetSelectQuery(List<ExtractTableSchemaInfo> ColumnSchemaInfoCollection, string sourceTablename,int? limit = null)
        {
            string query = string.Empty;
            foreach (var columnName in ColumnSchemaInfoCollection)
            {
                string column_name = columnName.ColumnName;
                if (Utilities.IsTypeCastingImage(columnName.DataType))
                {
                    column_name = " Convert(varbinary(MAX),[" + column_name + "]) as [" + column_name + "] ";
                }
                else
                {
                    column_name = " [" + column_name + "] ";
                }
                if (string.IsNullOrEmpty(query))
                {
                    query = string.Concat(column_name, ",");
                }
                else
                {
                    query = string.Concat(query, column_name, ",");
                }
            }

            if (limit != null)
            {
                query = string.Format(CultureInfo.InvariantCulture, "SELECT TOP({0}) {1} FROM {2}", limit, query.TrimEnd(','), sourceTablename);
            }
            else
            {
                query = string.Format(CultureInfo.InvariantCulture, "SELECT {0} FROM {1}", query.TrimEnd(','), sourceTablename);
            }
            return query;
        }

        public static SqlDataReader SQLExecuteReader(SqlCommand command, SqlConnection connection)
        {
            SqlDataReader reader = null;
         
                try
                {
                    reader = command.ExecuteReader();                    
                }
                catch (SqlException ex)
                {
                   
                }
            return reader;
        }

        /// <summary>
        /// Get the Sql server table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public string GetTableName(string tableName, string schemaName)
        {
            return "[" + schemaName + "].[" + tableName + "]";
        }
        /// <summary>
        /// Update and return sql command instance.
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="commandType">Command type</param>
        /// <returns>SqlCommand</returns>
        private SqlCommand GetSqlCommandInstance(string commandText, CommandType commandType)
        {
            this.SqlCommand.CommandText = commandText;
            this.SqlCommand.CommandType = commandType;
            return SqlCommand;
        }
        public string GetAllTablesQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "select * from INFORMATION_SCHEMA.TABLES WHERES TABLE_SCHEMA = '{0}'", schemaName);
        }

        public string GetAvailableSchemaQuery()
        {
            return "SELECT * FROM SYS.SCHEMAS";
        }
        public string GetAllSchemaTables()
        {
            return "select schema_name(t.schema_id) as schema_name,t.name as table_name, t.create_date,t.modify_date from sys.tables t order by table_name";
        }
        public string GetTableSchemaQuery(string schemaName, string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA='{1}'", ChangeTableNameFormat(tableName), schemaName);
        }

        private object ChangeTableNameFormat(string tableName)
        {
            return tableName.Replace("'", "''");
        }

        public string CheckSchemaExistsQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "IF EXISTS( SELECT* FROM SYS.SCHEMAS WHERE NAME ='{0}') SELECT 1 ELSE SELECT 0", schemaName);
        }

        public string CheckTableExistsQuery(string schemaName, string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}') SELECT 1 ELSE SELECT 0", tableName, schemaName);
        }

        public string CreateSchemaQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "CREATE SCHEMA \"{0}\"", schemaName);
        }

        public string CreateTableQuery(string tableName, string column)
        {
            return string.Format(CultureInfo.InvariantCulture, "CREATE TABLE {0} ({1});", tableName, column);
        }

        public string DropSchemaQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "DROP SCHEMA[{0}];", schemaName);
        }

        public string DropTableQuery(string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "DROP TABLE {0};", tableName);
        }

        public string GetTableSchemaQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}'", schemaName);
        }

        public string InsertTableQuery(string tableName, string values)
        {
            return string.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} VALUES ({1})", tableName, values);
        }
        /// <summary>
        ///  Get the primary key column for given table
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="schemaName">Schema Name</param>
        /// <returns>Result Value</returns>
        public Result GetPrimaryKey(string tableName, string schemaName)
        {
            var result = new Result();
            try
            {
                List<string> primaryKeyList = new List<string>();
                var com = GetSqlCommandInstance(string.Concat("USE [" + this.DatabaseName + "];", GetPrimaryKeyQuery(schemaName, tableName)), CommandType.Text);
                using (SqlDataReader reader = SQLExecuteReader(com, this.SqlConnection))
                {
                    while (reader.Read())
                    {
                        primaryKeyList.Add(reader["PRIMARYKEYCOLUMN"].ToString());
                    }
                }
                result.ReturnValue = primaryKeyList;
                result.Status = true;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Exception = e;
            }

            return result;
        }
 
        public string GetPrimaryKeyQuery(string schemaName, string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "SELECT KU.TABLE_NAME AS TABLENAME, COLUMN_NAME AS PRIMARYKEYCOLUMN FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
               "AS KU ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME AND KU.TABLE_NAME = '{0}' and TC.TABLE_SCHEMA='{1}' WHERE KU.TABLE_SCHEMA = '{1}' " +
               "ORDER BY KU.TABLE_NAME, KU.ORDINAL_POSITION", ChangeTableNameFormat(tableName), schemaName);
        }

    }
}
