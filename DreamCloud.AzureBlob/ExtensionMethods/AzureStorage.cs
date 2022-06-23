using Azure.Storage.Blobs;

namespace DreamCloud.AzureBlob.ExtensionMethods;

public static class AzureStorage
{
    public static async Task<BlobClient> GetBlobContainerClientAsync(this BlobServiceClient blobServiceClient,
        string blobContainerName, string blobName)
    {
        var blobContainerClient = await blobServiceClient.GetBlobContainerClientAsync(blobContainerName);

        var blobClient = blobContainerClient.GetBlobClient(blobName);

        return blobClient;
    }

    public static async Task<BlobContainerClient> GetBlobContainerClientAsync(this BlobServiceClient blobServiceClient, string blobContainerName)
    {
        // Create container
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
        var result = await blobContainerClient.CreateIfNotExistsAsync();
        //var blobContainerInfo = result.Value;

        return blobContainerClient;
    }
}