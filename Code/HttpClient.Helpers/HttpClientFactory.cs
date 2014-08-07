using System.Net.Http;

namespace WorldDomination.HttpClient.Helpers
{
    public static class HttpClientFactory
    {
        public static HttpMessageHandler MessageHandler { get; set; }

        public static System.Net.Http.HttpClient GetHttpClient()
        {
            return MessageHandler == null
                ? new System.Net.Http.HttpClient()
                : new System.Net.Http.HttpClient(MessageHandler);
        }
    }
}