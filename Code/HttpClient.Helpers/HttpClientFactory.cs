using System;
using System.Collections.Generic;
using System.Net.Http;

namespace WorldDomination.Net.Http
{
    public static class HttpClientFactory
    {
        private static readonly Lazy<Dictionary<string, MessageHandlerItem>> MessageHandlers =
            new Lazy<Dictionary<string, MessageHandlerItem>>();

        public static void AddMessageHandler(HttpMessageHandler messageHandler, 
            string key,
            bool disposeHandler = true)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException("messageHandler");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (MessageHandlers.Value.ContainsKey(key))
            {
                var errorMessage =
                    string.Format(
                        "Unable to add the MessageHandler instance because the key '{0}' -already- exists. Please use another key.",
                        key);
                throw new Exception(errorMessage);
            }

            MessageHandlers.Value.Add(key, new MessageHandlerItem(messageHandler, disposeHandler));
        }

        public static void RemoveMessageHandler(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            MessageHandlers.Value.Remove(key);
        }

        /// <summary>
        /// A normal HttpClient or one dependent upon a MessageHandler.
        /// </summary>
        /// <param name="httpMessageHandler">Optional message handler with specific rules for this client.</param>
        /// <param name="disposeHandler">true if the inner handler should be disposed of by Dispose(),false if you intend to reuse the inner handler.</param>
        /// <returns></returns>
        public static HttpClient GetHttpClient(HttpMessageHandler httpMessageHandler,
            bool disposeHandler = true)
        {
            return httpMessageHandler != null
                ? new HttpClient(httpMessageHandler, disposeHandler)
                : new HttpClient();
        }

        /// <summary>
        /// A normal HttpClient or one dependent upon a MessageHandler.
        /// </summary>
        /// <param name="messageHandlerKey">Optional message handler with specific rules for this client.</param>
        /// <returns></returns>
        public static HttpClient GetHttpClient(string messageHandlerKey)
        {
            if (string.IsNullOrWhiteSpace(messageHandlerKey))
            {
                throw new ArgumentNullException("messageHandlerKey");
            }

            // Determine which message handler we should use.
            var httpMessageHandler = MessageHandlers.Value[messageHandlerKey];

            // Behold! Our HttpClient instance, based on the httpMessageHandler!
            return GetHttpClient(httpMessageHandler.HttpMessageHandler, httpMessageHandler.DisposeHandler);
        }
    }
}