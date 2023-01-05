using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
    public class PostgresSQLConnection
    {
        public PostgresSQLConnection(Dictionary<string, object> destinationConnectionBuilder)
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder(destinationConnectionBuilder["ConnectionString"]?.ToString());
            if (destinationConnectionBuilder.ContainsKey("Server") && !string.IsNullOrEmpty(destinationConnectionBuilder["Server"]?.ToString()))
            {
                connectionStringBuilder.Host = destinationConnectionBuilder["Server"]?.ToString();
                connectionStringBuilder.Database = destinationConnectionBuilder["Database"]?.ToString();
                connectionStringBuilder.Username = destinationConnectionBuilder["UserName"]?.ToString();    
                connectionStringBuilder.Password = destinationConnectionBuilder["Password"]?.ToString();
            }

            this.Server = connectionStringBuilder.Host;
            this.IntegratedSecurity = connectionStringBuilder.IntegratedSecurity;
            this.UserName = connectionStringBuilder.Username;
            this.Password = connectionStringBuilder.Password;
            this.DatabaseName = connectionStringBuilder.Database;
            this.ConnectionString = connectionStringBuilder.ConnectionString;
            this.ParentSchemaName = destinationConnectionBuilder.ContainsKey("ParentSchemaName") ? destinationConnectionBuilder["ParentSchemaName"]?.ToString() : null;
        }

        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string ConnectionString { get; set; }
        public string ParentSchemaName { get; set; }
        public int Port { get; set; }
        public bool IgnoreTableDelete { get; set; }
        public NpgsqlConnection ExtractConnection { get; set; }
        public NpgsqlCommand ExtractCommand { get; set; }
        public NpgsqlTransaction Transaction { get; set; }
        private const string FieldDescriptionQuery = "comment on column \"{0}\".\"{1}\".\"{2}\" is '{3}';";
        public bool CommitTransaction()
        {
            try
            {
                this.Transaction.Commit();
                this.ExtractConnection.Close();
            }
            catch (Exception)
            {
            }
            return true;
        }

        public bool CreateCommandAndConnection()
        {
            if (this.ExtractConnection == null)
            {
                this.ExtractConnection = new NpgsqlConnection(this.ConnectionString);
            }
            try
            {
                if (this.ExtractConnection.State == ConnectionState.Closed)
                {
                    this.ExtractConnection.Close();
                    this.ExtractConnection.Open();
                }
            }
            catch (Exception ex)
            {
            }
            if (this.Transaction == null)
            {
                this.Transaction = this.ExtractConnection.BeginTransaction();
            }
            if (this.ExtractCommand == null || this.ExtractCommand.Transaction == null)
            {
                this.ExtractCommand = new NpgsqlCommand() { Connection = this.ExtractConnection, Transaction = this.Transaction };
            }

            return true;
        }

        public Result CreateIndividualTableInTargetServer(ExtractTableInfo tables, string schemaname)
        {
            var result = new Result();
            try
            {
               
                    var schemaInfo = tables.TableSchemaInfo;
                    var tableName = tables.TableName;
                    var primaryKey = tables.PrimaryKey;

                    if (schemaInfo == null)
                    {
                        throw new ArgumentNullException();
                    }
                    if (schemaInfo.ColumnSchemaInfoCollection.Count > 0)
                    {
                        if (DoesTableExists(schemaInfo, tableName))
                        {
                            RemoveTable(schemaInfo, tableName);
                        }
                       
                            CreateSchemaInTargetServer(schemaname);
                    
                        var queryString = GenerateCreateTableQuery(schemaInfo, tableName, primaryKey);
                        var command = GetSqlCommandInstance(queryString, CommandType.Text);
                command.ExecuteNonQuery();
                        result.Status = true;
                    }
               
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Exception = ex;
            }

            return result;
        }

        public Result CreateSchemaInTargetServer(string schemaname)
        {
            Result result = new Result();
            try
            {
         
                var command = GetSqlCommandInstance(CreateSchemaQuery(schemaname), CommandType.Text);
                 command.ExecuteNonQuery();
                result.Status = true;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Exception = e;
            }
            return result;
        }
        public string CreateSchemaQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "CREATE SCHEMA IF NOT EXISTS \"{0}\"", schemaName);
        }


        public Result DeleteSchemaInTargetServer()
        {
            Result result = new Result();
            try
            {
                if (DoesSchemaExists(ParentSchemaName))
                {
                    RemoveSchemaFromDatabase(ParentSchemaName);
                }

                result.Status = true;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Exception = e;
                throw;
            }
            return result;
        }

        public void Dispose()
        {
            DisposeConnectionObjects();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Result GetProperties()
        {
            var result = new Result();
            var connectionBuilder = new Dictionary<string, object>();
            connectionBuilder.Add("ConnectionString", this.ConnectionString);
            connectionBuilder.Add("ParentSchemaName", this.ParentSchemaName);
            result.ReturnValue = connectionBuilder;
            return result;
        }
        private DataTable DataTableAdapter(DataTable dataTable, ExtractTableSchemaInfo tableSchema)
        {
            DataTable cloned = dataTable.Clone();
            int index = 0;
            foreach (DataColumn dc in cloned.Columns)
            {
                if (dc.DataType == typeof(TimeSpan))
                {
                    dc.DataType = typeof(string);
                }
                if (tableSchema.ColumnSchemaInfoCollection[index].DataType.ToUpper().Contains("VARCHAR") && dc.DataType == typeof(Byte[]))
                {
                    dc.DataType = typeof(string);
                }
                if (tableSchema.ColumnSchemaInfoCollection[index].DataType.ToUpper().Contains("TIME") && dc.DataType == typeof(String))
                {
                    dc.DataType = typeof(TimeSpan);
                }
                if (tableSchema.ColumnSchemaInfoCollection[index].DataType.ToUpper().Contains("TINYINT") && dc.DataType == typeof(Byte))
                {
                    dc.DataType = typeof(int);
                }
                if (tableSchema.ColumnSchemaInfoCollection[index].DataType.ToUpper().Contains("BOOL") && dc.DataType == typeof(UInt64))
                {
                    dc.DataType = typeof(bool);
                }
                index++;
            }

            foreach (DataRow dr in dataTable.Rows)
            {
                cloned.ImportRow(dr);
            }
            DataTable table = cloned;

            for (int dataRow = 0; dataRow < table.Rows.Count; dataRow++)
            {
                for (int tableColumns = 0; tableColumns < tableSchema.ColumnSchemaInfoCollection.Count; tableColumns++)
                {
                    bool nullResult = false;
                    foreach (DataColumn column in table.Columns)
                    {
                        column.ReadOnly = false;
                        if (tableSchema.ColumnSchemaInfoCollection[tableColumns].ColumnName.Equals(column.ColumnName.Trim()))
                        {
                            column.ReadOnly = false;
                            var cell = table.Rows[dataRow][column.ColumnName];
                            bool result = cell.ToString().All(char.IsLetterOrDigit);
                            if (Convert.ToString(cell) == null || string.IsNullOrEmpty(cell.ToString()))
                            {
                                nullResult = true;
                            }
                            double nan = 0.0;
                            if (cell.ToString() != "System.Byte[]")
                            {
                                if (cell.ToString() != "System.Byte")
                                {
                                    cell = ConvertCsvPercentageData(cell.ToString());
                                }
                            }
                            bool nanResult = false;
                            if (double.TryParse(cell.ToString(), out nan))
                            {
                                nanResult = double.IsNaN(nan);
                            }
                            DateTime dt;
                            if (cell is string && !result && DateTime.TryParse(cell.ToString(), out dt) && tableSchema != null && (tableSchema.ColumnSchemaInfoCollection[tableColumns].DataType.ToUpper() == "DATETIME" || tableSchema.ColumnSchemaInfoCollection[tableColumns].DataType.ToUpper() == "DATETIME2" || tableSchema.ColumnSchemaInfoCollection[tableColumns].DataType.ToUpper() == "DATE" || column.DataType.Name.ToUpper().Contains("DATETIMEOFFSET")))
                            {
                                if (column.DataType.Name.ToUpper().Contains("DATETIMEOFFSET"))
                                {
                                    var cellValue = cell.ToString().Split(' ');
                                    if (cellValue.Length >= 3)
                                    {
                                        string datetimeoffset = GetValidSQLDateTimeValue(DateTime.Parse(cellValue[0].ToString())).ToString("yyyy-MM-dd") + " " + cellValue[1].ToString() + " " + cellValue[2].ToString();

                                        table.Rows[dataRow][column.ColumnName] = DateTimeOffset.Parse(datetimeoffset, CultureInfo.InvariantCulture);
                                        break;
                                    }
                                }
                                dt = DateTime.Parse(GetValidSQLDateTimeValue(dt).ToString());
                                table.Rows[dataRow][column.ColumnName] = dt.ToString("yyyy-MM-dd HH:mm:ss.sss", CultureInfo.InvariantCulture);
                                break;
                            }
                            else
                            {
                                if (cell is DateTime)
                                {
                                    cell = DateTime.Parse(GetValidSQLDateTimeValue((DateTime)cell).ToString());
                                }

                                table.Rows[dataRow][column.ColumnName] = nullResult ? DBNull.Value : nanResult ? DBNull.Value : cell;
                                break;
                            }
                        }
                    }

                }
            }

            return table;
        }
        /// <summary>
        /// If dateformat is lesser that SQL lower dateformat. Here changed to mininum SQL dateformat
        /// </summary>
        /// <param name="dateTime">input datetime</param>
        /// <returns></returns>
        private DateTime GetValidSQLDateTimeValue(DateTime dt)
        {
            // Checked Min and Max date range accepted by SQL. If not in valid range, assigned with SqlDateTime.MinValue
            if (!((dt < new DateTime(9999, 12, 31)) && (dt > new DateTime(1753, 1, 1))))
            {
                dt = DateTime.Parse(System.Data.SqlTypes.SqlDateTime.MinValue.ToString());
            }
            return dt;
        }

        /// <summary>
        /// Double data value contains % means remove the percentage and divide the value by 100
        /// </summary>
        /// <param name="cellValue">Value need to be change</param>
        /// <returns></returns>
        private string ConvertCsvPercentageData(string cellValue)
        {
            if (cellValue.Contains("%"))
            {
                double value = 0.0;
                cellValue = cellValue.Replace("%", "");
                if (double.TryParse(cellValue, out value))
                {
                    value = value / 100;
                }
                cellValue = value.ToString();
            }
            return cellValue;
        }

        public Result InsertDataRows(DataTable dataTable, string sourceConnectionType, string tableName, ExtractTableSchemaInfo tableSchema)
        {
            var insertTableValueResult = new Result();
            try
            {
               CreateCommandAndConnection();
                dataTable = DataTableAdapter(dataTable, tableSchema);
                //// Here 2096 denotes maximum number of SQL parameters limit. 
                int numberOfInsertStatements = 2096 / tableSchema.ColumnSchemaInfoCollection.Count;
                int pageSize = dataTable.Rows.Count / numberOfInsertStatements;

                int pageCount = 0;
                int dataRowCount = 0;
                int numberOfStatementCondition = pageSize == 0 ? dataTable.Rows.Count : numberOfInsertStatements;
                if (dataTable.Rows.Count != 0)
                {
                    do
                    {
                        string insertQuery = string.Empty;
                        this.ExtractCommand.Parameters.Clear();
                        insertQuery = "INSERT INTO" + "\"" + tableSchema.ParentSchemaName+ "\".\"" + tableName + "\" VALUES";
                        for (int dataRow = dataRowCount; dataRow < numberOfStatementCondition; dataRow++)
                        {
                            insertQuery = insertQuery + "\n" + GenerateInsertTableQueryForParameters(tableName, tableSchema, dataRow);
                            InsertQueryWithParameters(tableSchema, sourceConnectionType, dataTable, dataRow);
                            dataRowCount++;
                            insertQuery = insertQuery + ",";
                        }
                        if (!string.IsNullOrEmpty(insertQuery) && !insertQuery.EndsWith("VALUES"))
                        {
                            this.ExtractCommand.CommandText = insertQuery.TrimEnd(',');
                            this.ExtractCommand.CommandType = CommandType.Text;
                            this.ExtractCommand.CommandTimeout = 300;
                            this.ExtractCommand.ExecuteNonQuery();
                        }
                        pageCount++;
                        numberOfStatementCondition = pageSize == 0 ? dataTable.Rows.Count : (pageCount + 1) * numberOfInsertStatements < dataTable.Rows.Count ? (pageCount + 1) * numberOfInsertStatements : dataTable.Rows.Count;
                    }
                    while (pageCount <= pageSize);

                   CommitTransaction();
                }
                insertTableValueResult.Status = true;
            }
            catch (Exception ex)
            {
                insertTableValueResult.Status = false;
                insertTableValueResult.Exception = ex;
            }
            finally
            {
              CommitTransaction();
            }

            return insertTableValueResult;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public bool TruncateTable(string tableName)
        {
            var command = GetSqlCommandInstance(TruncateTableQuery(tableName), CommandType.Text);
            try
            {
                CreateCommandAndConnection();
                command.ExecuteNonQuery();
                CommitTransaction();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string TruncateTableQuery(string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "TRUNCATE TABLE {0};", tableName);
        }


        /// <summary>
        /// Checks data whether need to insert or update in target table.
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="value">Container contains both column key and value</param>
        /// <returns>bool</returns>
        public bool IsInsertData(string tableName, Dictionary<string, string> value)
        {
            string wherequery = GenerateWhereQuery(value);
            var connection = this.ExtractConnection;
            string query = TotalRowCountQuery(tableName);
            if (!string.IsNullOrEmpty(wherequery))
            {
                wherequery = " where " + wherequery;
            }

            var getDataCount = GetSqlCommandInstance(query + wherequery, CommandType.Text);
            object valueIfExist = getDataCount.ExecuteScalar();

            if (int.Parse(valueIfExist.ToString()) > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public string TotalRowCountQuery(string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "SELECT COUNT(*) FROM {0}", tableName);
        }



        /// <summary>
        /// Remove table from target database connection.
        /// </summary>
        /// <param name="connection">Sql connection details</param>
        /// <param name="tableSchema">Table schema information</param>
        /// <param name="tableName">Table name</param>
        /// <returns>bool</returns>
        public bool RemoveTable(ExtractTableSchemaInfo tableSchema, string tableName = null)
        {
            tableName = tableName == null ? null : "\"" + tableSchema.ParentSchemaName + "\".\"" + tableName + "\"";
            string currentName = tableName ?? "\"" + tableSchema.ParentSchemaName + "\".\"" + tableSchema.ColumnName + "\"";
            var command = GetSqlCommandInstance(DropTableQuery(currentName), CommandType.Text);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string DropTableQuery(string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "DROP TABLE {0};", tableName);
        }

        /// <summary>
        ///  Get the last Modified datetime for destination
        /// </summary>
        /// <param name="destinationTableName">Table name</param>
        /// <param name="lastModifiedColumn">LastModified</param>
        /// <returns>Last Modified DateTime Value</returns>
       


        public void UpdateData(string destinationTableName, Dictionary<string, string> primaryvalues, Dictionary<string, string> updaterows)
        {
            CreateCommandAndConnection();
            string wherequery = GenerateWhereQuery(primaryvalues);
            var sourceConnection = this.ExtractConnection;
            var updatequery = GenerateUpdateQuery(updaterows);
            var com = GetSqlCommandInstance("update " + destinationTableName + " set " + updatequery + " where " + wherequery, CommandType.Text);
            com.ExecuteNonQuery();
            CommitTransaction();
        }

        /// <summary>
        /// Update and return postgresql command instance.
        /// </summary>
        /// <param name="connection">Sql connection</param>
        /// <param name="commandText">Command text</param>
        /// <param name="commandType">Command type</param>
        /// <returns>NpgsqlCommand</returns>
        private NpgsqlCommand GetSqlCommandInstance(string commandText, CommandType commandType)
        {
            this.ExtractCommand.CommandText = commandText;
            this.ExtractCommand.CommandType = commandType;
            this.ExtractCommand.CommandTimeout = 3000;
            return ExtractCommand;
        }

        /// <summary>
        /// Checks table if exists in target database connection.
        /// </summary>
        /// <param name="connection">Sql connection</param>
        /// <param name="tableSchema">Table schema information</param>
        /// <param name="tableName">Table name</param>
        /// <returns>bool</returns>
        private bool DoesTableExists(ExtractTableSchemaInfo tableSchema, string tableName = null)
        {
            string currentName = tableName ?? tableSchema?.ColumnName;
            bool result = false;
            if (this.ExtractConnection.State == ConnectionState.Closed)
            {
                this.ExtractConnection.Open();
            }
            var dataCheck = GetSqlCommandInstance(CheckTableExistsQuery(tableSchema.ParentSchemaName, currentName), CommandType.Text);
            try
            {
                int x = Convert.ToInt32(dataCheck.ExecuteScalar(), CultureInfo.InvariantCulture);
                return result = x == 1;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public string CreateTableQuery(string tableName, string column)
        {
            return string.Format(CultureInfo.InvariantCulture, "CREATE TABLE {0} ({1});", tableName, column);
        }
        public string CheckTableExistsQuery(string schemaName, string tableName)
        {
            return string.Format(CultureInfo.InvariantCulture, "SELECT CASE WHEN EXISTS(SELECT table_schema, table_name FROM information_schema.tables WHERE table_schema = '{0}' AND table_name = '{1}') THEN 1 ELSE 0 END", schemaName, tableName);
        }

        /// <summary>
        ///  Get the destination Table Name
        /// </summary>
        /// <param name="schemaName">Schema Name</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetDestinationName(string schemaName, string tableName)
        {
            return string.IsNullOrEmpty(schemaName) ? tableName : tableName + "_" + schemaName;
        }


        /// <summary>
        /// Generate create table query.
        /// </summary>
        /// <param name="tableSchema">Table schema information</param>
        /// <param name="tableName">Table name</param>
        /// <param name="shouldRestrictDataType">Boolean value to indicate restrict data type</param>
        /// <returns>Create table query</returns>
        private string GenerateCreateTableQuery(ExtractTableSchemaInfo tableSchema, string tableName = null, List<string> primaryKey = null, bool shouldRestrictDataType = false)
        {
            string createTableQuery = string.Empty;
            string descriptionQuery = string.Empty;
           string currentTable = tableName == null ? null : "\"" + tableSchema.ParentSchemaName + "\"." + "\"" + tableName + "\"";
       //     string currentTable = "\"" + tableSchema.ParentSchemaName + "\".\"" + tableName + "\"";
            createTableQuery = CreateTableQuery(currentTable, string.Join(", ",
                tableSchema.ColumnSchemaInfoCollection.Select(column => string.Format(CultureInfo.InvariantCulture,
                "\"{0}\" {1}", column.fieldid, GetPostgreSqlDataTypeName(column.DataType,  column.DataTypeMaxLength, column.NumericPrecision, column.NumericScale)))));

            foreach (var field in tableSchema.ColumnSchemaInfoCollection)
            {
                descriptionQuery += string.Format(FieldDescriptionQuery, tableSchema.ParentSchemaName, tableName, field.fieldid, field.ColumnName.Replace("'","''"));
                descriptionQuery += "\n";
            }
           Dictionary<string,string> primarkeyfieldid= Getprimarkeyfieldid(tableSchema.ColumnSchemaInfoCollection,primaryKey);
            if (primaryKey != null && primaryKey.Count > 0)
            {
                bool isConstaintAdded = false;
                createTableQuery = createTableQuery.Remove(createTableQuery.Length - 2);
                if (primaryKey.Count == 1)
                {
                    createTableQuery += ",PRIMARY KEY (\"" + primarkeyfieldid[primaryKey[0]] + "\")";
                }
                else if (primaryKey.Count != 1 && isConstaintAdded == false)
                {
                    var constraintKey = tableName.Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty);
                    createTableQuery += ",CONSTRAINT \"PK_" + constraintKey + "\" PRIMARY KEY (";
                    isConstaintAdded = true;
                }
                if (isConstaintAdded == true)
                {
                    for (var i = 0; i < primaryKey.Count; i++)
                    {
                        createTableQuery += "\"" + primarkeyfieldid[primaryKey[i]] + "\",";
                    }
                    createTableQuery = createTableQuery.Remove(createTableQuery.Length - 1);
                    createTableQuery += ")";
                }
                createTableQuery += ");";
            }
            createTableQuery += ";\n" + descriptionQuery;
            return createTableQuery;
        }

        private Dictionary<string, string> Getprimarkeyfieldid(List<ExtractTableSchemaInfo> columnSchemaInfoCollection,List<string> primarykey)
        {
            Dictionary<string, string> primaryKeylist = new Dictionary<string, string>();
            foreach(var pk in primarykey)
            {
                for(int i=0;i< columnSchemaInfoCollection.Count;i++)
                {
                    if(columnSchemaInfoCollection[i].ColumnName.Trim()==pk.ToString().Trim())
                    {
                        primaryKeylist.Add(pk.ToString(), columnSchemaInfoCollection[i].fieldid.ToString());
                    }
                }
            }
            return primaryKeylist;
        }

        /// <summary>
        ///  Get the schema Name is exist or not in given database
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="schemaName">Schema Name</param>
        /// <returns>Bool value</returns>
        private bool DoesSchemaExists(string schemaName)
        {
            bool result = false;
            var dataCheck = GetSqlCommandInstance(CheckSchemaExistsQuery(schemaName), CommandType.Text);
            try
            {
                int x = Convert.ToInt32(dataCheck.ExecuteScalar());
                    return result = x == 1;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public string CheckSchemaExistsQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "SELECT CASE WHEN EXISTS(SELECT table_schema FROM information_schema.tables WHERE table_schema = '{0}' AND table_catalog='{1}') THEN 1 ELSE 0 END", schemaName,this.DatabaseName);
        }

        /// <summary>
        ///  Remove the schema name from database
        /// </summary>
        /// <param name="sqlConnection">Sql Connection</param>
        /// <param name="schemaName">Schema Name</param>
        private void RemoveSchemaFromDatabase(string schemaName)
        {
            DataTable table = new DataTable();
            List<string> tableNames = new List<string>();
            var command = GetSqlCommandInstance(GetTableSchemaQuery(schemaName), CommandType.Text);
            try
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);
                adapter.Fill(table);
                foreach (DataRow item in table.Rows)
                {
                    tableNames.Add(item.ItemArray[0].ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
            tableNames.ForEach(name =>
            {
                RemoveTable(new ExtractTableSchemaInfo() { ColumnName = name, ParentSchemaName = schemaName });
            });
            var removeCommand = GetSqlCommandInstance(DropSchemaQuery(schemaName), CommandType.Text);
            try
            {
                removeCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (this.ExtractCommand != null)
                {
                    this.ExtractCommand.Dispose();
                }
            }
        }
        public string DropSchemaQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "DROP SCHEMA \"{0}\";", schemaName);
        }

        public string GetTableSchemaQuery(string schemaName)
        {
            return string.Format(CultureInfo.InvariantCulture, "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}'", schemaName);
        }


        /// <summary>
        /// Dispose connection objects.
        /// </summary>
        private void DisposeConnectionObjects()
        {
            if (this.ExtractCommand != null)
            {
                this.ExtractCommand.Dispose();
            }
            if (this.ExtractConnection != null)
            {
                if (this.ExtractCommand != null)
                {
                    this.Transaction.Dispose();
                }
                if (ExtractConnection.State != ConnectionState.Closed || ExtractConnection.State != ConnectionState.Broken)
                {
                    this.ExtractConnection.Close();
                    this.ExtractConnection.Dispose();
                }
            }
        }

        /// <summary>
        /// To generate query to insert values in Sql server. In future we need to merge this method with generate insert table query. 
        /// </summary>
        /// <param name="tableName">Name of table.</param>
        /// <param name="schemaInfo">Schema information</param>
        /// <param name="limit">Limit value.</param>
        /// <returns>Returns insert query.</returns>
        private string GenerateInsertTableQueryForParameters(string tableName, ExtractTableSchemaInfo schemaInfo, int limit)
        {
            StringBuilder insertQuery = new StringBuilder();
            for (int i = 0; i < schemaInfo.ColumnSchemaInfoCollection.Count; i++)
            {
                insertQuery.Append("@C" + limit + "to" + i);
                if (i != schemaInfo.ColumnSchemaInfoCollection.Count - 1)
                {
                    insertQuery.Append(",");
                }
            }
            return InsertTableQuery("\"" + this.ParentSchemaName + "\".\"" + tableName + "\"", insertQuery.ToString());
        }
        public string InsertTableQuery(string tableName, string values)
        {
            return string.Format(CultureInfo.InvariantCulture, "({0})", values);
        }


        /// <summary>
        ///  Get the Insert query with parameters Values
        /// </summary>
        /// <param name="tableSchema">Schema Information</param>
        /// <param name="table">Table</param>
        /// <param name="tableName">Table name</param>
        /// <param name="dataRow">Data Row</param>
        private void InsertQueryWithParameters(ExtractTableSchemaInfo tableSchema, string sourceConnectionType, DataTable table, int dataRow)
        {
            for (int tableColumns = 0; tableColumns < tableSchema.ColumnSchemaInfoCollection.Count; tableColumns++)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (tableSchema.ColumnSchemaInfoCollection[tableColumns].fieldid.ToString().Trim().Equals(column.ColumnName.Trim()))
                    {
                        var cell = table.Rows[dataRow][column.ColumnName];
                        bool result = cell.ToString().All(char.IsLetterOrDigit);
                        bool nullResult = string.IsNullOrEmpty(Convert.ToString(cell));
                        double nan = 0.0;
                        bool nanResult = false;
                        if (double.TryParse(cell.ToString(), out nan))
                        {
                            nanResult = double.IsNaN(nan);
                        }
                        DateTime dt;
                        var columnDataType = GetPostgreSqlDataTypeName(tableSchema.ColumnSchemaInfoCollection[tableColumns].DataType, tableSchema.ColumnSchemaInfoCollection[tableColumns].DataTypeMaxLength.ToString(), tableSchema.ColumnSchemaInfoCollection[tableColumns].NumericPrecision.ToString(), tableSchema.ColumnSchemaInfoCollection[tableColumns].NumericScale.ToString());
                        var npgSqlType = GetNpgSqlDbType(columnDataType);
                        if(tableSchema.ColumnSchemaInfoCollection[tableColumns].DataType.ToUpper().Contains("DATETIMEOFFSET") )
                        {
                            npgSqlType = NpgsqlDbType.TimestampTz;
                        }
                        if (cell is string && !result && DateTime.TryParse(cell.ToString(), out dt) && tableSchema != null && (columnDataType == ExtractManagedType.DateTime || columnDataType == ExtractManagedType.Date || columnDataType == ExtractManagedType.Timestamp))
                        {
                            this.ExtractCommand.Parameters.AddWithValue("@c" + dataRow + "to" + tableColumns, npgSqlType, dt.ToString("yyyy-MM-dd HH:mm:ss.sss", CultureInfo.InvariantCulture));
                            break;
                        }
                        else
                        {
                            this.ExtractCommand.Parameters.AddWithValue("@c" + dataRow + "to" + tableColumns, npgSqlType, nullResult ? DBNull.Value : nanResult ? DBNull.Value : cell);
                            break;
                        }
                    }
                }
            }
        }

        private NpgsqlDbType GetNpgSqlDbType(string columnDataType)
        {
            columnDataType = columnDataType.Split('(').FirstOrDefault();
            switch (columnDataType.ToLower())
            {
                case "int64":
                case "bigint":
                    return NpgsqlDbType.Bigint;
                case "bool":
                case "boolean":
                case "bit":
                    return NpgsqlDbType.Boolean;
                case "int":
                case "int4":
                case "int8":
                case "int16":
                case "int32":
                    return NpgsqlDbType.Integer;
                case "date":
                    return NpgsqlDbType.Date;
                case "timestamp":
                    return NpgsqlDbType.Timestamp;
                case "box":
                    return NpgsqlDbType.Box;
                case "circle":
                    return NpgsqlDbType.Circle;
                case "char":
                    return NpgsqlDbType.Char;
                case "bytea":
                    return NpgsqlDbType.Bytea;
             
                case "double":               
                    return NpgsqlDbType.Double;
                case "decimal":
                case "float":
                    return NpgsqlDbType.Numeric;
                case "line":
                    return NpgsqlDbType.Line;
                case "lseg":
                    return NpgsqlDbType.LSeg;
                case "money":
                    return NpgsqlDbType.Money;
                case "numeric":
                    return NpgsqlDbType.Numeric;
                case "path":
                    return NpgsqlDbType.Path;
                case "point":
                    return NpgsqlDbType.Point;
                case "polygon":
                    return NpgsqlDbType.Polygon;
                case "real":
                    return NpgsqlDbType.Real;
                case "smallint":
                    return NpgsqlDbType.Smallint;
                case "text":
                    return NpgsqlDbType.Text;
                case "time":
                    return NpgsqlDbType.Time;
                case "timestamp with time zone":
                    return NpgsqlDbType.TimestampTZ;
                case "time with time zone":
                    return NpgsqlDbType.TimeTZ;
                case "xml":
                    return NpgsqlDbType.Xml;
                case "uuid":
                    return NpgsqlDbType.Uuid;
                case "varchar":
                default:
                    return NpgsqlDbType.Varchar;
            }
        }

        /// <summary>
        /// Generates where query. 
        /// </summary>
        /// <param name="data">Container contains both column name and column value.</param>
        /// <returns>where query</returns>
        private string GenerateWhereQuery(Dictionary<string, string> data)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Keys.Count; i++)
            {
                string key = data.Keys.ElementAt(i);
                builder.Append(string.Format(CultureInfo.InvariantCulture, "{0} = {1}", key, data[key]));
                if (i != data.Keys.Count - 1)
                {
                    builder.Append(" AND ");
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Generates update query for target SQL server connection.
        /// </summary>
        /// <param name="data">Contains both key and value</param>
        /// <returns>Update Query</returns>
        private string GenerateUpdateQuery(Dictionary<string, string> data)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Keys.Count; i++)
            {
                string key = data.Keys.ElementAt(i);
                builder.Append(string.Format(CultureInfo.InvariantCulture, "{0} = N'{1}'", key, data[key]));
                if (i != data.Keys.Count - 1)
                {
                    builder.Append(", ");
                }
            }
            return builder.ToString();
        }
         /// <summary>


        private static string GetPostgreSqlDataTypeName(string dataType, string datatype_max_len, string numeric_precision, string numeric_scale)
        {
            switch (dataType.ToLower())
            {
                case ExtractManagedType.Char:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "char";
                    }
                    else
                    {
                        return "char(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.BigInt:
                    return "bigint";
                case ExtractManagedType.Boolean:
                case ExtractManagedType.Bool:
                    return "boolean";
                case ExtractManagedType.Int:
                    return "int";
                case ExtractManagedType.TinyInt:
                case ExtractManagedType.SmallInt:
                    return "smallint";
                case ExtractManagedType.Bit:
                    return "boolean";
                case ExtractManagedType.Xml:
                    return "XML";
                case ExtractManagedType.Numeric:
                    if (string.IsNullOrEmpty(numeric_precision) && string.IsNullOrEmpty(numeric_scale))
                    {
                        return "numeric";
                    }
                    else
                    {
                        return "numeric(" + numeric_precision + "," + numeric_scale + ")";
                    }
                case ExtractManagedType.Decimal:
                    if (string.IsNullOrEmpty(numeric_precision) && string.IsNullOrEmpty(numeric_scale))
                    {
                        return "decimal";
                    }
                    else
                    {
                        return "decimal(" + numeric_precision + "," + numeric_scale + ")";
                    }
                case ExtractManagedType.Money:
                case ExtractManagedType.SmallMoney:
                    return "money";
                case ExtractManagedType.Float:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "float";
                    }
                    else
                    {
                        return "float(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Real:
                    return "real";
                case ExtractManagedType.Date:
                case ExtractManagedType.DateTime:
                case ExtractManagedType.DateTime + "2":
                case ExtractManagedType.DateTimeOffset:
                case ExtractManagedType.SmallDateTime:
                    return "timestamp";
                case ExtractManagedType.Time:
                    return "time";
                case ExtractManagedType.NText:
                case ExtractManagedType.Text:
                case "text":
                    return "text";

                case ExtractManagedType.Binary:
                case ExtractManagedType.VarBinary:
                case ExtractManagedType.Image:
                case ExtractManagedType.Timestamp:
                    return "bytea";
                case ExtractManagedType.UniqueIdentifier:
                    return "uuid";
                case ExtractManagedType.VaryingCharacter:
                case ExtractManagedType.NVaryingCharacter:
                case ExtractManagedType.NChar:
             
            
                default:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "varchar";
                    }
                    else if (datatype_max_len == "-1" || uint.Parse(datatype_max_len) >= int.MaxValue)
                    {
                        return "varchar";
                    }
                    else
                    {
                        return "varchar(" + datatype_max_len + ")";
                    }
            }
        }


    }
}
