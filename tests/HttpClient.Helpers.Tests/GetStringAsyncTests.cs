using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class GetStringAsyncTests
    {
        [Fact]
        public async Task GivenARequest_GetStringAsync_ReturnsAFakeResponse()
        {
            // Arrange.
            const string requestUri = "http://www.something.com/some/website";
            const string responseContent = "hi";
            var options = new HttpMessageOptions
            {
                HttpMethod = HttpMethod.Get,
                RequestUri = requestUri,
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseContent)
                }
            };

            var messageHandler = new FakeHttpMessageHandler(options);

            string content;
            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // Act.
                content = await httpClient.GetStringAsync(requestUri);
            }

            // Assert.
            content.ShouldBe(responseContent);
        }
    }
}