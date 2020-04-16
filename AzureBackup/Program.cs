using System;

namespace AzureBackup
{
    class Program
    {
        static async void Main(string[] args)
        {
            var connect = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            try {
                var uploader = new FileUploader(connect, "database");
                await uploader.UploadFileAsync($"db_{DateTime.Today.ToString("yyyyMMdd")}.bak.gz");
            } catch (Exception eBak) {
                await Console.Error.WriteLineAsync($"MySQL Database back error: {eBak}");
            }
        }
    }
}
