using System;
using System.Net.Http;

namespace WorldDomination.Net.Http
{
    internal class MessageHandlerItem
    {
        public MessageHandlerItem(HttpMessageHandler httpMessageHandler,
            bool disposeHandler)
        {
            if (httpMessageHandler == null)
            {
                throw new ArgumentNullException("httpMessageHandler");
            }

            HttpMessageHandler = httpMessageHandler;
            DisposeHandler = disposeHandler;
        }

        public HttpMessageHandler HttpMessageHandler { get; private set; }
        public bool DisposeHandler { get; private set; }
    }
}