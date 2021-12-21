using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
    public class Utilities
    {
     

        /// <summary>
        /// Returns data type string alone from the given object
        /// Eg.returns Int32 from System.Int32
        /// </summary>
        internal static string GetDataTypeStr(object dataType)
        {
            return dataType.ToString().Split('.').ElementAt(1);
        }



        /// <summary>
        ///  get the typecasting for mysql
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns>true or false</returns>
        internal static bool IsTypeCastingChar(string datatype)
        {
            var supportedTypes = new string[] { ExtractManagedType.Set, ExtractManagedType.Enum };
            if (datatype == null)
            {
                return false;
            }

            return supportedTypes.Contains(datatype);
        }

        /// <summary>
        ///  get the type casting for sql server
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns>True or false</returns>
        internal static bool IsTypeCastingImage(string datatype)
        {
            var supportedTypes = new string[] { ExtractManagedType.Image };
            if (datatype == null)
            {
                return false;
            }

            return supportedTypes.Contains(datatype);
        }

        /// <summary>
        ///  get the unsupported datatypes for SQL server
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns>true or false</returns>
        internal static bool IsTypeSupported(string datatype)
        {
            var notSupportedTypes = new string[]
            {
            ExtractManagedType.Binary, ExtractManagedType.Geography, ExtractManagedType.Geometry, ExtractManagedType.Image, ExtractManagedType.VarBinary, ExtractManagedType.BFile, ExtractManagedType.Blob, ExtractManagedType.Clob, ExtractManagedType.NClob, ExtractManagedType.IntervalMonthToSecond, ExtractManagedType.IntervalYearToMonth, ExtractManagedType.URowId, ExtractManagedType.Polygon, ExtractManagedType.Circle,
            ExtractManagedType.Box, ExtractManagedType.LSeg, ExtractManagedType.Inet, ExtractManagedType.Cidr, ExtractManagedType.UUID, ExtractManagedType.MacAddress, ExtractManagedType.Path, ExtractManagedType.Line, ExtractManagedType.Interval, ExtractManagedType.Point, ExtractManagedType.LineString, ExtractManagedType.GeometryCollection, ExtractManagedType.MultiLineString, ExtractManagedType.MultiPoint, ExtractManagedType.MultiPolygon, ExtractManagedType.PgLsn,
            ExtractManagedType.TxidSnapshot, ExtractManagedType.BitVarying, ExtractManagedType.IntervalYearToMonth, ExtractManagedType.IntervalMonthToSecond, ExtractManagedType.Raw, ExtractManagedType.RowId, ExtractManagedType.LongRaw, ExtractManagedType.URowId, ExtractManagedType.TsVector, ExtractManagedType.TsQuery, ExtractManagedType.TsRange, ExtractManagedType.Array
            };
            if (datatype == null)
            {
                return false;
            }

            return !notSupportedTypes.Contains(datatype);
        }

        /// <summary>
        /// Returns true if data type is binary type
        /// </summary>
        internal static bool IsBinaryTypeData(string dataType)
        {
            var type = dataType.Split('(')[0].ToString().Trim();
            return type.ToLower().Equals("binary") || type.ToLower().Equals("varbinary");
        }

        /// <summary>
        /// It returns SQL data type name for given datatype.
        /// </summary>
        /// <param name="dataType">Datatype</param>
        /// <param name="shouldRestrictDataType">Boolean value to indicate restrict data type</param>
        /// <returns>Object</returns>
        private static string GetMySQLDataTypeName(string dataType, string datatype_max_len = null, string numeric_precision = null, string numeric_scale = null, bool shouldRestrictDataType = false)
        {
            if (shouldRestrictDataType)
            {
                return "varchar(max)";
            }

            switch (dataType.ToLower())
            {
                case ExtractManagedType.Bit:
                case ExtractManagedType.Boolean:
                case ExtractManagedType.Bool:
                    return "bit";
                case ExtractManagedType.Char:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "char";
                    }
                    else
                    {
                        return "char(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.VaryingCharacter:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "varchar";
                    }
                    else if (datatype_max_len == "-1")
                    {
                        return "varchar(MAX)";
                    }
                    else
                    {
                        return "varchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.NVaryingCharacter:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "nvarchar";
                    }
                    else if (datatype_max_len == "-1")
                    {
                        return "nvarchar(MAX)";
                    }
                    else
                    {
                        return "nvarchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.LongText:
                    return "nvarchar(max)";
                case ExtractManagedType.NChar:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "nchar";
                    }
                    else
                    {
                        return "nchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Binary:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "binary";
                    }
                    else
                    {
                        return "binary(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.VarBinary:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "varbinary";
                    }
                    else if (datatype_max_len == "-1")
                    {
                        return "varbinary(MAX)";
                    }
                    else
                    {
                        return "varbinary(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.BigInt:
                    return "bigint";
                case ExtractManagedType.Int:
                case ExtractManagedType.MediumInt:
                    return "int";
                case ExtractManagedType.SmallInt:
                    return "smallint";
                case ExtractManagedType.TinyInt:
                    return "tinyint";
                case ExtractManagedType.Geometry:
                case ExtractManagedType.Point:
                case ExtractManagedType.Polygon:
                case ExtractManagedType.Curve:
                case ExtractManagedType.MultiLineString:
                case ExtractManagedType.LineString:
                case ExtractManagedType.MultiPoint:
                case ExtractManagedType.MultiPolygon:
                case ExtractManagedType.GeometryCollection:
                case ExtractManagedType.TinyBlob:
                case ExtractManagedType.Blob:
                case ExtractManagedType.MediumBlob:
                case ExtractManagedType.LongBlob:
                case ExtractManagedType.GeomCollection:
                    return "varbinary(MAX)";
                case ExtractManagedType.Numeric:
                    if (string.IsNullOrEmpty(numeric_precision) || string.IsNullOrEmpty(numeric_scale))
                    {
                        return "numeric";
                    }
                    else
                    {
                        return "numeric(" + numeric_precision + "," + numeric_scale + ")";
                    }
                case ExtractManagedType.Decimal:
                    if (string.IsNullOrEmpty(numeric_precision) || string.IsNullOrEmpty(numeric_scale))
                    {
                        return "decimal";
                    }
                    else
                    {
                        return "decimal(" + numeric_precision + "," + numeric_scale + ")";
                    }

                case ExtractManagedType.Float:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "float";
                    }
                    else
                    {
                        return "float(24)";
                    }
                case ExtractManagedType.Real:
                case ExtractManagedType.Double:
                    return "float(53)";
                case ExtractManagedType.Date:
                    return "date";
                case ExtractManagedType.Timestamp:
                case ExtractManagedType.DateTime:
                    return "datetime2";
                case ExtractManagedType.Timespan:
                    return "smalldatetime";
                case ExtractManagedType.Year:
                    return "smallint";
                case ExtractManagedType.SmallDateTime:
                    return "smalldatetime";
                case ExtractManagedType.Time:
                    return "time";
                case ExtractManagedType.Set:
                case ExtractManagedType.Enum:
                    return "char(255)";
                case ExtractManagedType.TextValue:
                case ExtractManagedType.TinyText:
                case ExtractManagedType.MediumText:
                case ExtractManagedType.Text:
                case ExtractManagedType.Json:
                default:
                    return "varchar(max)";
            }
        }

        private static string GetPostgresSQLDataTypeName(string dataType, string datatype_max_len, string numeric_precision, string numeric_scale)
        {
            switch (dataType.ToLower())
            {
                case ExtractManagedType.Bit:
                case ExtractManagedType.Boolean:
                case ExtractManagedType.Bool:
                    return "bit";
                case ExtractManagedType.SmallInt:
                case ExtractManagedType.SmallSerial:
                case ExtractManagedType.Int16:
                    return "smallint";
                case ExtractManagedType.TimestampWithtimeZone:
                case ExtractManagedType.TimestampWithoutTimeZone:
                case ExtractManagedType.Timespan:
                    return "datetime";
                case ExtractManagedType.Date:
                    return "date";
                case ExtractManagedType.Time:
                case ExtractManagedType.TimeWithoutTimeZone:
                    return "time";
                case ExtractManagedType.TimeWithtimeZone:
                    return "datetimeoffset";
                case ExtractManagedType.Xml:
                    return "xml";
                case ExtractManagedType.UUID:
                case ExtractManagedType.Point:
                case ExtractManagedType.Line:
                case ExtractManagedType.LSeg:
                case ExtractManagedType.Box:
                case ExtractManagedType.Path:
                case ExtractManagedType.Polygon:
                case ExtractManagedType.Circle:
                case ExtractManagedType.Inet:
                case ExtractManagedType.Cidr:
                case ExtractManagedType.MacAddress:
                case ExtractManagedType.PgLsn:
                case ExtractManagedType.TxidSnapshot:
                case ExtractManagedType.Interval:
                case ExtractManagedType.BitVarying:
                    return "varchar(50)";
                case ExtractManagedType.BigInt:
                case ExtractManagedType.BigSerial:
                case ExtractManagedType.Int64:
                    return "bigint";
                case ExtractManagedType.Serial:
                case ExtractManagedType.Int:
                case ExtractManagedType.Int32:
                case ExtractManagedType.Integer:
                    return "int";
                case ExtractManagedType.Bytea:
                    if (string.IsNullOrEmpty(datatype_max_len) || datatype_max_len == "-1")
                    {
                        return "VARBINARY(max)";
                    }
                    else
                    {
                        return "VARBINARY(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Char:
                case ExtractManagedType.Character:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "nchar(MAX)";
                    }
                    else
                    {
                        return "nchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.CharacterVarying:
                    return string.Format(CultureInfo.InvariantCulture, "varchar({0})", string.IsNullOrEmpty(datatype_max_len) ? "256" : datatype_max_len);
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
                    return "money";
                case ExtractManagedType.DoublePrecision:
                case ExtractManagedType.Float:
                case ExtractManagedType.Real:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "float";
                    }
                    else
                    {
                        return "float(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.TsVector:
                case ExtractManagedType.TsQuery:
                case ExtractManagedType.TsRange:
                case ExtractManagedType.Array:
                case ExtractManagedType.Json:
                case ExtractManagedType.JsonBinary:
                case ExtractManagedType.VaryingCharacter:
                case ExtractManagedType.TextValue:
                default:
                    return "varchar(max)";
            }
        }

        private static string GetOracleDataTypeName(string dataType, string datatype_max_len, string numeric_precision, string numeric_scale)
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
                case ExtractManagedType.Varchar2:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "varchar";
                    }
                    else if (datatype_max_len == "-1")
                    {
                        return "varchar(MAX)";
                    }
                    else
                    {
                        return "varchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.NVarchar2:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "nvarchar";
                    }
                    else if (datatype_max_len == "-1")
                    {
                        return "nvarchar(MAX)";
                    }
                    else
                    {
                        return "nvarchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Blob:
                    return "varbinary(MAX)";
                case ExtractManagedType.NChar:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "nchar";
                    }
                    else
                    {
                        return "nchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Numeric:
                    if (string.IsNullOrEmpty(numeric_precision) || string.IsNullOrEmpty(numeric_scale))
                    {
                        return "numeric";
                    }
                    else
                    {
                        return "numeric(" + numeric_precision + "," + numeric_scale + ")";
                    }
                case ExtractManagedType.Number:
                    if (string.IsNullOrEmpty(numeric_precision) || string.IsNullOrEmpty(numeric_scale))
                    {
                        return "float";
                    }
                    else
                    {
                        return "numeric(" + numeric_precision + "," + numeric_scale + ")";
                    }
                case ExtractManagedType.Decimal:
                    if (string.IsNullOrEmpty(numeric_precision) || string.IsNullOrEmpty(numeric_scale))
                    {
                        return "decimal";
                    }
                    else
                    {
                        return "decimal(" + numeric_precision + "," + numeric_scale + ")";
                    }
                case ExtractManagedType.Dec:
                    if (string.IsNullOrEmpty(numeric_precision) || string.IsNullOrEmpty(numeric_scale))
                    {
                        return "dec";
                    }
                    else
                    {
                        return "dec(" + numeric_precision + "," + numeric_scale + ")";
                    }
                case ExtractManagedType.Float:
                    return "float";
                case ExtractManagedType.Integer:
                    return "integer";
                case ExtractManagedType.Int:
                    return "numeric(38)";
                case ExtractManagedType.SmallInt:
                    return "smallint";
                case ExtractManagedType.Real:
                case ExtractManagedType.BinaryDouble:
                case ExtractManagedType.BinaryFloat:
                    return "float";
                case ExtractManagedType.RowId:
                case ExtractManagedType.URowId:
                case ExtractManagedType.Raw:
                    return "varchar(50)";
                case ExtractManagedType.Timespan:
                case ExtractManagedType.Timestamp2:
                case ExtractManagedType.Timestamp2WithtimeZone:
                    return "datetime";
                case ExtractManagedType.TimestampWithtimeZone:
                case ExtractManagedType.TimestampWithLocalTimeZone:
                case ExtractManagedType.Timestamp2WithLocalTimeZone:
                    return "varchar(37)";
                case ExtractManagedType.Date:
                    return "date";
                case ExtractManagedType.XmlType:
                    return "xml";
                case ExtractManagedType.Long:
                case ExtractManagedType.IntervalMonthToSecond:
                case ExtractManagedType.IntervalYearToMonth:
                case ExtractManagedType.LongRaw:
                case ExtractManagedType.BFile:
                case ExtractManagedType.Clob:
                case ExtractManagedType.NClob:
                default:
                    return "varchar(max)";
            }
        }

        private static string GetSqlServerDataTypeName(string dataType, string datatype_max_len, string numeric_precision, string numeric_scale)
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
                case ExtractManagedType.VaryingCharacter:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "varchar";
                    }
                    else if (datatype_max_len == "-1" || uint.Parse(datatype_max_len) >= int.MaxValue)
                    {
                        return "varchar(MAX)";
                    }
                    else
                    {
                        return "varchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.NVaryingCharacter:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "nvarchar";
                    }
                    else if (datatype_max_len == "-1" || uint.Parse(datatype_max_len) >= int.MaxValue)
                    {
                        return "nvarchar(MAX)";
                    }
                    else
                    {
                        return "nvarchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Text:
                    return "text";
                case ExtractManagedType.NText:
                    return "ntext";
                case ExtractManagedType.NChar:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "nchar";
                    }
                    else
                    {
                        return "nchar(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Binary:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "binary";
                    }
                    else
                    {
                        return "binary(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.VarBinary:
                    if (string.IsNullOrEmpty(datatype_max_len))
                    {
                        return "varbinary";
                    }
                    else if (datatype_max_len == "-1" || uint.Parse(datatype_max_len) >= int.MaxValue)
                    {
                        return "varbinary(MAX)";
                    }
                    else
                    {
                        return "varbinary(" + datatype_max_len + ")";
                    }
                case ExtractManagedType.Image:
                    return "varbinary(MAX)";
                case ExtractManagedType.BigInt:
                    return "bigint";
                case ExtractManagedType.Int:
                    return "int";
                case ExtractManagedType.SmallInt:
                    return "smallint";
                case ExtractManagedType.TinyInt:
                    return "tinyint";
                case ExtractManagedType.Bit:
                    return "bit";
                case ExtractManagedType.Xml:
                    return "XML";
                case ExtractManagedType.UniqueIdentifier:
                    return "uniqueidentifier";
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
                    return "money";
                case ExtractManagedType.SmallMoney:
                    return "smallmoney";
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
                    return "date";
                case ExtractManagedType.DateTime:
                    return "datetime";
                case ExtractManagedType.DateTime + "2":
                    return "datetime2";
                case ExtractManagedType.DateTimeOffset:
                    return "datetimeoffset";
                case ExtractManagedType.SmallDateTime:
                    return "smalldatetime";
                case ExtractManagedType.Time:
                    return "time";
            }
            return dataType;
        }
    }
}
