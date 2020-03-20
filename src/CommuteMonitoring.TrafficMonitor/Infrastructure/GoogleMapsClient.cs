using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommuteMonitoring.TrafficMonitor.Infrastructure
{
    public class GoogleMapsClient
    {
        private readonly string _key;

        public GoogleMapsClient(string key)
        {
            _key = key;
        }

        public async Task<int> GetDistnace(string origin, string destination)
        {
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&key={_key}&departure_time=now";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var time = ParseRespnse(content);

                    return time;
                }

                //ToDo: throw exception
                return 0;
            }
        }

        private static int ParseRespnse(string jsonResponse)
        {
            JObject googleSearch = JObject.Parse(jsonResponse);

            var result = googleSearch["routes"].First["legs"].First["duration_in_traffic"]["value"];
            var resultInSeconds = result.ToObject<int>();

            return resultInSeconds;
        }
    }
}
