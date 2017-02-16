using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorldDomination.Net.Http
{
    public class FakeHttpMessageHandler : HttpClientHandler
    {
        private readonly HttpRequestException _exception;

        private readonly IDictionary<string, HttpMessageOptions> _lotsOfOptions = new Dictionary<string, HttpMessageOptions>();

        private readonly IEqualityComparer<HttpContent> _httpContentEqualityComparer = new HttpContentEqualityComparer();

        /// <summary>
        /// A fake message handler.
        /// </summary>
        /// <remarks>TIP: If you have a requestUri = "*", this is a catch-all ... so if none of the other requestUri's match, then it will fall back to this dictionary item.</remarks>
        public FakeHttpMessageHandler(HttpMessageOptions options) : this(new List<HttpMessageOptions>
        {
            options
        })
        {
        }

        public FakeHttpMessageHandler(IEnumerable<HttpMessageOptions> lotsOfOptions)
        {
            Initialize(lotsOfOptions.ToArray());
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
                HttpContent = request.Content,
                Headers = request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value)
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

            // Increment the number of times this option had been 'called'.
            IncrementCalls(expectedOption);

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

            IEnumerable<HttpMessageOptions> options = _lotsOfOptions.Values;

            options = options.Where(x => x.RequestUri == option.RequestUri || x.RequestUri == HttpMessageOptions.NoValue);

            options = options.Where(x => x.HttpMethod == option.HttpMethod || x.HttpMethod == null);

            options = options.Where(x => _httpContentEqualityComparer.Equals(x.HttpContent, option.HttpContent));

            options = options.Where(x => x.Headers == null || x.Headers.Count == 0 || (x.Headers != null && HeaderExists(x.Headers, option.Headers)));

            return options.SingleOrDefault();
        }

        private static void IncrementCalls(HttpMessageOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var type = typeof(HttpMessageOptions);
            var propertyInfo = type.GetTypeInfo().GetProperty("NumberOfTimesCalled");
            if (propertyInfo == null)
            {
                return;
            }

            var existingValue = (int) propertyInfo.GetValue(options);
            propertyInfo.SetValue(options, ++existingValue);
        }

        private static bool HeaderExists(IDictionary<string, IEnumerable<string>> source,
                                         IDictionary<string, IEnumerable<string>> destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            // Both sides are not the same size.
            if (source.Count != destination.Count)
            {
                return false;
            }

            foreach (var key in source.Keys)
            {
                if (!destination.ContainsKey(key))
                {
                    // Key is missing from the destination.
                    return false;
                }

                if (source[key].Count() != destination[key].Count())
                {
                    // The destination now doesn't have the same size of 'values'.
                    return false;
                }

                foreach (var value in source[key])
                {
                    if (!destination[key].Contains(value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}