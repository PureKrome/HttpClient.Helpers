using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shouldly;
using WorldDomination.Net.Http;

namespace WorldDomination.HttpClient.Helpers.Tests.Wiki_Examples
{
    public class MyService
    {
        public const string GetFooEndPoint = "http://www.something.com/some/website";
        public const string GetBaaEndPoint = "http://www.something.com/another/site";
        private readonly FakeHttpMessageHandler _messageHandler;

        public MyService(FakeHttpMessageHandler messageHandler = null)
        {
            _messageHandler = messageHandler;
        }

        public async Task<Foo> GetAllDataAsync()
        {
            var foo = await GetSomeFooDataAsync();
            foo.Baa = await GetSomeBaaDataAsync();

            return foo;
        }

        public async Task<Foo> GetSomeFooDataAsync()
        {
            return await GetSomeDataAsync<Foo>(GetFooEndPoint);
        }

        public async Task<Baa> GetSomeBaaDataAsync()
        {
            // NOTE: notice how this request endpoint is different to the one, above?
            return await GetSomeDataAsync<Baa>(GetBaaEndPoint);
        }

        private async Task<T> GetSomeDataAsync<T>(string endPoint)
        {
            endPoint.ShouldNotBeNullOrWhiteSpace();

            HttpResponseMessage message;
            string content;
            using (var httpClient = _messageHandler == null
                 ? new System.Net.Http.HttpClient()
                 : new System.Net.Http.HttpClient(_messageHandler))
            {
                message = await httpClient.GetAsync(endPoint);
                content = await message.Content.ReadAsStringAsync();
            }

            if (message.StatusCode != HttpStatusCode.OK)
            {
                // TODO: handle this ru-roh-error.
                throw new InvalidOperationException(content);
            }

            // Assumption: content is in a json format.
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
