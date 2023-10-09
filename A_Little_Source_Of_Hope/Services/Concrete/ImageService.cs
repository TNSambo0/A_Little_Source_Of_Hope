using A_Little_Source_Of_Hope.Services.Abstract;
using A_Little_Source_Of_Hope.Options;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace A_Little_Source_Of_Hope.Services.Concrete
{
    public class ImageService : IImageService
    {
        private readonly AzureOptions _azureOptions;
        public ImageService(IOptions<AzureOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }
        public string uploadImageToAzure(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            using MemoryStream fileUploadsStream = new();
            file.CopyTo(fileUploadsStream);
            fileUploadsStream.Position = 0;
            BlobContainerClient blobContainerClient = new(_azureOptions.ConnectionString, _azureOptions.Container);
            var uniqueName = Guid.NewGuid().ToString()+fileExtension;
            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueName);
            blobClient.Upload(fileUploadsStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/bitmap"
                }
            }, cancellationToken:default);
            return uniqueName;
        }
        public void deleteImageFromAzure(string fileName)
        {
            BlobContainerClient blobContainerClient = new(_azureOptions.ConnectionString, _azureOptions.Container);
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            blobClient.Delete();
        }
    }
}
