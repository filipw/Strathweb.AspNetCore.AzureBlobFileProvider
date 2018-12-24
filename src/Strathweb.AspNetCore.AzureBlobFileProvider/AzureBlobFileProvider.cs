using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Strathweb.AspNetCore.AzureBlobFileProvider
{
    public class AzureBlobFileProvider : IFileProvider
    {
        private readonly IContainerFactory _containerFactory;

        public AzureBlobFileProvider(IContainerFactory containerFactory)
        {
            _containerFactory = containerFactory;
        }

        public AzureBlobFileProvider(AzureBlobOptions azureBlobOptions)
        {
            _containerFactory = new DefaultContainerFactory(azureBlobOptions);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var container = _containerFactory.GetContainer(subpath);
            var blob = container.GetDirectoryReference(_containerFactory.TransformPath(subpath));
            return new AzureBlobDirectoryContents(blob);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var container = _containerFactory.GetContainer(subpath);
            var blob = container.GetBlockBlobReference(_containerFactory.TransformPath(subpath));
            return new AzureBlobFileInfo(blob);
        }

        public IChangeToken Watch(string filter) => throw new NotImplementedException();
    }
}

