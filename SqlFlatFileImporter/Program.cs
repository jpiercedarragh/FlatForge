using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FlatForge;
using FlatForge.Import;
using FlatForge.Logging;
using FlatForge.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Azure;

class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddSecretClient(new Uri(Config.VaultUri));
                    builder.UseCredential(new DefaultAzureCredential());
                });

                services.AddSingleton<Config>();
            })
            .Build();

        // Resolve dependencies
        var config = host.Services.GetRequiredService<Config>();
        await config.LoadSecretsAsync();

        config.Validate(); // still use static validation

        string today = DateTime.Today.ToString("yyyy-MM-dd");
        string logPath = Path.Combine(Config.ExportDirectory, $"errorlog-{today}.txt");
        using var logger = new Logger(logPath);

        logger.LogInfo("Starting multi-table import process...");

        var tablesToImport = new List<string> { "product" };

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