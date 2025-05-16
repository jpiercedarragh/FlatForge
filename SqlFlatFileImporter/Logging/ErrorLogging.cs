using System;
using System.IO;
using System.Text;

namespace FlatForge.Logging
{
    public class Logger : IDisposable
    {
        private readonly string _logDirectory;
        private readonly string _logFileName;
        private readonly StringBuilder _logBuilder = new StringBuilder();
        private bool _hasErrors = false;

        public string LogFilePath => Path.Combine(_logDirectory, _logFileName);
        public bool HasErrors => _hasErrors;

        public Logger()
        {
            _logDirectory = Config.ExportDirectory;
            _logFileName = $"Dev-ErrorLog-{DateTime.Now:yyyy-MM-dd}.txt";

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public Logger(string logFilePath)
        {
            _logDirectory = Path.GetDirectoryName(logFilePath) ?? throw new ArgumentException("Invalid log file path.");
            _logFileName = Path.GetFileName(logFilePath);

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public void LogInfo(string message)
        {
            string timestamped = $"[INFO]  {DateTime.Now:HH:mm:ss} - {message}";
            Console.WriteLine(timestamped);
            _logBuilder.AppendLine(timestamped);
        }

        public void LogError(string message)
        {
            _hasErrors = true;
            string timestamped = $"[ERROR] {DateTime.Now:HH:mm:ss} - {message}";
            Console.WriteLine(timestamped);
            _logBuilder.AppendLine(timestamped);
        }

        public void SaveLog()
        {
            try
            {
                File.WriteAllText(LogFilePath, _logBuilder.ToString());
                Console.WriteLine($"Log written to: {LogFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log file: {ex.Message}");
            }
        }

        // Implemented IDisposable to `using var logger = new Logger(); in Program.cs`
        public void Dispose()
        {
            SaveLog();
        }
    }
}