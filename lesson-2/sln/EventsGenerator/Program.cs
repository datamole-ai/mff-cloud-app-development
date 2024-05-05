using System.Text;
using System.Text.Json;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventsGenerator;

public class Program
{
    private const int EventsPerBatch = 50;
    
    public static async Task Main(string[] args)
    {
        var connectionString = args[0];
        
        await using var producerClient = new EventHubProducerClient(connectionString);
        
        using var eventBatch = await producerClient.CreateBatchAsync();

        while (true)
        {
            for (var i = 1; i <= EventsPerBatch; i++)
            {
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(GeneratorHelpers.GenerateTransport())))))
                {
                    // If it is too large for the batch
                    throw new($"Event {i} is too large for the batch and cannot be sent.");
                }
            }
            
            await producerClient.SendAsync(eventBatch);
            
            Console.WriteLine("Event Batch Sent");
            
            await Task.Delay(500);
        }
    }
}
