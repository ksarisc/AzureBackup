using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace AzureBackup
{
    public class FileUploader
    {
        private readonly BlobServiceClient serviceClient;
        private readonly BlobContainerClient containerClient;

        public FileUploader(string connection, string containerName)
        {
            // connect
            serviceClient = new BlobServiceClient(connection);
            containerClient = serviceClient.GetBlobContainerClient(containerName);
        }

        private static async Task<string> CompressAsync(string fileName)
        {
            //if ((File.GetAttributes(fileInfo.FullName) & FileAttributes.Hidden) == FileAttributes.Hidden || EXTENSION)
            if (Path.GetExtension(fileName).Equals(".gz", StringComparison.OrdinalIgnoreCase))
            {
                return fileName;
            }
            var writeFile = $"{fileName}.gz";
            using (var streamRead = File.OpenRead(fileName))
            using (var streamWrite = File.Create(writeFile))
            using (var compressor = new GZipStream(streamWrite, CompressionMode.Compress))
            {
                await streamRead.CopyToAsync(compressor); //, 1024, cancellation);
            }
            return writeFile;
        } // END CompressAsync

        public async Task UploadFileAsync(string localFile, string remoteFolder = null)
        {
            // compress the file?
            localFile = await CompressAsync(localFile);
            var fileName = Path.GetFileName(localFile);
            // upload
            var client = containerClient.GetBlobClient(
                String.IsNullOrWhiteSpace(remoteFolder) ?
                    fileName :
                    Path.Combine(remoteFolder, fileName)
            );
            //Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
            using (var stream = File.OpenRead(localFile))
            {
                await client.UploadAsync(stream, true);
                stream.Close();
            }
        } // END UploadFileAsync
    }
}
