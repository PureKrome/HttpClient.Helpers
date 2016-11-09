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
        public FakeHttpMessageHandler(string requestUri,
                                      HttpResponseMessage httpResponseMessage)
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
        public FakeHttpMessageHandler(HttpMessageOptions options) : this(new List<HttpMessageOptions>
                                                                         {
                                                                             options
                                                                         })
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

            // NOTE: We assume HttpGet is the default when none are provided in this 'shortcut' method.
            var lotsOfOptions = responses.Select(item => new HttpMessageOptions
                                                 {
                                                     RequestUri = item.Key,
                                                     HttpResponseMessage = item.Value,
                                                     HttpMethod = HttpMethod.Get
                                                 }).ToArray();

            Initialize(lotsOfOptions);
        }

        public FakeHttpMessageHandler(IEnumerable<HttpMessageOptions> lotsOfOptions)
        {
            Initialize(lotsOfOptions.ToArray());
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
                throw new ArgumentNullException(nameof(exception));
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

            var requestUri = request.RequestUri.AbsoluteUri;
            var option = new HttpMessageOptions
            {
                RequestUri = requestUri,
                HttpMethod = request.Method,
                HttpContent = request.Content
            };

            var expectedOption = GetExpectedOption(option);
            if (expectedOption == null)
            {
                // Nope - no keys found exactly OR starting-with...

                var responsesText = string.Join(";", _lotsOfOptions.Values);
                var errorMessage =
                    $"No HttpResponseMessage found for the Request Uri: {request.RequestUri}. Please provide one in the FakeHttpMessageHandler constructor Or use a '*' for any request uri. Search-Key: '{requestUri}. Setup: {(!_lotsOfOptions.Any() ? "- no responses -" : _lotsOfOptions.Count.ToString())} responses: {responsesText}";
                throw new InvalidOperationException(errorMessage);
            }

            tcs.SetResult(expectedOption.HttpResponseMessage);
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

        private void Initialize(HttpMessageOptions[] lotsOfOptions)
        {
            if (lotsOfOptions == null)
            {
                throw new ArgumentNullException(nameof(lotsOfOptions));
            }

            if (!lotsOfOptions.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(lotsOfOptions),
                                                      "Need at least _one_ expected request/response (a.k.a. HttpMessageOptions) setup.");
            }

            // We need to make sure the requests are unique.
            foreach (var option in lotsOfOptions)
            {
                if (_lotsOfOptions.ContainsKey(option.ToString()))
                {
                    throw new InvalidOperationException(
                        $"Trying to add a request/response (a.k.a. HttpMessageOptions) which has already been setup. Can only have one unique request/response, setup. Unique info: {option}");
                }

                _lotsOfOptions.Add(option.ToString(), option);
            }
        }

        private HttpMessageOptions GetExpectedOption(HttpMessageOptions option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            return _lotsOfOptions.Values.SingleOrDefault(x => (x.RequestUri == option.RequestUri ||
                                                               x.RequestUri == HttpMessageOptions.NoValue) &&
                                                              (x.HttpMethod == option.HttpMethod ||
                                                               x.HttpMethod == null) &&
                                                              (x.HttpContent == option.HttpContent ||
                                                               x.HttpContent == null));
        }
    }
}