using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;
using System;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class GetStringAsyncTests
    {
        [Theory]
        [InlineData("http://www.something.com/some/website")] // Simple uri.
        [InlineData("http://www.something.com/some/website?a=1&b=2")] // Query string params.
        [InlineData("http://www.something.com/some/website?json={\"name\":\"hi\"}")] // Querystring content that needs to be encoded.
        public async Task GivenARequest_GetStringAsync_ReturnsAFakeResponse(string requestUri)
        {
            // Arrange.
            const string responseContent = "hi";
            var options = new HttpMessageOptions
            {
                HttpMethod = HttpMethod.Get,
                RequestUri = new Uri(requestUri),
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