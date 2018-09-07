using FilesUpload.AzureFunction.Models;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;

namespace FilesUpload.AzureService
{
    public static class AzureService
    {

        private static string StorageAccountName = ConfigurationManager.AppSettings["STORAGE_ACCOUNT_NAME"];
        private static string StorageAccountKey = ConfigurationManager.AppSettings["STORAGE_ACCOUNT_KEY"];

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


        public static InfoFile SaveFile(Stream FileStream, string ContentType, TraceWriter log)
        {
            string blobUrl = null;

            try
            {
                var filename = Guid.NewGuid().ToString();
                var blob = GetBlobContainer("test-blob").GetBlockBlobReference(filename);
                blob.Properties.ContentType = ContentType;
                blob.UploadFromStreamAsync(FileStream);
                blobUrl = blob.Uri.AbsoluteUri;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                throw new ArgumentNullException($"Error: {e.Message}");
            }

            return new InfoFile() { Url = blobUrl , ContentType = ContentType};

        }
    }
}
