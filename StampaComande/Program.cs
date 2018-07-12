using Microsoft.Azure.ServiceBus;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;

namespace StampaComande
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var nomeCoda = args[0];

            var cloudStorageAccount = CloudStorageAccount.Parse("");
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(nomeCoda);
            await container.CreateIfNotExistsAsync();

            var queueClient = new QueueClient("", nomeCoda);
            queueClient.RegisterMessageHandler(async (m, c) =>
            {
                var blobName = $"{Guid.NewGuid()}.json";
                var blob = container.GetBlockBlobReference(blobName);
                await blob.UploadFromByteArrayAsync(m.Body, 0, m.Body.Length);
            }, e =>
            {
                return Task.CompletedTask;
            });
        }
    }
}
