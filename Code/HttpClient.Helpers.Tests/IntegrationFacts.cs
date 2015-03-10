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

        [Fact]
        public async Task GivenTwoGetRequests_GetStringAsync_ReturnsSomeData()
        {
            // Arrange.
            string html1, html2;

            HttpClientFactory.MessageHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential("api", "hi there!")
            };

            using (var httpClient = HttpClientFactory.GetHttpClient())
            {
                html1 = await httpClient.GetStringAsync("http://www.google.com.au");
            }

            using (var httpClient = HttpClientFactory.GetHttpClient())
            {
                html2 = await httpClient.GetStringAsync("http://www.google.com.au");
            }

            // Assert.
            html1.ShouldNotBeNullOrEmpty();
            html2.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task GivenAGetRequestAndDifferentNetworkCredentials_GetStringAsync_ReturnsSomeData()
        {
            // Arrange.
            string html1, html2;
            
            var credentials1 = new HttpClientHandler
            {
                Credentials = new NetworkCredential("api", "hi there!")
            };
            var credentials2 = new HttpClientHandler
            {
                Credentials = new NetworkCredential("cccc", "ddddd")
            };

            // Act.
            using (var httpClient = HttpClientFactory.GetHttpClient(credentials1))
            {
                html1 = await httpClient.GetStringAsync("http://www.google.com.au");
            }

            using (var httpClient = HttpClientFactory.GetHttpClient(credentials2))
            {
                html2 = await httpClient.GetStringAsync("http://www.google.com.au");
            }

            // Assert.
            html1.ShouldNotBeNullOrEmpty();
            html2.ShouldNotBeNullOrEmpty();
        }
    }
}