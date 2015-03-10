using System.Net.Http;

namespace WorldDomination.Net.Http
{
    public static class HttpClientFactory
    {
        /// <summary>
        /// A custom message handler to use.
        /// </summary>
        /// <remarks>This is usually used for unit testing, so the HttpClient doesn't do a real request to some endpoint. Also, this is a static property so it's not thread safe.</remarks>
        public static HttpMessageHandler MessageHandler { get; set; }

        /// <summary>
        /// A normal HttpClient or one dependent upon a MessageHandler.
        /// </summary>
        /// <param name="httpMessageHandler">Optional message handler with specific rules for this client.</param>
        /// <returns></returns>
        public static HttpClient GetHttpClient(HttpMessageHandler httpMessageHandler = null)
        {
            // Logic: The static Message Handler takes priority. Why? This is usually used for unit tests.
            //        Also, we do not dispose of this message handler in case we reuse this again.
            //        Next, we use any handler passed in.
            //        Finally, no handler, just create a normal Http Client.
            return MessageHandler != null
                ? new HttpClient(MessageHandler, false)
                : httpMessageHandler != null
                    ? new HttpClient(httpMessageHandler)
                    : new HttpClient();
        }
    }
}