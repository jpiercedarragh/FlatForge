using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SqlFlatFileImporter.Email
{
    public static class EmailHelper
    {
        public static void SendLogEmail(string logFilePath, bool hasErrors)
        {
            string toEmail = Config.EmailTo;
            string fromEmail = Config.EmailFrom;
            string subject = hasErrors ? "🚨 SQL Import Errors Detected" : "✅ SQL Import Success Report";
            string body = hasErrors
                ? "Errors occurred during the SQL import process. Please review the attached log for details."
                : "The SQL import process completed successfully. See the attached log for details.";

            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body
            };

            try
            {
                if (File.Exists(logFilePath))
                {
                    message.Attachments.Add(new Attachment(logFilePath));
                }

                using (var smtp = new SmtpClient(Config.SmtpServer, Config.SmtpPort))
                {
                    smtp.Credentials = new NetworkCredential(Config.SmtpUsername, Config.SmtpPassword);
                    smtp.EnableSsl = true;

                    smtp.Send(message);
                    Console.WriteLine("Log email sent to " + toEmail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}