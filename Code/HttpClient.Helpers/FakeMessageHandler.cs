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
        private readonly IDictionary<string, HttpMessageOptions> _lotsOfOptions = new Dictionary<string, HttpMessageOptions>();

        /// <summary>
        /// A fake message handler.
        /// </summary>
        /// <remarks>This is mainly used for unit testing purposes.</remarks>
        /// <param name="requestUri">The endpoint the HttpClient would normally try and connect to.</param>
        /// <param name="httpResponseMessage">The faked response message.</param>
        public FakeHttpMessageHandler(string requestUri, HttpResponseMessage httpResponseMessage)
            : this(new HttpMessageOptions
            {
                RequestUri = requestUri,
                HttpResponseMessage = httpResponseMessage
            })
        {
        }

        /// <summary>
        /// A fake message handler.
        /// </summary>
        /// <remarks>TIP: If you have a requestUri = "*", this is a catch-all ... so if none of the other requestUri's match, then it will fall back to this dictionary item.</remarks>
        ///// <param name="httpResponseMessages">A dictionary of request endpoints and their respective fake response message.</param>
        //public FakeHttpMessageHandler(IDictionary<string, HttpResponseMessage> httpResponseMessages)
        //{
        //    if (httpResponseMessages == null)
        //    {
        //        throw new ArgumentNullException(nameof(httpResponseMessages));
        //    }

        //    if (!httpResponseMessages.Any())
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(httpResponseMessages));
        //    }

        //    _responses = httpResponseMessages;
        //}

        public FakeHttpMessageHandler(HttpMessageOptions options) : this(new List<HttpMessageOptions> { options})
        {
        }

        public FakeHttpMessageHandler(IDictionary<string, HttpResponseMessage> responses)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            if (!responses.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(responses));
            }

            var options = responses.Select(item => new HttpMessageOptions
            {
                RequestUri = item.Key,
                HttpResponseMessage = item.Value
            });

            Initialize(options);
        }

        public FakeHttpMessageHandler(IEnumerable<HttpMessageOptions> lotsOfOptions)
        {
            Initialize(lotsOfOptions);
        }

        /// <summary>
        /// A fake message handler which ignores whatever endpoint you're trying to connect to.
        /// </summary>
        /// <remarks>This constructor doesn't care what the request endpoint it. So if you're code is trying to hit multuple endpoints, then it will always return the same response message.</remarks>
        /// <param name="httpResponseMessage">The faked response message.</param>
        public FakeHttpMessageHandler(HttpResponseMessage httpResponseMessage)
            : this(new HttpMessageOptions
            {
                HttpResponseMessage = httpResponseMessage
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

            HttpMessageOptions options;
            var requestUri = request.RequestUri.ToString();

            // If we don't care 
            var uniqueKey = CreateDictionaryKey(requestUri, request.Method);
            var wildcardKey = CreateDictionaryKey("*", HttpMethod.Get);

            // Determine the Response message based upon the Request uri && HttpMethod.
            // 1. Exact match.
            // 2. Wildcard '*' == I don't care what the Uri is, just use this Response.
            // 3. Starts with == this is so we don't have to have a huge string in our test case. Just keeping code a bit cleaner.

            // 1) & 3) checks.
            if (!_lotsOfOptions.TryGetValue(uniqueKey, out options) &&
                !_lotsOfOptions.TryGetValue(wildcardKey, out options))
            {
                // 2) Starts-with check.
                foreach (var key in _lotsOfOptions.Keys.Where(uniqueKey.StartsWith))
                {
                    options = _lotsOfOptions[key];
                    break;
                }

                if (options == null)
                {
                    // Nope - no keys found exactly OR starting-with...

                    var responsesText = !_lotsOfOptions.Any()
                        ? "-none-"
                        : string.Join(";", _lotsOfOptions.Values);

                    var errorMessage =
                        string.Format(
                            "No HttpResponseMessage found for the Request Uri: {0}. Please provide one in the FakeHttpMessageHandler constructor Or use a '*' for any request uri. Search-Key: '{1}. Setup: {2} responses: {3}",
                            request.RequestUri,
                            requestUri,
                            !_lotsOfOptions.Any()
                                ? "- no responses -"
                                : _lotsOfOptions.Count.ToString(),
                                responsesText);
                    throw new InvalidOperationException(errorMessage);
                }
            }

            tcs.SetResult(options.HttpResponseMessage);
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

        private void Initialize(IEnumerable<HttpMessageOptions> lotsOfOptions)
        {
            if (lotsOfOptions == null)
            {
                throw new ArgumentNullException(nameof(lotsOfOptions));
            }

            if (!lotsOfOptions.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(lotsOfOptions));
            }

            foreach (var option in lotsOfOptions)
            {
                var key = CreateDictionaryKey(option.RequestUri, option.HttpMethod);
                _lotsOfOptions[key] = option;
            }
        }

        private static string CreateDictionaryKey(string requestUri, HttpMethod httpMethod)
        {
            var httpMethodText = httpMethod?.ToString() ?? "*";
            return string.Format($"{requestUri}||{httpMethodText}");
        }
    }
}