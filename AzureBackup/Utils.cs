using System;

namespace AzureBackup
{
    internal static class Utils
    {
        private static readonly string datestampFormat = "yyyyMMdd";
        private static readonly string timestampFormat = "yyyyMMdd_HHmmss";

        public static string GetTimestamp(bool includeTime = true)
        {
            return DateTime.Now.ToString(
                        includeTime ? timestampFormat : datestampFormat);
        }
    }
}
