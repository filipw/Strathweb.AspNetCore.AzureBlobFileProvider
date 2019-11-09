using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

namespace Strathweb.AspNetCore.AzureBlobFileProvider
{
    public class DefaultBlobContainerFactory : IBlobContainerFactory
    {
        private readonly CloudBlobContainer _container;

        public DefaultBlobContainerFactory(AzureBlobOptions azureBlobOptions)
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

        public CloudBlobContainer GetContainer(string subpath)
        {
            return _container;
        }

        public string TransformPath(string subpath)
        {
            return subpath.TrimStart('/').TrimEnd('/');
        }
    }
}
