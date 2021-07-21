using System.Net.Http;
using System.Net.Http.Headers;

namespace JangleeBot.Helpers
{
    public static class ClientHelper
    {
        public static HttpClient Client { get; set; }
        public static void InitializeClient()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
