using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Npgsql;

namespace MigrationUitility
{
    class Program
    {
        static void Main(string[] args)
        {
            string databaseFile = AppDomain.CurrentDomain.BaseDirectory + "\\SQLServerDatabasesList.txt";
            StreamReader databaseReader = new StreamReader(databaseFile);
            String DatabaseName = "";
            if (!IsTextFileEmpty(databaseFile))
            {
               

                        StreamWriter exported = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ExportedList.txt", true);
                        StreamWriter exportFail = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\ExportFailed.txt", true);
                        Console.WriteLine("Exporeted List in " + AppDomain.CurrentDomain.BaseDirectory + "\\ExportedList.txt");
                        Console.WriteLine("Export Failed List in " + AppDomain.CurrentDomain.BaseDirectory + "\\ExportFailed.txt");
                       
                        #region ExportDatabase
                        Connections connectioninfo = FetchConnectionInfo();
                DatabaseName = connectioninfo.DestinationConnection.Database;
                //create Database
                if (CreateDatabase(connectioninfo, DatabaseName))
                        {
                            Dictionary<string, object> sourceConnection = new Dictionary<string, object>();

                            sourceConnection.Add("ConnectionString", "Server=" + connectioninfo.SourceConnection.ServerName + "," + connectioninfo.SourceConnection.Port + ";Database=" + connectioninfo.SourceConnection.Database + ";User Id=" + connectioninfo.SourceConnection.UserName + ";Password=" + connectioninfo.SourceConnection.Password + ";");
                            sourceConnection.Add("ConnectionName", connectioninfo.SourceConnection.ConnectionName);
                            sourceConnection.Add("ServerName", connectioninfo.SourceConnection.ServerName);
                            sourceConnection.Add("Port", connectioninfo.SourceConnection.Port);
                            sourceConnection.Add("UserName", connectioninfo.SourceConnection.UserName);
                            sourceConnection.Add("IntegratedSecurity", false);
                            sourceConnection.Add("IsEnableSSL", false);
                            sourceConnection.Add("Password", connectioninfo.SourceConnection.Password);
                            sourceConnection.Add("Database", DatabaseName);
                            Dictionary<string, object> destinationConnection = new Dictionary<string, object>();
                            destinationConnection.Add("ConnectionString", "User ID=" + connectioninfo.DestinationConnection.UserName + ";Password=" + connectioninfo.DestinationConnection.Password + ";Server=" + connectioninfo.DestinationConnection.ServerName + ";Port=" + connectioninfo.DestinationConnection.Port + ";Database=" + connectioninfo.SourceConnection.Database.ToLower()+ ";");
                            destinationConnection.Add("IntegratedSecurity", false);
                            destinationConnection.Add("IsEnableSSL", false);
                            destinationConnection.Add("ConnectionName", connectioninfo.DestinationConnection.ConnectionName);
                            destinationConnection.Add("Server", connectioninfo.DestinationConnection.ServerName);
                            destinationConnection.Add("Port", connectioninfo.DestinationConnection.Port);
                            destinationConnection.Add("UserName", connectioninfo.DestinationConnection.UserName);
                            destinationConnection.Add("Password", connectioninfo.DestinationConnection.Password);
                            destinationConnection.Add("Database", DatabaseName.ToLower());
                            SqlServerConnection sqlServerConnection = new SqlServerConnection(sourceConnection);

                            PostgresSQLConnection postgreSQLConnection = new PostgresSQLConnection(destinationConnection);

                            sqlServerConnection.CreateCommandAndConnection();
                            postgreSQLConnection.CreateCommandAndConnection();
                            List<ExtractTableInfo> tableList = new List<ExtractTableInfo>();
                            var response = new Result();
                            List<TableSchema> getAllSchema = new List<TableSchema>();
                           
                                getAllSchema = sqlServerConnection.GetAllSchema();
                         
                            var tablelist = new ExtractTableInfo();                       
                            foreach (var table in getAllSchema)
                            {

                                try
                                {
                                    response = sqlServerConnection.GetTableSchema(table.TableName, table.SchemaName);
                                    if (response.Status)
                                    {
                                        var tableSchema = new ExtractTableSchemaInfo
                                        {
                                            ColumnSchemaInfoCollection = (List<ExtractTableSchemaInfo>)response.ReturnValue,
                                            ParentSchemaName = table.SchemaName
                                        };

                                        var primaryKeyResult = sqlServerConnection.GetPrimaryKey(table.TableName, table.SchemaName);
                                        if (primaryKeyResult.Status)
                                        {

                                            tablelist.TableName = table.TableName;
                                            tablelist.TableSchemaInfo = tableSchema;
                                            tablelist.SourceTableName = table.TableName;
                                            tablelist.PrimaryKey = (List<string>)primaryKeyResult.ReturnValue;
                                        }
                                        else
                                        {
                                            if (!response.Status)
                                            {
                                                Console.WriteLine(response.Exception.Message);
                                            }
                                            else if (!primaryKeyResult.Status)
                                            {
                                                Console.WriteLine(response.Exception.Message);
                                            }
                                        }
                                    }

                                    response = postgreSQLConnection.CreateIndividualTableInTargetServer(tablelist, tablelist.TableSchemaInfo.ParentSchemaName);
                                    if (response.Status)
                                    {
                                        List<ExtractTableSchemaInfo> extractTableSchemaInfo = (List<ExtractTableSchemaInfo>)sqlServerConnection.GetTableSchema(tablelist.SourceTableName, tablelist.TableSchemaInfo.ParentSchemaName).ReturnValue;

                                        DataTable datatable = (DataTable)sqlServerConnection.GetAllData(extractTableSchemaInfo, "[" + tablelist.TableSchemaInfo.ParentSchemaName + "]" + "." + "[" + tablelist.SourceTableName + "]").ReturnValue;
                                        for (int i = 0; i < tablelist.TableSchemaInfo.ColumnSchemaInfoCollection.Count; i++)
                                        {
                                            datatable.Columns[i].ColumnName = tablelist.TableSchemaInfo.ColumnSchemaInfoCollection[i].fieldid.ToString();
                                            datatable.AcceptChanges();
                                        }
                                        postgreSQLConnection.CommitTransaction();
                                        Result resultinsert = postgreSQLConnection.InsertDataRows(datatable, "SQLServer", tablelist.TableName, tablelist.TableSchemaInfo);
                                        if (resultinsert.Status)
                                        {
                                            Console.WriteLine("Exported "+ "[" + DatabaseName + "]"+ "[" + tablelist.TableSchemaInfo.ParentSchemaName + "]" + "." + "[" + tablelist.SourceTableName + "]");
                                            exported.WriteLine("["+DatabaseName+"]"+"[" + tablelist.TableSchemaInfo.ParentSchemaName + "]" + "." + "[" + tablelist.SourceTableName + "]");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Insertion Failed "+ "[" + DatabaseName + "]" + "[" + tablelist.TableSchemaInfo.ParentSchemaName + "]" + "." + "[" + tablelist.SourceTableName + "]");
                                            exportFail.WriteLine("[" + DatabaseName + "]" + "[" + tablelist.TableSchemaInfo.ParentSchemaName + "]" + "." + "[" + tablelist.SourceTableName + "]" + resultinsert.Exception.Message);
                                            postgreSQLConnection.CommitTransaction();
                                            postgreSQLConnection.CreateCommandAndConnection();
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Table Creaation Failed "+ "[" + DatabaseName + "]." + "[" + tablelist.TableSchemaInfo.ParentSchemaName + "]" + "." + "[" + tablelist.SourceTableName + "]" + response.Exception.Message);
                                        exportFail.WriteLine("Table creation Failed "+ "[" + DatabaseName + "]."  + "[" + tablelist.TableSchemaInfo.ParentSchemaName + "]" + "." + "[" + tablelist.SourceTableName + "]" + response.Exception.Message);
                                        postgreSQLConnection.CommitTransaction();
                                        postgreSQLConnection.CreateCommandAndConnection();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message + "[" + DatabaseName + "]" + " " + table.TableName + " " + table.SchemaName);
                                }

                            }
                            exportFail.Close();
                            exported.Close();
                            sqlServerConnection.Dispose();
                            postgreSQLConnection.Dispose();
                            #endregion
                        }
                    
            }
            Console.WriteLine("Completed");
            Console.ReadKey();
        }

        private static bool CreateDatabase(Connections connectioninfo,string databasename)
        {
            string  sql = "", port = "", username = "", password = "";
            NpgsqlConnection conn = new NpgsqlConnection();
            NpgsqlCommand command = new NpgsqlCommand();

            port = connectioninfo.DestinationConnection.Port;
            username = connectioninfo.DestinationConnection.UserName;
            password = connectioninfo.DestinationConnection.Password;
            sql = "CREATE DATABASE \"" + databasename + "\" " +
                       "WITH OWNER = " + username + " " +
                       "CONNECTION LIMIT = -1;";
            try
            {
                bool isDatabaseExist = false;
                if (conn.State == ConnectionState.Closed)
                {
                    string conString = "Server=" + connectioninfo.DestinationConnection.ServerName + " ;Port=" + port + ";User Id=" + username + ";Password=" + password + ";";
                    conn.ConnectionString = conString;
                    conn.Open();
                }
                command.CommandText = "SELECT 1 AS result FROM pg_database WHERE datname = '" + databasename.ToLower() +"'";
                command.Connection = conn;
                
                        int result  = Convert.ToInt32(command.ExecuteScalar());
                if (result == 1)
                {
                    isDatabaseExist = true;
                    conn.Close();
                    return true;
                }
                else
                {
                    command.CommandText = sql;
                    command.Connection = conn;

                     result =command.ExecuteNonQuery();
                    return true;
                }
                    
            
                conn.Close();
           return false;
               
            }
            catch(Exception ex)
            {
                Console.WriteLine("Database creation failed:" + databasename + " "+ex.Message);
                conn.Close();

                return false;
            }



        }

        public static bool IsTextFileEmpty(string fileName)
        {
            var info = new FileInfo(fileName);
            if (info.Length == 0)
                return true;

            // only if your use case can involve files with 1 or a few bytes of content.
            if (info.Length < 6)
            {
                var content = File.ReadAllText(fileName);
                return content.Length == 0;
            }
            return false;
        }
        static Connections FetchConnectionInfo()
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Connections));
                System.IO.StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\Connection.xml");
                Connections connectionInfo = (Connections)reader.Deserialize(file);
                file.Close();
                return connectionInfo;
            }

          static  string[] GetAllTables(SqlConnection connection)
            {
                List<string> result = new List<string>();
                SqlCommand cmd = new SqlCommand("SELECT name FROM sys.Tables", connection);
                System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    result.Add(reader["name"].ToString());
                return result.ToArray();
            }
        }
    }

