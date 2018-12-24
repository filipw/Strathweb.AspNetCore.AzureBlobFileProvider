using Microsoft.WindowsAzure.Storage.Blob;

namespace Strathweb.AspNetCore.AzureBlobFileProvider
{
    public interface IContainerFactory
    {
        CloudBlobContainer GetContainer(string subpath);
        string TransformPath(string subpath);
    }
}
