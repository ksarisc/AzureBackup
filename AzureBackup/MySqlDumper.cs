using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AzureBackup
{
    public class MySqlDumper
    {
        private static readonly string command = @"/usr/bin/mysqldump -u{0} -p {1} | gzip > {2}.sql.gz";
        private readonly string db, user, pass;

        public MySqlDumper(string database, string userName, string password)
        {
            db = database;
            user = userName;
            pass = password;
        }

        public async Task BackupAsync(string dumpPath)
        {
            var cmd = String.Format(command, user, db,
                    Path.Combine(dumpPath, $"{db}_{Utils.GetTimestamp()}"));
            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                };
                //proc.ErrorDataReceived
                //proc.OutputDataReceived

                proc.Start();
                proc.WaitForInputIdle();
                await proc.StandardInput.WriteLineAsync(pass);
                // proc.WaitForExit();
                // proc.WaitForInputIdle();
            }
        } // END BackupAsync
    }
}
