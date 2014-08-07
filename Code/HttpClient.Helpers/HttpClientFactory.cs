using System.Net.Http;

namespace WorldDomination.Net.Http
{
    public static class HttpClientFactory
    {
        public static HttpMessageHandler MessageHandler { get; set; }

        public static HttpClient GetHttpClient()
        {
            return MessageHandler == null
                ? new HttpClient()
                : new HttpClient(MessageHandler);
        }
    }
}