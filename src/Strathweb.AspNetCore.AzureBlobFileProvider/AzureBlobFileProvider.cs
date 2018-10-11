using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace Strathweb.AspNetCore.AzureBlobFileProvider
{
    public class AzureBlobFileProvider : IFileProvider
    {
        private CloudBlobContainer _container;

        public AzureBlobFileProvider(AzureBlobOptions azureBlobOptions)
        {
            CloudBlobClient blobClient;
            if (azureBlobOptions.ConnectionString != null && CloudStorageAccount.TryParse(azureBlobOptions.ConnectionString, out var cloudStorageAccount))
            {
                blobClient = cloudStorageAccount.CreateCloudBlobClient();
            }
            else if (azureBlobOptions.BaseUri != null && azureBlobOptions.Token != null)
            {
                blobClient = new CloudBlobClient(azureBlobOptions.BaseUri, new StorageCredentials(azureBlobOptions.Token));
            }
            else
            {
                throw new ArgumentException("One of the following must be set: 'ConnectionString' or 'BaseUri'+'Token'!");
            }

            _container = blobClient.GetContainerReference(azureBlobOptions.DocumentContainer);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var blob = _container.GetDirectoryReference(subpath.TrimStart('/').TrimEnd('/'));
            return new AzureBlobDirectoryContents(blob);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var blob = _container.GetBlockBlobReference(subpath.TrimStart('/').TrimEnd('/'));
            return new AzureBlobFileInfo(blob);
        }

        public IChangeToken Watch(string filter) => throw new NotImplementedException();
    }
}

