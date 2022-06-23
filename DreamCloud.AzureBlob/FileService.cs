using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DreamCloud.AzureBlob.ExtensionMethods;

namespace DreamCloud.AzureBlob;

public interface IFileService
{
    Task<BlobContentInfo> UploadFileAsync(string uploadFileName, string blobName);
}

public class FileService : IFileService
{
    private readonly BlobServiceClient _blobServiceClient;
    public FileService(BlobServiceClient blobServiceClient, string blobContainerName)
    {
        _blobServiceClient = blobServiceClient;
        BlobContainerName = blobContainerName;
    }

    public string BlobContainerName { get;}
    public async Task<BlobContentInfo> UploadFileAsync(string uploadFileName, string blobName)
    {
        var blobClient = await _blobServiceClient.GetBlobContainerClientAsync(BlobContainerName, blobName);
            
        // Get content stream
        var stream = GetFileStream(uploadFileName);

        // Upload blob
        var blobUploadOptions = new BlobUploadOptions()
        {
            Tags = new Dictionary<string, string>() { { "Team", "CRM" } }
        };
        var response = await blobClient.UploadAsync(stream, blobUploadOptions);

        return response.Value;
    }

    public FileStream GetFileStream(string fileName)
    {
        var filePathName = Path.Combine(Environment.CurrentDirectory, @"TestFiles\", fileName);

        // Get content stream
        var stream = File.OpenRead(filePathName);

        return stream;
    }
}