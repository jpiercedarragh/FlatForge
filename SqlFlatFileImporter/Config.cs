using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace SqlFlatFileImporter
{
    public static class Config
    {
        public static string ImportDirectory => @"C:\SqlImport\Exports\";
        public static string ExportDirectory => @"C:\SqlImport\ErrorLogs\";

        public static string DbUser { get; private set; } = string.Empty;
        public static string DbPassword { get; private set; } = string.Empty;
        public static string ConnectionString { get; private set; } = string.Empty;

        public static string SmtpUsername { get; private set; } = string.Empty;
        public static string SmtpPassword { get; private set; } = string.Empty;

        public static string SmtpServer => "smtp.darraghcompany.com";
        public static int SmtpPort => 587;
        public static string EmailFrom => "noreply@darraghcompany.com";
        public static string EmailTo => "jpierce@darraghcompany.com";

        private static readonly string KeyVaultName = "AzureKeyVaultName"; // change me
        private static readonly string VaultUri = $"https://{KeyVaultName}.vault.azure.net/";

        public static async Task LoadSecretsAsync()
        {
            var client = new SecretClient(new Uri(VaultUri), new DefaultAzureCredential());

            DbUser = (await client.GetSecretAsync("SqlDbUser")).Value.Value?.Trim()
                           ?? throw new Exception("SqlDbUser not found in Key Vault.");
            DbPassword = (await client.GetSecretAsync("SqlDbPassword")).Value.Value?.Trim()
                           ?? throw new Exception("SqlDbPassword not found in Key Vault.");
            SmtpUsername = (await client.GetSecretAsync("SmtpUsername")).Value.Value?.Trim()
                           ?? throw new Exception("SmtpUsername not found in Key Vault.");
            SmtpPassword = (await client.GetSecretAsync("SmtpPassword")).Value.Value?.Trim()
                           ?? throw new Exception("SmtpPassword not found in Key Vault.");

            //Build connection string dynamically
            ConnectionString = $"Server=tcp:darragh.database.windows.net,1433;" +
                               $"Database=darragh-dev;" +
                               $"User ID={DbUser};" +
                               $"Password={DbPassword};" +
                               $"Encrypt=True;" +
                               $"TrustServerCertificate=False;";
        }

        public static void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception("SQL Connection string is not set.");
            if (string.IsNullOrWhiteSpace(ImportDirectory))
                throw new Exception("Import directory path is not set.");
            if (string.IsNullOrWhiteSpace(ExportDirectory))
                throw new Exception("Export directory path is not set.");
        }
    }
}