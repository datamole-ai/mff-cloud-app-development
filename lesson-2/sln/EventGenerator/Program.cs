using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunctions.Models;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = args[0];
            var eventHubName = "client-events";

            int wareshouseNumber = 10;
            var sectionNumber = 5;

            var warehouses = Enumerable
                .Range(0, wareshouseNumber).Select(r => $"warehouse_{r:D2}")
                .ToList();
            var sectionsEnum = Enumerable
                .Range(0, sectionNumber).Select(r => $"section-{r:D2}")
                .ToList();
            var sections = new List<string> { "receiveArea" };
            sections.AddRange(sectionsEnum);
            sections.Add("dispatchArea");

            var transports = GetTransports(warehouses, sections);

            await using var producerClient = new EventHubProducerClient(connectionString, eventHubName);
            EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
            foreach (var transport in transports)
            {
                var json = JsonSerializer.Serialize(transport);
                
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json))))
                {
                    await producerClient.SendAsync(eventBatch);
                    eventBatch = await producerClient.CreateBatchAsync();
                    Console.WriteLine("Sending batch");
                }

            }

            eventBatch.Dispose();
        }

        static IEnumerable<Transport> GetTransports(IList<string> warehouses, IList<string> sections)
        {
            var warehouseRandom = new Random(456);
            var secondsRandom = new Random(789);
            foreach (var movements in GetMovementLocations(sections))
            {
                var objectId = $"object_{Guid.NewGuid()}";
                var warehouseId = warehouses[warehouseRandom.Next(0, warehouses.Count)];
                foreach (var (a, b) in movements)
                {
                    yield return new Transport
                    {
                        LocationFrom = a,
                        LocationTo = b,
                        TransportDurationSec = secondsRandom.Next(0, 100),
                        ObjectId = objectId,
                        TranportedDateTime = DateTimeOffset.Now,
                        WarehouseId = warehouseId
                    };
                }
            }
        }

        static IEnumerable<IList<(string, string)>> GetMovementLocations(IList<string> locationNames)
        {
            if (locationNames.Count < 2)
            {
                throw new ArgumentException("There must be at least two locations");
            }
            foreach (var movements in GetMovements(locationNames.Count - 2))
            {
                if (!movements.Any())
                {
                    yield return new List<(string, string)>
                    {
                        (locationNames[0], locationNames[locationNames.Count-1])
                    };
                }
                else
                {
                    var (first, _) = movements.First();
                    var locs = new List<(string, string)>
                    {
                        (locationNames[0], locationNames[first+1])
                    };
                    foreach (var (location1, location2) in movements)
                    {
                        locs.Add((locationNames[location1 + 1], locationNames[location2 + 1]));
                    }
                    var (_, last) = movements[movements.Count - 1];
                    locs.Add((locationNames[last + 1], locationNames[locationNames.Count - 1]));
                    yield return locs;
                }
            }
        }

        static IEnumerable<IList<(int, int)>> GetMovements(int maxMovements)
        {
            var alphabet = "01";
            foreach (var randomString in GetRandomStringEnumerable(maxMovements, alphabet))
            {
                var indices = randomString
                    .Zip(Enumerable.Range(0, maxMovements))
                    .Where(r => r.First == '1')
                    .Select(r => r.Second);

                yield return indices
                    .Zip(indices.Skip(1))
                    .ToList();
            }
        }

        static IEnumerable<string> GetRandomStringEnumerable(int n, string alphabet)
        {
            var seed = 123;
            var random = new Random(seed);

            while (true)
            {
                yield return new string(Enumerable.Repeat(alphabet, n)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
    }


}
