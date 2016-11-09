using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;
// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class PostAsyncTests
    {
        public static IEnumerable<object[]> ValidPostHttpContent
        {
            get
            {
                yield return new object[]
                {
                    // Sample json.
                    new StringContent("{\"id\":1}", Encoding.UTF8)
                };

                yield return new object[]
                {
                    // Form key/values.
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("a", "b"),
                        new KeyValuePair<string, string>("c", "1")
                    })
                };
            }
        }

        public static IEnumerable<object[]> InvalidPostHttpContent
        {
            get
            {
                yield return new object[]
                {
                    // Sample json.
                    new StringContent("{\"id\":1}", Encoding.UTF8),
                    new StringContent("{\"id\":2}", Encoding.UTF8)
                };

                yield return new object[]
                {
                    // Form key/values.
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("a", "b"),
                        new KeyValuePair<string, string>("c", "1")
                    }),
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("2", "1")
                    })
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidPostHttpContent))]
        public async Task GivenAPostRequest_PostAsync_ReturnsAFakeResponse(HttpContent httpContent)
        {
            // Arrange.
            const string requestUri = "http://www.something.com/some/website";
            const string responseContent = "hi";
            var options = new HttpMessageOptions
            {
                HttpMethod = HttpMethod.Post,
                RequestUri = requestUri,
                HttpContent = httpContent,
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseContent)
                }
            };

            var messageHandler = new FakeHttpMessageHandler(options);

            HttpResponseMessage message;
            string content;
            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // Act.
                message = await httpClient.PostAsync(requestUri, httpContent);
                content = await message.Content.ReadAsStringAsync();
            }

            // Assert.
            message.StatusCode.ShouldBe(HttpStatusCode.OK);
            content.ShouldBe(responseContent);
        }

        [Theory]
        [MemberData(nameof(InvalidPostHttpContent))]
        public async Task GivenAPostRequestWithIncorrectlySetupOptions_PostAsync_ThrowsAnException(HttpContent expectedHttpContent,
            HttpContent sentHttpContent)
        {
            // Arrange.
            const string responseContent = "hi";
            var options = new HttpMessageOptions
            {
                HttpMethod = HttpMethod.Post,
                RequestUri = "http://www.something.com/some/website",
                HttpContent = expectedHttpContent,
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseContent)
                }
            };

            var messageHandler = new FakeHttpMessageHandler(options);
            InvalidOperationException exception;
            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // Act.
                exception = await Should.ThrowAsync<InvalidOperationException>(async () => await httpClient.PostAsync("http://www.something.com/some/website", sentHttpContent));
            }

            // Assert.
            exception.Message.ShouldStartWith("No HttpResponseMessage found");
        }
    }
}
