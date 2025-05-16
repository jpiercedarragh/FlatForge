using FlatForge.Logging;
using System;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;

namespace FlatForge.Import
{
    public static class TableImporter
    {
        public static void ImportTable(string tableName, Logger logger)
        {
            string filePath = Path.Combine(Config.ExportDirectory, $"{tableName}.txt");
            DataTable table = TableSchemas.GetSchema(tableName);

            logger.LogInfo($"Starting to read data for table {tableName} from {filePath}");

            int rowNum = 0;
            foreach (var line in File.ReadLines(filePath))
            {
                rowNum++;
                var fields = line.Split('~');

                if (!Validator.ValidateRow(tableName, fields, out var row))
                {
                    logger.LogError($"Row {rowNum} in {tableName} failed validation: {string.Join("~", fields)}");
                    continue;
                }

                table.Rows.Add(row);
            }

            logger.LogInfo($"Finished reading {tableName} data. Starting bulk insert...");

            using var conn = new SqlConnection(Config.ConnectionString);
            conn.Open();

            using (var clearCmd = new SqlCommand($"TRUNCATE TABLE dbo.{tableName}", conn))
            {
                clearCmd.ExecuteNonQuery();
                logger.LogInfo($"Truncated {tableName} table.");
            }

            using var bulkCopy = new SqlBulkCopy(conn)
            {
                DestinationTableName = $"dbo.{tableName}",
                BulkCopyTimeout = 0
            };

            bulkCopy.WriteToServer(table);

            logger.LogInfo($"Bulk insert for {tableName} completed successfully.");
        }
    }
}