using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class IntegrationFacts
    {
        // This test checks to see if a normal integration test succeeds/fails.
        [Fact]
        public async Task GivenAGetRequest_GetStringAsync_ReturnsSomeData()
        {
            // Arrange.
            string html;

            HttpClientFactory.MessageHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential("api", "hi there!")
            };

            using (var httpClient = HttpClientFactory.GetHttpClient())
            {
                html = await httpClient.GetStringAsync("http://www.google.com.au");
            }

            // Assert.
            html.ShouldNotBeNullOrEmpty();
        }
    }
}