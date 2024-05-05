using System.Text;
using System.Text.Json;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventsGenerator;

public class Program
{
    private const int EventsPerBatch = 500;
    
    public static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Please provide the connection string as the first argument.");
            return;
        }
        
        var connectionString = args[0];
        
        await using var producerClient = new EventHubProducerClient(connectionString);

        while (GeneratorHelpers.DaysLeft >= 0)
        {
            using var eventBatch = await producerClient.CreateBatchAsync();
            
            for (var i = 1; i <= EventsPerBatch; i++)
            {
                var transport = GeneratorHelpers.GenerateTransport();
                var eventData = new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(transport)));
                
                if (!eventBatch.TryAdd(eventData))
                {
                    // If it is too large for the batch
                    throw new($"Event is too large for the batch and cannot be sent.");
                }
            }
            
            await producerClient.SendAsync(eventBatch);
            
            Console.WriteLine($"Event Batch Sent. Sent {GeneratorHelpers.ParcelCounter} events.");
        }
        
        Console.WriteLine($"Filled {GeneratorHelpers.Days} past days with {GeneratorHelpers.ParcelsPerDay} parcels each.");
    }
}
