using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace Strathweb.AspNetCore.AzureBlobFileProvider
{
    public class AzureBlobFileInfo : IFileInfo
    {
        private readonly CloudBlockBlob _blockBlob;

        public AzureBlobFileInfo(IListBlobItem blob)
        {
            Exists = true;
            switch (blob)
            {
                case CloudBlobDirectory d:
                    IsDirectory = true;
                    Name = ((CloudBlobDirectory)blob).Prefix.TrimEnd('/');
                    PhysicalPath = d.StorageUri.PrimaryUri.ToString();
                    break;

                case CloudBlockBlob b:
                    b.FetchAttributes();
                    Length = b.Properties.Length;
                    PhysicalPath = b.Uri.ToString();
                    Name = !string.IsNullOrEmpty(b.Parent.Prefix) ? b.Name.Replace(b.Parent.Prefix, "") : b.Name;
                    LastModified = b.Properties.LastModified ?? DateTimeOffset.MinValue;
                    _blockBlob = b;
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

