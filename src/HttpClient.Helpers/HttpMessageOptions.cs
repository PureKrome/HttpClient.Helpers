using System;
using System.Net.Http;

namespace WorldDomination.Net.Http
{
    public class HttpMessageOptions
    {
        public const string NoValue = "*";
        private HttpContent _httpContent;
        private string _httpContentSerialized;
        private string _requestUri = NoValue;

        /// <summary>
        /// Required: End url we are targetting.
        /// </summary>
        public string RequestUri
        {
            get { return _requestUri; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value),
                        "RequestUri cannot be null/empty/whitespace. Please choose a valid RequestUri.");
                }

                _requestUri = value;
            }
        }

        /// <summary>
        /// Optional: If not provided, then assumed to be *any* method.
        /// </summary>
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// Required: Need to know what type of response we will return.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }

        /// <summary>
        /// Optional: If not provided, then assumed to be *no* content.
        /// </summary>
        public HttpContent HttpContent
        {
            get { return _httpContent; }
            set
            {
                _httpContent = value;
                _httpContentSerialized = _httpContent?.ReadAsStringAsync().Result ?? NoValue;
            }
        }

        public override string ToString()
        {
            var httpMethodText = HttpMethod?.ToString() ?? NoValue;
            return
                $"{httpMethodText} {RequestUri}{(HttpContent != null ? $" body/content: {_httpContentSerialized}" : "")}";
        }
    }
}