using System;
using System.Globalization; // install on server before deployment (available in local dev environment not live prod environment)

namespace FlatForge.Helpers
{
    public static class ParseHelper
    {
        public static int? ParseInt(string input)
        {
            return int.TryParse(input, out var result) ? result : (int?)null;
        }

        public static decimal? ParseDecimal(string input, int precision = 2)
        {
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return Math.Round(result, precision);
            }
            return null;
        }

        public static DateTime? ParseDate(string input)
        {
            if (DateTime.TryParse(input, out var result))
                return result;

            // Date format may need to be edited for Live Environment
            if (DateTime.TryParseExact(input,
                new[] { "yyyyMMdd", "MM/dd/yyyy", "yyyy-MM-dd" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result))
            {
                return result;
            }

            return null;
        }

        public static bool? ParseBool(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            switch (input.Trim().ToLower())
            {
                case "1":
                case "true":
                case "yes":
                case "y":
                    return true;
                case "0":
                case "false":
                case "no":
                case "n":
                    return false;
                default:
                    return null;
            }
        }

        public static string? ParseString(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        public static T ParseEnum<T>(string input) where T : struct
        {
            return Enum.TryParse<T>(input, true, out var result) ? result : default;
        }
    }
}
