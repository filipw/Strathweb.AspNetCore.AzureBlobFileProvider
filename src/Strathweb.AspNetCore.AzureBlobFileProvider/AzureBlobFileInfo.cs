using System;
using System.IO;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.FileProviders;

namespace Strathweb.AspNetCore.AzureBlobFileProvider
{
    public class AzureBlobFileInfo : IFileInfo
    {
        private readonly CloudBlockBlob _blockBlob;

        public AzureBlobFileInfo(IListBlobItem blob)
        {
            switch (blob)
            {
                case CloudBlobDirectory d:
                    Exists = true;
                    IsDirectory = true;
                    Name = ((CloudBlobDirectory)blob).Prefix.TrimEnd('/');
                    PhysicalPath = d.StorageUri.PrimaryUri.ToString();
                    break;

                case CloudBlockBlob b:
                    _blockBlob = b;
                    Name = !string.IsNullOrEmpty(b.Parent.Prefix) ? b.Name.Replace(b.Parent.Prefix, "") : b.Name;
                    Exists = b.Exists();
                    if (Exists)
                    {
                        b.FetchAttributes();
                        Length = b.Properties.Length;
                        PhysicalPath = b.Uri.ToString();
                        LastModified = b.Properties.LastModified ?? DateTimeOffset.MinValue;
                    }
                    else
                    {
                        Length = -1;
                        // IFileInfo.PhysicalPath docs say: Return null if the file is not directly accessible.
                        // (PhysicalPath should maybe also be null for blobs that do exist but that would be a potentially breaking change.)
                        PhysicalPath = null;
                    }
                    break;
            }
        }

        public Stream CreateReadStream()
        {
            var stream = new MemoryStream();
            _blockBlob.DownloadToStream(stream);
            stream.Position = 0;
            return stream;
        }

        public bool Exists { get; }
        public long Length { get; }
        public string PhysicalPath { get; }
        public string Name { get; }
        public DateTimeOffset LastModified { get; }
        public bool IsDirectory { get; }
    }
}

