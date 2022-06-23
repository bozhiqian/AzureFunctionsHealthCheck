using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DreamCloud.Functions.Infrastructure;

[assembly: FunctionsStartup(typeof(DreamCloud.Functions.Startup))]
namespace DreamCloud.Functions;

public class Startup : FunctionsStartup
{
    private IConfiguration _configuration;
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var storageAccountName = _configuration["StorageAccountName"];

        if (storageAccountName == null)
        {
            // todo... logging.
            throw new ArgumentNullException("Storage AccountName cannot be null!");
        }

        // Create a BlobServiceClient that will authenticate through Active Directory
        Uri blobServiceUri = new Uri($"https://{storageAccountName}.blob.core.windows.net/");

        builder.Services.AddFunctionHealthChecks()
            .AddAzureBlobStorage(blobServiceUri, new Azure.Identity.DefaultAzureCredential());
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        // local.settings.json are automatically loaded when debugging.
        // When running on Azure, values are loaded defined in app settings.
        // See: https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings
        _configuration = builder.ConfigurationBuilder
            .AddAppSettingsJson(builder.GetContext())
            .Build();

        base.ConfigureAppConfiguration(builder);
    }
}