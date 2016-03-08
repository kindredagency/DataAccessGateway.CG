using Framework.DataAccessGateway.CG.Models;
using Framework.DataAccessGateway.Core;
using Framework.DataAccessGateway.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.DataAccessGateway.CG
{
    internal static class ExtensionMethods
    {
        public static string ToSqlDataTypeSize(this DBSchemaTableColumnDefinition column)
        {
            switch (column.DataType)
            {
                case DBHandlerDataType.Binary: return "(" + (column.Length == -1 ? "MAX" : column.Length.ToString()) + ")";
                case DBHandlerDataType.Char: return "(" + (column.Length == -1 ? "MAX" : column.Length.ToString()) + ")";
                case DBHandlerDataType.NChar: return "(" + (column.Length == -1 ? "MAX" : column.Length.ToString()) + ")";
                case DBHandlerDataType.NVarChar: return "(" + (column.Length == -1 ? "MAX" : column.Length.ToString()) + ")";
                case DBHandlerDataType.VarBinary: return "(" + (column.Length == -1 ? "MAX" : column.Length.ToString()) + ")";
                case DBHandlerDataType.VarChar: return "(" + (column.Length == -1 ? "MAX" : column.Length.ToString()) + ")";

                case DBHandlerDataType.DateTime2: return "(" + column.Precision + ")";
                case DBHandlerDataType.DateTimeOffset: return "(" + column.Precision + ")";
                case DBHandlerDataType.Decimal: return "(" + column.Precision + "," + column.Scale + ")";
            }

            return String.Empty;
        }

        public static SqlDbType ToSqlDataType(this DBHandlerDataType dbHandlerDataType)
        {
            return DBHandlerDataMapping.Mappings.Where(c => c.DBHandlerDataType == dbHandlerDataType).FirstOrDefault().SqlDataType;
        }

        public static Type ToCSharpType(this DBHandlerDataType dbHandlerDataType)
        {
            return DBHandlerDataMapping.Mappings.Where(c => c.DBHandlerDataType == dbHandlerDataType).FirstOrDefault().CSDataType;
        }

        public static string ReadTextFile(this string fileName, string location)
        {
            if (!fileName.Contains(".txt"))
                throw new Exception("File name is not a valid text file reference");

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            string fileContent = null;

            using (StreamReader reader = new StreamReader(currentAssembly.GetManifestResourceStream(currentAssembly.GetName().Name + "." + location + "." + fileName)))
            {
                fileContent = reader.ReadToEnd();
            }

            return fileContent;
        }

        public static bool IsAllowedForInsertOrUpdate(this DBHandlerDataType dbHandlerDataType)
        {
            if (dbHandlerDataType == DBHandlerDataType.TimeStamp)
                return false;

            return true;
        }

        public static string FormatCode(this string value)
        {
            return Regex.Replace(value, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
        }

        public static string TrimEndFrom(this string value, string stringToTrim)
        {
            return value.Substring(0, value.LastIndexOf(stringToTrim)).TrimEnd();
        }
        
        public static string ToCreateSql(this List<StoredProcedure> storedProcedures)
        {
            StringBuilder sql = new StringBuilder();

            foreach (var storedProcedure in storedProcedures)
            {
                sql.AppendLine(storedProcedure.CreateStatement);
                sql.AppendLine("GO");
            }

            return sql.ToString();
        }

        public static string ToCreateSql(this List<UserDefinedTableType> storedProcedures)
        {
            StringBuilder sql = new StringBuilder();

            foreach (var storedProcedure in storedProcedures)
            {
                sql.AppendLine(storedProcedure.CreateStatement);
                sql.AppendLine("GO");
            }

            return sql.ToString();
        }

        public static string ToDropSql(this List<StoredProcedure> storedProcedures)
        {
            StringBuilder sql = new StringBuilder();

            foreach (var storedProcedure in storedProcedures)
            {
                sql.AppendLine(storedProcedure.DropStatement);
                sql.AppendLine("GO");
            }

            return sql.ToString();
        }

        public static string ToDropSql(this List<UserDefinedTableType> storedProcedures)
        {
            StringBuilder sql = new StringBuilder();

            foreach (var storedProcedure in storedProcedures)
            {
                sql.AppendLine(storedProcedure.DropStatement);
                sql.AppendLine("GO");
            }

            return sql.ToString();
        }

        public static string ToSql(this List<StoredProcedure> storedProcedures)
        {
            StringBuilder sql = new StringBuilder();

            foreach (var storedProcedure in storedProcedures)
            {
                sql.AppendLine(storedProcedure.SqlStatement);
            }

            return sql.ToString();
        }

        public static string ToSql(this List<UserDefinedTableType> storedProcedures)
        {
            StringBuilder sql = new StringBuilder();

            foreach (var storedProcedure in storedProcedures)
            {
                sql.AppendLine(storedProcedure.SqlStatement);
            }

            return sql.ToString();
        }

        public static bool IsNonNullable(this Type value)
        {
            Type[] nonNullableTypes = new Type[] {  typeof(byte[]), typeof(string) };

            if (nonNullableTypes.Contains(value))
                return true;

            return false;
        }       
    }

}