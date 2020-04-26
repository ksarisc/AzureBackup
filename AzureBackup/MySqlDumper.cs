using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AzureBackup
{
    public class MySqlDumper
    {
        private static readonly string command = @"/usr/bin/mysqldump -u{0} -p --routines {1} | gzip > {2}.sql.gz";
        private readonly string db, user, pass;
        private readonly StreamWriter writer;

        public MySqlDumper(string database, string userName, string password, string logPath)
        {
            // environment DEBIAN
            //{/usr/bin/mysqldump}
            // environment WINDOWS
            //
            db = database;
            user = userName;
            pass = password;
            writer = new StreamWriter(logPath);
        }

        public async Task BackupAsync(string dumpPath)
        {
            var cmd = String.Format(command, user, db,
                    Path.Combine(dumpPath, $"{db}_{Utils.GetTimestamp()}"))
                    .Replace("\"", "\\\"");
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{cmd}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            })
            {
                proc.ErrorDataReceived += OnErrorDataReceived;
                proc.OutputDataReceived += OnOutputDataReceived;
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                proc.WaitForInputIdle();
                await proc.StandardInput.WriteLineAsync(pass);
                // proc.WaitForExit();
                // proc.WaitForInputIdle();

                proc.ErrorDataReceived -= OnErrorDataReceived;
                proc.OutputDataReceived -= OnOutputDataReceived;
            }
        } // END BackupAsync

        private async void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            await writer.WriteLineAsync($"{Utils.GetTimestamp(true)} [ERR] {e.Data}");
        } // END OnErrorDataReceived

        private async void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            await writer.WriteLineAsync($"{Utils.GetTimestamp(true)} [STD] {e.Data}");
        } // END OnOutputDataReceived
    }
}
