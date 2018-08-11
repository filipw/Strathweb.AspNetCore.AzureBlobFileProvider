using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Strathweb.AspNetCore.AzureBlobFileProvider;

namespace SampleWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var blobOptions = Configuration.GetSection("AzureBlobOptions").Get<AzureBlobOptions>();
            var azureBlobFileProvider = new AzureBlobFileProvider(blobOptions);
            services.AddSingleton(azureBlobFileProvider);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
}
