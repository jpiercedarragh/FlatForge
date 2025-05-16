using System;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;

namespace FlatForge
{
    public class Config
    {
        public string ImportDirectory => @"C:\SqlImport\Exports\";
        public string ExportDirectory => @"C:\SqlImport\ErrorLogs\";

        public string DbUser { get; private set; } = string.Empty;
        public string DbPassword { get; private set; } = string.Empty;
        public string ConnectionString { get; private set; } = string.Empty;

        public string SmtpUsername { get; private set; } = string.Empty;
        public string SmtpPassword { get; private set; } = string.Empty;

        public string SmtpServer => "smtp.darraghcompany.com";
        public int SmtpPort => 587;
        public string EmailFrom => "noreply@darraghcompany.com";
        public string EmailTo => "jpierce@darraghcompany.com";

        public static readonly string KeyVaultName = "AzureKeyVaultName";
        public static readonly string VaultUri = $"https://{KeyVaultName}.vault.azure.net/";

        private readonly SecretClient _secretClient;

        public Config(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public async Task LoadSecretsAsync()
        {
            DbUser = (await _secretClient.GetSecretAsync("SqlDbUser")).Value.Value?.Trim()
                     ?? throw new Exception("SqlDbUser not found.");
            DbPassword = (await _secretClient.GetSecretAsync("SqlDbPassword")).Value.Value?.Trim()
                     ?? throw new Exception("SqlDbPassword not found.");
            SmtpUsername = (await _secretClient.GetSecretAsync("SmtpUsername")).Value.Value?.Trim()
                     ?? throw new Exception("SmtpUsername not found.");
            SmtpPassword = (await _secretClient.GetSecretAsync("SmtpPassword")).Value.Value?.Trim()
                     ?? throw new Exception("SmtpPassword not found.");

            ConnectionString = $"Server=tcp:darragh.database.windows.net,1433;" +
                               $"Database=darragh-dev;" +
                               $"User ID={DbUser};" +
                               $"Password={DbPassword};" +
                               $"Encrypt=True;" +
                               $"TrustServerCertificate=False;";
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception("SQL Connection string is not set.");
        }
    }
}