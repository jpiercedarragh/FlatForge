using System;
using System.Data;
using FlatForge.Helpers;

namespace FlatForge.Import
{
    public static class Validator
    {
        /// <summary>
        /// Validates and parses a row into a DataRow object, replacing invalid or blank fields with DBNull.Value.
        /// Returns false if the number of fields doesn't match the schema.
        /// </summary>
        public static bool ValidateRow(string tableName, string[] fields, out DataRow row)
        {
            var tableSchema = TableSchemas.GetSchema(tableName);

            if (fields.Length != tableSchema.Columns.Count)
            {
                row = null!;
                return false;
            }

            row = tableSchema.NewRow();

            for (int i = 0; i < fields.Length; i++)
            {
                var column = tableSchema.Columns[i];
                var rawValue = fields[i];

                object value = DBNull.Value;

                if (!string.IsNullOrWhiteSpace(rawValue))
                {
                    if (column.DataType == typeof(int))
                    {
                        var parsed = ParseHelper.ParseInt(rawValue);
                        value = parsed.HasValue ? parsed.Value : DBNull.Value;
                    }
                    else if (column.DataType == typeof(decimal))
                    {
                        var parsed = ParseHelper.ParseDecimal(rawValue);
                        value = parsed.HasValue ? parsed.Value : DBNull.Value;
                    }
                    else if (column.DataType == typeof(DateTime))
                    {
                        var parsed = ParseHelper.ParseDate(rawValue);
                        value = parsed.HasValue ? parsed.Value : DBNull.Value;
                    }
                    else if (column.DataType == typeof(bool))
                    {
                        var parsed = ParseHelper.ParseBool(rawValue);
                        value = parsed.HasValue ? parsed.Value : DBNull.Value;
                    }
                    else if (column.DataType == typeof(string))
                    {
                        var parsed = ParseHelper.ParseString(rawValue);
                        value = parsed is not null ? parsed : DBNull.Value;
                    }
                }

                row[i] = value;
            }

            return true;
        }
    }
}