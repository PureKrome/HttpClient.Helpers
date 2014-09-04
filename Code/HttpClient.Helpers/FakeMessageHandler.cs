using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorldDomination.Net.Http
{
    public class FakeHttpMessageHandler : HttpClientHandler
    {
        private readonly HttpRequestException _exception;
        private readonly IDictionary<string, HttpResponseMessage> _responses;

        /// <summary>
        /// A fake message handler.
        /// </summary>
        /// <remarks>This is mainly used for unit testing purposes.</remarks>
        /// <param name="requestUri">The endpoint the HttpClient would normally try and connect to.</param>
        /// <param name="httpResponseMessage">The faked response message.</param>
        public FakeHttpMessageHandler(string requestUri, HttpResponseMessage httpResponseMessage)
            : this(new Dictionary<string, HttpResponseMessage>
            {
                {requestUri, httpResponseMessage}
            })
        {
        }

        /// <summary>
        /// A fake message handler.
        /// </summary>
        /// <remarks>TIP: If you have a requestUri = "*", this is a catch-all ... so if none of the other requestUri's match, then it will fall back to this dictionary item.</remarks>
        /// <param name="httpResponseMessages">A dictionary of request endpoints and their respective fake response message.</param>
        public FakeHttpMessageHandler(IDictionary<string, HttpResponseMessage> httpResponseMessages)
        {
            if (httpResponseMessages == null)
            {
                throw new ArgumentNullException("httpResponseMessages");
            }

            if (!httpResponseMessages.Any())
            {
                throw new ArgumentOutOfRangeException("httpResponseMessages");
            }

            _responses = httpResponseMessages;
        }

        /// <summary>
        /// A fake message handler which ignores whatever endpoint you're trying to connect to.
        /// </summary>
        /// <remarks>This constructor doesn't care what the request endpoint it. So if you're code is trying to hit multuple endpoints, then it will always return the same response message.</remarks>
        /// <param name="httpResponseMessage">The faked response message.</param>
        public FakeHttpMessageHandler(HttpResponseMessage httpResponseMessage)
            : this(new Dictionary<string, HttpResponseMessage>
            {
                {"*", httpResponseMessage}
            })
        {
        }

        /// <summary>
        /// A fake message handler for an exception.
        /// </summary>
        /// <remarks>This is mainly used for unit testing exceptions when the HttpClient fails.</remarks>
        /// <param name="exception">The exception that will occur.</param>
        public FakeHttpMessageHandler(HttpRequestException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            _exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            var tcs = new TaskCompletionSource<HttpResponseMessage>();

            HttpResponseMessage response;
            var requestUri = request.RequestUri.ToString();

            // Determine the Response message based upon the Request uri.
            // 1. Exact match.
            // 2. Wildcard '*' == I don't care what the Uri is, just use this Response.
            // 3. Starts with == this is so we don't have to have a huge string in our test case. Just keeping code a bit cleaner.
            if (!_responses.TryGetValue(requestUri, out response) &&
                !_responses.TryGetValue("*", out response))
            {
                foreach (var key in _responses.Keys.Where(requestUri.StartsWith))
                {
                    response = _responses[key];
                    break;
                }

                if (response == null)
                {
                    // Nope - no keys found exactly OR starting with...
                    var errorMessage =
                        string.Format(
                            "No HttpResponseMessage found for the Request Uri: {0}. Please provide one in the FakeHttpMessageHandler constructor Or use a '*' for any request uri.",
                            request.RequestUri);
                    throw new InvalidOperationException(errorMessage);
                }
            }

            tcs.SetResult(response);
            return tcs.Task;
        }

        /// <summary>
        /// Helper method to easily return a simple HttpResponseMessage.
        /// </summary>
        public static HttpResponseMessage GetStringHttpResponseMessage(string content,
            HttpStatusCode httpStatusCode = HttpStatusCode.OK,
            string mediaType = "application/json")
        {
            return new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                Content = new StringContent(content, Encoding.UTF8, mediaType)
            };
        }
    }
}