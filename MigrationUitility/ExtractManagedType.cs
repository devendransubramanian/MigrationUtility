using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUitility
{
    /// <summary>
    /// Data types of all datasources
    /// </summary>
    public sealed class ExtractManagedType
    {
        /// <summary>
        /// Binary field used to store Byte[] data type value as string.
        /// </summary>
        public const string Byte = "byte[]";

        /// <summary>
        /// Bit field used to store Boolean data type value as string.
        /// </summary>
        public const string Bit = "bit";

        /// <summary>
        /// Blob field used to store BLOB data type value as string.
        /// </summary>
        public const string Blob = "blob";

        /// <summary>
        /// Counter field used to store COUNTER data type value as string.
        /// </summary>
        public const string Counter = "counter";

        /// <summary>
        /// CurrencyType2 field used to store CURRENCY data type value as string.
        /// </summary>
        public const string Currency = "currency";

        /// <summary>
        /// Dash field used to store Dash data type value as string.
        /// </summary>
        public const string Dash = "dash";

        /// <summary>
        /// Dot field used to store Dot data type value as string.
        /// </summary>
        public const string Dot = "dot";

        /// <summary>
        /// DotDash field used to store DotDash data type value as string.
        /// </summary>
        public const string DotDash = "dotdash";

        /// <summary>
        /// Date field used to store date data type value as string.
        /// </summary>
        public const string Date = "date";

        /// <summary>
        /// DateTime field used to store DateTime data type value as string.
        /// </summary>
        public const string DateTime = "datetime";

        /// <summary>
        /// Date Time Offset field used to store date time off set data type value as string.
        /// </summary>
        public const string DateTimeOffset = "datetimeoffset";

        /// <summary>
        /// DBDate field used to store DBDate data type value as string.
        /// </summary>
        public const string DBDate = "dbdate";

        /// <summary>
        /// Decimal field used to store Decimal data type value as string.
        /// </summary>
        public const string Decimal = "decimal";

        /// <summary>
        /// Double field used to store Double data type value as string.
        /// </summary>
        public const string Double = "double";

        /// <summary>
        /// Float field used to store float data type value as string.
        /// </summary>
        public const string Float = "float";

        /// <summary>
        /// Horizontal field used to store Horizontal data type value as string.
        /// </summary>
        public const string Horizontal = "horizontal";

        /// <summary>
        /// Int field used to store int data type value as string.
        /// </summary>
        public const string Int = "int";

        /// <summary>
        /// Int16 field used to store Int16 data type value as string.
        /// </summary>
        public const string Int16 = "int16";

        /// <summary>
        /// Int32 field used to store Int32 data type value as string.
        /// </summary>
        public const string Int32 = "int32";

        /// <summary>
        /// Int64 field used to store Int64 data type value as string.
        /// </summary>
        public const string Int64 = "int64";

        /// <summary>
        /// Integer field used to store Integer data type value as string.
        /// </summary>
        public const string Integer = "integer";

        /// <summary>
        /// IsEmpty field used to store (Blanks) data type value as string.
        /// </summary>
        public const string IsEmpty = "(blanks)";

        /// <summary>
        /// IsNull field used to store (Null) data type value as string.
        /// </summary>
        public const string IsNull = "(null)";

        /// <summary>
        /// Long field used to store Long data type value as string.
        /// </summary>
        public const string Long = "long";

        /// <summary>
        /// LongDash field used to store LongDash data type value as string.
        /// </summary>
        public const string LongDash = "longdash";

        /// <summary>
        /// LongDashDotDot field used to store LongDashDotDot data type value as string.
        /// </summary>
        public const string LongDashDotDot = "longdashdotdot";

        /// <summary>
        /// Numeric field used to store numeric data type value as string.
        /// </summary>
        public const string Numeric = "numeric";

        /// <summary>
        ///  Number field used to store numeric data type value as string.
        /// </summary>
        public const string Number = "number";

        /// <summary>
        /// NVaryingCharacter field used to store nvarchar data type value as string.
        /// </summary>
        public const string NVaryingCharacter = "nvarchar";

        /// <summary>
        /// Object field used to store object data type value as string.
        /// </summary>
        public const string Object = "object";

        /// <summary>
        /// PrimaryAxis field used to store PrimaryAxis data type value as string.
        /// </summary>
        public const string PrimaryAxis = "primaryaxis";

        /// <summary>
        /// Real field used to store real data type value as string.
        /// </summary>
        public const string Real = "real";

        /// <summary>
        /// SecondaryAxis field used to store SecondaryAxis data type value as string.
        /// </summary>
        public const string SecondaryAxis = "secondaryaxis";

        /// <summary>
        /// Short field used to store Short data type value as string.
        /// </summary>
        public const string Short = "short";

        /// <summary>
        /// Single field used to store Byte data type value as string.
        /// </summary>
        public const string Single = "byte";

        /// <summary>
        /// SmallInt field used to store smallint data type value as string.
        /// </summary>
        public const string SmallInt = "smallint";

        /// <summary>
        /// SmallMoney field used to store small money data type value as string.
        /// </summary>
        public const string SmallMoney = "smallmoney";

        /// <summary>
        /// String field used to store string data type value as string.
        /// </summary>
        public const string String = "string";

        /// <summary>
        /// Text field used to store String data type value as string.
        /// </summary>
        public const string Text = "String";

        /// <summary>
        /// Time field used to store time data type value as string.
        /// </summary>
        public const string Time = "time";

        /// <summary>
        /// Timestamp field used to store TimeSpan data type value as string.
        /// </summary>
        public const string Timespan = "timespan";

        /// <summary>
        /// UniqueColumnName field used to store Syncfusion_Expression_Unique_Name data type value as string.
        /// </summary>
        public const string UniqueColumnName = "syncfusion_expression_unique_name";

        /// <summary>
        /// UniqueIdentifier field used to store Guid data type value as string.
        /// </summary>
        public const string UniqueIdentifier = "uniqueidentifier";

        /// <summary>
        /// UnsignedInt field used to store UInt32 data type value as string.
        /// </summary>
        public const string UnsignedInt = "uint32";

        /// <summary>
        /// UnsignedInt64 field used to store UInt64 data type value as string.
        /// </summary>
        public const string UnsignedInt64 = "uint64";

        /// <summary>
        /// UnsignedShort field used to store UInt16 data type value as string.
        /// </summary>
        public const string UnsignedShort = "uint16";

        /// <summary>
        /// VaryingCharacter field used to store varchar data type value as string.
        /// </summary>
        public const string VaryingCharacter = "varchar";

        /// <summary>
        /// Vertical field used to store Vertical data type value as string.
        /// </summary>
        public const string Vertical = "vertical";

        /// <summary>
        /// BigBinary field used to store BIGBINARY data type value as string.
        /// </summary>
        internal const string BigBinary = "bigbinary";

        /// <summary>
        /// BigInt field used to store bigint data type value as string.
        /// </summary>
        internal const string BigInt = "bigint";

        /// <summary>
        /// Char field used to store char data type value as string.
        /// </summary>
        internal const string Char = "char";

        /// <summary>
        /// DBTime field used to store DBTime data type value as string.
        /// </summary>
        internal const string DBTime = "dbtime";

        /// <summary>
        /// Geography field used to store geography data type value as string.
        /// </summary>
        internal const string Geography = "geography";

        /// <summary>
        /// Geometry field used to store geometry data type value as string.
        /// </summary>
        internal const string Geometry = "geometry";

        /// <summary>
        /// HierarchyId field used to store hierarchy id data type value as string.
        /// </summary>
        internal const string HierarchyId = "hierarchyid";

        /// <summary>
        /// Image field used to store image data type value as string.
        /// </summary>
        internal const string Image = "image";

        /// <summary>
        /// LongBinary field used to store LONGBINARY data type value as string.
        /// </summary>
        internal const string LongBinary = "longbinary";

        /// <summary>
        /// LongBlob field used to store LONGBLOB data type value as string.
        /// </summary>
        internal const string LongBlob = "longblob";

        /// <summary>
        /// LongText field used to store LONGTEXT data type value as string.
        /// </summary>
        internal const string LongText = "longtext";

        /// <summary>
        /// LongVarBinary field used to store LongVarBinary data type value as string.
        /// </summary>
        internal const string LongVarBinary = "longvarbinary";

        /// <summary>
        /// MediumBlob field used to store MEDIUMBLOB data type value as string.
        /// </summary>
        internal const string MediumBlob = "mediumblob";

        /// <summary>
        /// Memo field used to store Memo data type value as string.
        /// </summary>
        internal const string Memo = "memo";

        /// <summary>
        /// Money field used to store money data type value as string.
        /// </summary>
        internal const string Money = "money";

        /// <summary>
        /// NChar field used to store nchar data type value as string.
        /// </summary>
        internal const string NChar = "nchar";

        /// <summary>
        /// NText field used to store ntext data type value as string.
        /// </summary>
        internal const string NText = "ntext";

        /// <summary>
        /// Object1 field used to store Object data type value as string.
        /// </summary>
        internal const string Object1 = "object";

        /// <summary>
        /// Ole field used to store Ole data type value as string.
        /// </summary>
        internal const string Ole = "ole";

        /// <summary>
        /// SingleType field used to store Single data type value as string.
        /// </summary>
        internal const string SingleType = "single";

        /// <summary>
        /// SmallDateTime field used to store small date time data type value as string.
        /// </summary>
        internal const string SmallDateTime = "smalldatetime";

        /// <summary>
        /// SQLVariant field used to store SQL_variant data type value as string.
        /// </summary>
        internal const string SqlVariant = "sql_variant";

        /// <summary>
        /// TinyBlob field used to store TINYBLOB data type value as string.
        /// </summary>
        internal const string TinyBlob = "tinyblob";

        /// <summary>
        /// TinyInt field used to store tinyint data type value as string.
        /// </summary>
        internal const string TinyInt = "tinyint";

        /// <summary>
        /// UnsignedBigInt field used to store UnsignedBigInt data type value as string.
        /// </summary>
        internal const string UnsignedBigInt = "unsignedbigint";

        /// <summary>
        /// UnsignedSmallInt field used to store UnsignedSmallInt data type value as string.
        /// </summary>
        internal const string UnsignedSmallInt = "unsignedsmallint";

        /// <summary>
        /// UnsignedTinyInt field used to store UnsignedTinyInt data type value as string.
        /// </summary>
        internal const string UnsignedTinyInt = "unsignedtinyint";

        /// <summary>
        /// VarBinary field used to store varbinary data type value as string.
        /// </summary>
        internal const string VarBinary = "varbinary";

        /// <summary>
        /// VarNumeric field used to store VarNumeric data type value as string.
        /// </summary>
        internal const string VarNumeric = "varnumeric";

        /// <summary>
        /// WChar field used to store WChar data type value as string.
        /// </summary>
        internal const string WChar = "wchar";

        /// <summary>
        /// Xml field used to store xml data type value as string.
        /// </summary>
        internal const string Xml = "xml";

        /// <summary>
        /// Text field 
        /// </summary>
        internal const string TextValue = "text";

        /// <summary>
        /// Double precision to store double with precision type
        /// </summary>
        internal const string DoublePrecision = "double precision";

        /// <summary>
        /// Bytea to store bytea values
        /// </summary>
        internal const string Bytea = "bytea";

        /// <summary>
        /// Bit to store Boolean data type
        /// </summary>
        internal const string Boolean = "boolean";

        /// <summary>
        /// Varchar2 to store character values
        /// </summary>
        internal const string Varchar2 = "varchar2";

        /// <summary>
        /// Clob to store long text
        /// </summary>
        internal const string Clob = "clob";

        /// <summary>
        /// NVarchar2 to store nvarchar datatype
        /// </summary>
        internal const string NVarchar2 = "nvarchar2";

        /// <summary>
        /// Binary float to store float values
        /// </summary>
        internal const string BinaryFloat = "binary_float";

        /// <summary>
        /// Binary double to store double datatype
        /// </summary>
        internal const string BinaryDouble = "binary_double";

        /// <summary>
        /// Character varying to store char,varchar datatypes
        /// </summary>
        internal const string CharacterVarying = "character varying";

        /// <summary>
        /// Timestamp without time zone data type
        /// </summary>
        internal const string TimestampWithoutTimeZone = "timestamp without time zone";

        /// <summary>
        /// Timestamp with time zone data type
        /// </summary>
        internal const string TimestampWithtimeZone = "timestamp with time zone";

        /// <summary>
        /// Timestamp with time zone data type
        /// </summary>
        internal const string Timestamp2WithtimeZone = "timestamp(6) with time zone";

        /// <summary>
        /// Timestamp datatype to store timestamp values
        /// </summary>
        internal const string Timestamp = "timestamp";

        /// <summary>
        /// Timestamp datatype to store timestamp with precision 6
        /// </summary>
        internal const string Timestamp2 = "timestamp(6)";

        /// <summary>
        /// year datatype to store year values
        /// </summary>
        internal const string Year = "year";

        /// <summary>
        /// year datatype to store year values
        /// </summary>
        internal const string UUID = "uuid";

        /// <summary>
        /// Bit field used to store Boolean data type value as string.
        /// </summary>
        internal const string Bool = "bool";

        /// <summary>
        /// Float  field used to store Boolean data type value as string.
        /// </summary>
        internal const string Fixed = "fixed";

        /// <summary>
        /// Int field used to store Boolean data type value as string.
        /// </summary>
        internal const string MediumInt = "mediumint";

        /// <summary>
        /// Text field used to store Tiny Text data type value as string.
        /// </summary>
        internal const string TinyText = "tinytext";

        /// <summary>
        /// Text field used to store medium Text data type value as string.
        /// </summary>
        internal const string MediumText = "mediumtext";

        /// <summary>
        /// Bit field used to store Binary data type value as string.
        /// </summary>
        internal const string Binary = "binary";

        /// <summary>
        /// Timestamp with local time zone data type
        /// </summary>
        internal const string TimestampWithLocalTimeZone = "timestamp with local time zone";

        /// <summary>
        /// Timestamp(6) with local time zone data type
        /// </summary>
        internal const string Timestamp2WithLocalTimeZone = "timestamp(6) with local time zone";

        /// <summary>
        /// Geometric field used to store point data type value as string.
        /// </summary>
        internal const string Point = "point";

        /// <summary>
        /// Geometric field used to store line data type value as string.
        /// </summary>
        internal const string Line = "line";

        /// <summary>
        /// Geometric field used to store lseg data type value as string.
        /// </summary>
        internal const string LSeg = "lseg";

        /// <summary>
        /// Geometric field used to store Box data type value as string.
        /// </summary>
        internal const string Box = "box";

        /// <summary>
        /// Geometric field used to store Path data type value as string.
        /// </summary>
        internal const string Path = "path";

        /// <summary>
        /// Geometric field used to store Binary data type value as string.
        /// </summary>
        internal const string Polygon = "polygon";

        /// <summary>
        /// Bit field used to store Binary data type value as string.
        /// </summary>
        internal const string Circle = "circle";

        /// <summary>
        /// Network field used to store INET data type value as string.
        /// </summary>
        internal const string Inet = "inet";

        /// <summary>
        /// Network field used to store CIDR data type value as string.
        /// </summary>
        internal const string Cidr = "cidr";

        /// <summary>
        /// Network field used to store MACADDR data type value as string.
        /// </summary>
        internal const string MacAddress = "macaddr";

        /// <summary>
        /// TsQuery field used to store TsQuery data type value as string.
        /// </summary>
        internal const string TsQuery = "tsquery";

        /// <summary>
        /// TsVector field used to store TsVector data type value as string.
        /// </summary>
        internal const string TsVector = "tsvector";

        /// <summary>
        /// TsRange field used to store TsRange data type value as string.
        /// </summary>
        internal const string TsRange = "tsrange";

        /// <summary>
        /// Array field used to store Array data type value as string.
        /// </summary>
        internal const string Array = "ARRAY";

        /// <summary>
        /// postgresql log sequence field used to store pg_lsn data type value as string.
        /// </summary>
        internal const string PgLsn = "pg_lsn";

        /// <summary>
        /// json field used to store json data type value as string.
        /// </summary>
        internal const string Json = "json";

        /// <summary>
        /// json binary sequence field used to store jsonb data type value as string.
        /// </summary>
        internal const string JsonBinary = "jsonb";

        /// <summary>
        /// serial field used to store serial data type value as string.
        /// </summary>
        internal const string Serial = "serial";

        /// <summary>
        /// serial field used to store bigserial data type value as string.
        /// </summary>
        internal const string BigSerial = "bigserial";

        /// <summary>
        /// serial field used to store smallserial data type value as string.
        /// </summary>
        internal const string SmallSerial = "smallserial";

        /// <summary>
        /// Time without time zone data type
        /// </summary>
        internal const string TimeWithoutTimeZone = "time without time zone";

        /// <summary>
        /// Time with time zone data type
        /// </summary>
        internal const string TimeWithtimeZone = "time with time zone";

        /// <summary>
        /// Interval data type
        /// </summary>
        internal const string Interval = "interval";

        /// <summary>
        /// Bit varing with time bit data type
        /// </summary>
        internal const string BitVarying = "bit varying";

        /// <summary>
        /// interval with year to month data type
        /// </summary>
        internal const string IntervalYearToMonth = "interval year(2) to month";

        /// <summary>
        /// Interval with Month  to Second data type
        /// </summary>
        internal const string IntervalMonthToSecond = "interval day(2) to second(6)";

        /// <summary>
        /// Raw data type in oracle
        /// </summary>
        internal const string Raw = "raw";

        /// <summary>
        /// Raw with long data type
        /// </summary>
        internal const string LongRaw = "long raw";

        /// <summary>
        /// RowId data type in oracle
        /// </summary>
        internal const string RowId = "rowid";

        /// <summary>
        /// unsigned RowId data type in oracle
        /// </summary>
        internal const string URowId = "urowid";

        /// <summary>
        /// Bfile  data type in oracle
        /// </summary>
        internal const string BFile = "bfile";

        /// <summary>
        /// Nclob data type in oracle
        /// </summary>
        internal const string NClob = "nclob";

        internal const string Cursor = "cursor";
        internal const string Dec = "dec";
        internal const string UniqueIdentifierSQLServer = "uniqueidentifier";
        internal const string Table = "table";
        internal const string Character = "character";

        /// <summary>
        /// line string field used to store char data type value as line value.
        /// </summary>
        internal const string LineString = "linestring";

        /// <summary>
        ///  curve datatypes for mysql datasources
        /// </summary>
        internal const string Curve = "curve";

        /// <summary>
        ///  get the collection of gemometry values for mysql
        /// </summary>
        internal const string GeometryCollection = "geometrycollection";

        /// <summary>
        ///  get the collection of gemometry values for mysql in MySQL higher version
        /// </summary>
        internal const string GeomCollection = "geomcollection";

        /// <summary>
        ///  get the multiple line string datatypes from the mysql.
        /// </summary>
        internal const string MultiLineString = "multilinestring";

        /// <summary>
        ///  Get the multiple point datatypes for mysql
        /// </summary>
        internal const string MultiPoint = "multipoint";

        /// <summary>
        ///  Get the collection of polygon datatypes for mysql
        /// </summary>
        internal const string MultiPolygon = "multipolygon";

        /// <summary>
        ///  Get the set datatypes for mysql
        /// </summary>
        internal const string Set = "set";

        /// <summary>
        /// get the enum datatypes for mysql
        /// </summary>
        internal const string Enum = "enum";

        /// <summary>
        /// get the snapshot datatype for postgresql
        /// </summary>
        internal const string TxidSnapshot = "txid_snapshot";

        /// <summary>
        /// get the xml type for oracle
        /// </summary>
        internal const string XmlType = "xmltype";
    }

}
