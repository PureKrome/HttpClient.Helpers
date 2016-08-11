using System;
using System.Net.Http;

namespace WorldDomination.Net.Http
{
    public class HttpMessageOptions
    {
        private HttpMethod _httpMethod = HttpMethod.Get;
        private string _requestUri = "*";

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

        public HttpMethod HttpMethod
        {
            get { return _httpMethod; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value),
                        "HttpMethod cannot be null. Please choose a valid HttpMethod.");
                }

                _httpMethod = value;
            }
        }

        public HttpResponseMessage HttpResponseMessage { get; set; }

        public override string ToString()
        {
            var httpMethodText = HttpMethod?.ToString() ?? "*";
            return $"{httpMethodText} {RequestUri}";
        }
    }
}