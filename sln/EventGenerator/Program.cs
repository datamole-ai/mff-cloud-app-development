using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AzureFunctions.Models;


namespace EventGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            var uri = args[0];

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

            foreach (var transport in transports)
            {
                var json = JsonSerializer.Serialize(transport);
                Console.WriteLine($"Sending {json}");
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = stringContent
                };
                var response = await httpClient.SendAsync(request);

                Console.WriteLine(response.StatusCode);
            }
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
                        TimeSpentSeconds = secondsRandom.Next(0, 100),
                        ObjectId = objectId,
                        TranportedDateTime = DateTimeOffset.Now,
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
