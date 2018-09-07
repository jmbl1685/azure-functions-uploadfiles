using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace FilesUpload.AzureService
{
    public static class AzureService
    {

        private const string StorageAccountName = "kalmanzac";
        private const string StorageAccountKey = "wtliqXXKWGfi/Sy2S+ZwLOmlLXf6y9AQfHBkNh2P7KrIeoI4NvZPkOmB6Pf9qMKy2Liy2x0GGq0cEsMe5ETBWg==";

        #region GetStorageAccount() 
        private static CloudStorageAccount GetStorageAccount()
        {
            var StorageCredentials = new StorageCredentials(StorageAccountName, StorageAccountKey);
            return new CloudStorageAccount(StorageCredentials, true);
        }
        #endregion

        #region GetBlobContainer() 
        private static CloudBlobContainer GetBlobContainer(string name)
        {
            CloudBlobContainer container = null;

            try
            {
                var blobClient = GetStorageAccount().CreateCloudBlobClient();
                container = blobClient.GetContainerReference(name);

                container.CreateIfNotExists();

                container.SetPermissions(new BlobContainerPermissions()
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

            }
            catch (Exception e)
            {
                // TODO: Add method to save exception [message]
                return container;
            }
            return container;
        }
        #endregion

        public static string SaveFile(Stream FileStream, string ContentType)
        {
            string blobUrl = null;

            try
            {
                var blob = GetBlobContainer("test-blob").GetBlockBlobReference(Guid.NewGuid().ToString());
                blob.Properties.ContentType = ContentType;
                blob.UploadFromStreamAsync(FileStream);
                blobUrl = blob.Uri.AbsoluteUri;
            }
            catch (Exception e)
            {
                throw new ArgumentNullException($"Error {e.Message}");
            }
            return blobUrl;
        }

    }
}
