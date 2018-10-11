using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Strathweb.AspNetCore.AzureBlobFileProvider
{
    public class AzureBlobDirectoryContents : IDirectoryContents
    {
        private List<IListBlobItem> _blobs = new List<IListBlobItem>();
        public bool Exists { get; set; }

        public AzureBlobDirectoryContents(CloudBlobDirectory blob)
        {
            BlobContinuationToken continuationToken = null;

            do
            {
                var response = blob.ListBlobsSegmented(continuationToken);
                continuationToken = response.ContinuationToken;
                _blobs.AddRange(response.Results);
            }
            while (continuationToken != null);
            Exists = _blobs.Count > 0;
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _blobs.Select(blob => new AzureBlobFileInfo(blob)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

