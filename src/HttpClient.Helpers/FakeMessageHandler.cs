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

        /// <summary>
        /// A fake message handler.
        /// </summary>
        /// <remarks>TIP: If you have a requestUri = "*", this is a catch-all ... so if none of the other requestUri's match, then it will fall back to this dictionary item.</remarks>
        public FakeHttpMessageHandler(HttpMessageOptions options) : this(new List<HttpMessageOptions> { options })
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
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            var tcs = new TaskCompletionSource<HttpResponseMessage>();

            var requestUri = new Uri(request.RequestUri.AbsoluteUri);
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
                var setupOptionsText = new StringBuilder();

                var setupOptions = _lotsOfOptions.Values.ToList();
                for (var i = 0; i < setupOptions.Count; i++)
                {
                    if (i > 0)
                    {
                        setupOptionsText.Append(" ");
                    }
                    setupOptionsText.Append($"{i + 1}) {setupOptions[i]}.");
                }

                var errorMessage = $"No HttpResponseMessage found for the Request => What was called: [{option}]. At least one of these option(s) should have been matched: [{setupOptionsText}]";
                throw new InvalidOperationException(errorMessage);
            }

            // Increment the number of times this option had been 'called'.
            IncrementCalls(expectedOption);

            // Pass the request along.
            expectedOption.HttpResponseMessage.RequestMessage = request;

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

            // NOTE: We only compare the *setup* HttpMessageOptions properties if they were provided.
            //       So if there was no HEADERS provided ... but the real 'option' has some, we still ignore
            //       and don't compare.
            return _lotsOfOptions.Values.SingleOrDefault(x => (x.RequestUri == null || // Don't care about the Request Uri.
                                                               x.RequestUri.AbsoluteUri.Equals(option.RequestUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase)) &&

                                                              (x.HttpMethod == null || // Don't care about the HttpMethod.
                                                               x.HttpMethod == option.HttpMethod) &&

                                                              (x.HttpContent == null || // Don't care about the Content.
                                                               ContentAreEqual(x.HttpContent, option.HttpContent)) &&

                                                              (x.Headers == null || // Don't care about the Header.
                                                               x.Headers.Count == 0 || // No header's were supplied, so again don't care/
                                                               HeadersAreEqual(x.Headers, option.Headers)));
        }

        private static void IncrementCalls(HttpMessageOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var type = typeof(HttpMessageOptions);
            var propertyInfo = type.GetTypeInfo()
                                   ?.GetDeclaredProperty("NumberOfTimesCalled");
            //var propertyInfo = type.GetProperty("NumberOfTimesCalled");
            if (propertyInfo == null)
            {
                return;
            }

            var existingValue = (int)propertyInfo.GetValue(options);
            propertyInfo.SetValue(options, ++existingValue);
        }

        private static bool ContentAreEqual(HttpContent source, HttpContent destination)
        {
            if (source == null &&
                destination == null)
            {
                // Both are null - so they match :P
                return true;
            }

            if (source == null ||
                destination == null)
            {
                return false;
            }

            // Extract the content from both HttpContent's.
            var sourceContentTask = source.ReadAsStringAsync();
            var destinationContentTask = destination.ReadAsStringAsync();
            var tasks = new List<Task>
            {
                sourceContentTask,
                destinationContentTask
            };
            Task.WaitAll(tasks.ToArray());

            // Now compare both results.
            // NOTE: Case sensitive.
            return sourceContentTask.Result == destinationContentTask.Result;
        }

        private static bool HeadersAreEqual(IDictionary<string, IEnumerable<string>> source,
                                            IDictionary<string, IEnumerable<string>> destination)
        {
            if (source == null &&
                destination == null)
            {
                // Nothing from both .. so that's ok!
                return true;
            }

            if (source == null ||
                destination == null)
            {
                // At least one is different so don't bother checking.
                return false;
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
