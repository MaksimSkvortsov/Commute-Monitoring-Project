using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommuteMonitoring.TrafficMonitor.Infrastructure
{
    internal class Message
    {
        public string chat_id { get; set; }

        public string text { get; set; }
    }

    public class TelegramClient
    {
        private readonly string _apiKey;
        private readonly string _chatId;


        public TelegramClient(string apiKey, string chatId)
        {
            _apiKey = apiKey;
            _chatId = chatId;
        }


        public async Task<int> SendMessage(string message)
        {
            var url = $"https://api.telegram.org/bot{_apiKey}/sendMessage?chat_id={_chatId}&amp";
            var messageData = new Message { chat_id = _chatId, text = message };

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(messageData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                return (int)response.StatusCode;
            }
        }
    }
}
