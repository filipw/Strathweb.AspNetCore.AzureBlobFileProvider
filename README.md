# Strathweb.AspNetCore.AzureBlobFileProvider

Azure Blob Storage file provider (`IFileProvider`) for ASP.NET Core.

### Installation

```
Install-Package Strathweb.AspNetCore.AzureBlobFileProvider
```

### Usage

Configure access to your Blob Storage via storage account connection string or SAS token. 

Below is the usage example for both flows - where access to files from Blob Storage is enabled on the `/files` route (including directory browsing in the browser).

**Connection string**

```csharp
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var blobOptions = new AzureBlobOptions
            {
               ConnectionString = "{my connection string}",
               DocumentContainer = "{blob container name}"
            }
            
            var azureBlobFileProvider = new AzureBlobFileProvider(blobOptions);
            services.AddSingleton(azureBlobFileProvider);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var blobFileProvider = app.ApplicationServices.GetRequiredService<AzureBlobFileProvider>();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = blobFileProvider,
                RequestPath = "/files"
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = blobFileProvider,
                RequestPath = "/files"
            });
        }
    }
```

**Token** (need to provide the URL of the storage separately)

```csharp
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var blobOptions = new AzureBlobOptions
            {
               BaseUri = "{base URL of the storage account}",
               Token = "{SAS token}",
               DocumentContainer = "{blob container name}"
            }
            
            var azureBlobFileProvider = new AzureBlobFileProvider(blobOptions);
            services.AddSingleton(azureBlobFileProvider);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var blobFileProvider = app.ApplicationServices.GetRequiredService<AzureBlobFileProvider>();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = blobFileProvider,
                RequestPath = "/files"
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = blobFileProvider,
                RequestPath = "/files"
            });
        }
    }
```

In case both `ConnectionString` and `Token` are present, connection string is given the preference.

### Current limitations

The watch functionality of the file provider is currently not supported.
