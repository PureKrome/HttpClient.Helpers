using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class FakeMessageHandlerFacts
    {
        public class FakeHttpMessageHandlerFacts
        {
            [Fact]
            public async Task GivenAnHttpResponseMessage_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string requestUrl = "http://www.something.com/some/website";
                const string responseData = "I am not some Html.";
                var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);
                var messageHandler = new FakeHttpMessageHandler(requestUrl, messageResponse);

                HttpResponseMessage message;
                string content;
                using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
                {
                    // Act.
                    message = await httpClient.GetAsync(requestUrl);
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData);
            }

            [Fact]
            public async Task GivenAnHttpResponseMessageAndTheHttpClientFactory_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string requestUrl = "http://www.something.com/some/website";
                const string responseData = "I am not some Html.";
                var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);
                HttpClientFactory.MessageHandler = new FakeHttpMessageHandler(requestUrl, messageResponse);

                HttpResponseMessage message;
                string content;
                using (var httpClient = HttpClientFactory.GetHttpClient())
                {
                    // Act.
                    message = await httpClient.GetAsync(requestUrl);
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData);
            }

            [Fact]
            public async Task GivenAFewHttpResponseMessages_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string requestUrl1 = "http://www.something.com/some/website";
                const string responseData1 = "I am not some Html.";
                var messageResponse1 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData1);

                const string requestUrl2 = "http://www.something.com/another/site";
                const string responseData2 = "Html, I am not.";
                var messageResponse2 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData2);

                const string requestUrl3 = "http://www.whatever.com/";
                const string responseData3 = "<html><head><body>pew pew</body></head>";
                var messageResponse3 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData3);

                var messageResponses = new Dictionary<string, HttpResponseMessage>
                {
                    {requestUrl1, messageResponse1},
                    {requestUrl2, messageResponse2},
                    {requestUrl3, messageResponse3}
                };

                var messageHandler = new FakeHttpMessageHandler(messageResponses);
                
                HttpResponseMessage message;
                string content;
                using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
                {
                    // Act.
                    message = await httpClient.GetAsync(requestUrl2);
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData2);
            }

            [Fact]
            public async Task GivenAFewHttpResponseMessagesAndTheHttpClientFactory_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string requestUrl1 = "http://www.something.com/some/website";
                const string responseData1 = "I am not some Html.";
                var messageResponse1 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData1);

                const string requestUrl2 = "http://www.something.com/another/site";
                const string responseData2 = "Html, I am not.";
                var messageResponse2 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData2);

                const string requestUrl3 = "http://www.whatever.com/";
                const string responseData3 = "<html><head><body>pew pew</body></head>";
                var messageResponse3 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData3);

                var messageResponses = new Dictionary<string, HttpResponseMessage>
                {
                    {requestUrl1, messageResponse1},
                    {requestUrl2, messageResponse2},
                    {requestUrl3, messageResponse3}
                };

                HttpClientFactory.MessageHandler = new FakeHttpMessageHandler(messageResponses);

                HttpResponseMessage message;
                string content;
                using (var httpClient = HttpClientFactory.GetHttpClient())
                {
                    // Act.
                    message = await httpClient.GetAsync(requestUrl2);
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData2);
            }

            [Fact]
            public async Task GivenAnHttpResponseMessageAndNoRequestUri_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string responseData = "I am not some Html.";
                var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

                // No RequestUri == a wildcard == any url given.
                var messageHandler = new FakeHttpMessageHandler(messageResponse);

                HttpResponseMessage message;
                string content;
                using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
                {
                    // Act.
                    message = await httpClient.GetAsync("http://pewpew.com");
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData);
            }

            [Fact]
            public async Task GivenAnHttpResponseMessageAndNoRequestUriAndTheHttpClientFactory_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string responseData = "I am not some Html.";
                var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

                // No RequestUri == a wildcard == any url given.
                HttpClientFactory.MessageHandler = new FakeHttpMessageHandler(messageResponse);

                HttpResponseMessage message;
                string content;
                using (var httpClient = HttpClientFactory.GetHttpClient())
                {
                    // Act.
                    message = await httpClient.GetAsync("http://pewpew.com");
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData);
            }

            [Fact]
            public async Task GivenAFewHttpResponseMessagesWithAWildcard_GetAsync_ReturnsTheWildcardResponse()
            {
                // Arrange.
                const string requestUrl1 = "http://www.something.com/some/website";
                const string responseData1 = "I am not some Html.";
                var messageResponse1 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData1);

                const string requestUrl2 = "http://www.something.com/another/site";
                const string responseData2 = "Html, I am not.";
                var messageResponse2 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData2);

                const string requestUrl3 = "*";
                const string responseData3 = "<html><head><body>pew pew</body></head>";
                var messageResponse3 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData3);

                var messageResponses = new Dictionary<string, HttpResponseMessage>
                {
                    {requestUrl1, messageResponse1},
                    {requestUrl2, messageResponse2},
                    {requestUrl3, messageResponse3}
                };

                var messageHandler = new FakeHttpMessageHandler(messageResponses);

                HttpResponseMessage message;
                string content;
                using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
                {
                    // Act.
                    message = await httpClient.GetAsync("http://pewpew.com");
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData3);
            }

            [Fact]
            public async Task GivenAFewHttpResponseMessagesWithAWildcardAndTheHttpClientFactory_GetAsync_ReturnsTheWildcardResponse()
            {
                // Arrange.
                const string requestUrl1 = "http://www.something.com/some/website";
                const string responseData1 = "I am not some Html.";
                var messageResponse1 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData1);

                const string requestUrl2 = "http://www.something.com/another/site";
                const string responseData2 = "Html, I am not.";
                var messageResponse2 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData2);

                const string requestUrl3 = "*";
                const string responseData3 = "<html><head><body>pew pew</body></head>";
                var messageResponse3 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData3);

                var messageResponses = new Dictionary<string, HttpResponseMessage>
                {
                    {requestUrl1, messageResponse1},
                    {requestUrl2, messageResponse2},
                    {requestUrl3, messageResponse3}
                };

                HttpClientFactory.MessageHandler = new FakeHttpMessageHandler(messageResponses);

                HttpResponseMessage message;
                string content;
                using (var httpClient = HttpClientFactory.GetHttpClient())
                {
                    // Act.
                    message = await httpClient.GetAsync("http://pewpew.com");
                    content = await message.Content.ReadAsStringAsync();
                }

                // Assert.
                message.StatusCode.ShouldBe(HttpStatusCode.OK);
                content.ShouldBe(responseData3);
            }

            [Fact]
            public void GivenAnHttpClientException_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string errorMessage = "Oh man - something bad happened.";
                var exception = new HttpRequestException(errorMessage);
                var messageHandler = new FakeHttpMessageHandler(exception);

                HttpRequestException result;
                using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
                {
                    // Act.
                    result =
                        Should.Throw<HttpRequestException>(
                            async () => await httpClient.GetAsync("http://www.something.com/some/website"));
                }

                // Assert.
                result.Message.ShouldBe(errorMessage);
            }

            [Fact]
            public void GivenAnHttpClientExceptionAndTheHttpClientFactory_GetAsync_ReturnsAFakeResponse()
            {
                // Arrange.
                const string errorMessage = "Oh man - something bad happened.";
                var exception = new HttpRequestException(errorMessage);
                HttpClientFactory.MessageHandler = new FakeHttpMessageHandler(exception);

                HttpRequestException result;
                using (var httpClient = HttpClientFactory.GetHttpClient())
                {
                    // Act.
                    result = Should.Throw<HttpRequestException>(
                            async () => await httpClient.GetAsync("http://www.something.com/some/website"));
                }

                // Assert.
                result.Message.ShouldBe(errorMessage);
            }
        }
    }
}