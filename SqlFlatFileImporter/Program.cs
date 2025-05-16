using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FlatForge.Import;
using FlatForge.Logging;
using FlatForge.Email;

namespace FlatForge
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load secrets from Azure Key Vault before anything else
            try
            {
                await Config.LoadSecretsAsync();
                Config.Validate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load secrets from Azure Key Vault: {ex.Message}");
                return;
            }

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string logPath = Path.Combine(Config.ExportDirectory, $"errorlog-{today}.txt");
            using var logger = new Logger(logPath);

            logger.LogInfo("Starting multi-table import process...");

            var tablesToImport = new List<string>
            {
                "product",
                // Add other table names here
            };

            foreach (var table in tablesToImport)
            {
                try
                {
                    logger.LogInfo($"Beginning import for table: {table}");
                    TableImporter.ImportTable(table, logger);
                    logger.LogInfo($"Table {table} imported successfully at {DateTime.Now:HH:mm:ss}.");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error importing table {table}: {ex.Message}");
                }
            }

            logger.LogInfo("All table imports complete. Sending summary email...");

            try
            {
                EmailHelper.SendLogEmail(logPath, logger.HasErrors);
                logger.LogInfo("Log email sent successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to send log email: {ex.Message}");
            }
        }
    }
}