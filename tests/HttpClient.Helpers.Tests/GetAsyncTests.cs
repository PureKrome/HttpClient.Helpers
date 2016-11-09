using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class GetAsyncTests
    {
        private const string RequestUri = "http://www.something.com/some/website";
        private const string ExpectedContent = "pew pew";

        private static List<HttpMessageOptions> GetSomeFakeHttpMessageOptions(HttpMessageOptions option)
        {
            return new List<HttpMessageOptions>
            {
                new HttpMessageOptions
                {
                    HttpMethod = HttpMethod.Get,
                    RequestUri = "http://some/url",
                    HttpResponseMessage = SomeFakeResponse
                },
                new HttpMessageOptions
                {
                    HttpMethod = HttpMethod.Get,
                    RequestUri = "http://another/url",
                    HttpResponseMessage = SomeFakeResponse
                },
                option
            };
        }

        private static HttpResponseMessage SomeFakeResponse => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(ExpectedContent)
        };

        public static IEnumerable<object[]> ValidHttpMessageOptions
        {
            get
            {
                yield return new object[]
                {
                    // All wildcards.
                    new HttpMessageOptions
                    {
                        HttpResponseMessage = SomeFakeResponse
                    }
                };

                // Any Uri but has to be a GET.
                yield return new object[]
                {
                    new HttpMessageOptions
                    {
                        HttpMethod = HttpMethod.Get,
                        HttpResponseMessage = SomeFakeResponse
                    }
                };

                // Has to match GET + URI.
                // NOTE: Http GET shouldn't have a content/body.
                yield return new object[]
                {
                    new HttpMessageOptions
                    {
                        HttpMethod = HttpMethod.Get,
                        RequestUri = RequestUri,
                        HttpResponseMessage = SomeFakeResponse
                    }
                };
            }
        }

        public static IEnumerable<object[]> ValidSomeHttpMessageOptions
        {
            get
            {
                yield return new object[]
                {
                    // All wildcards.
                    GetSomeFakeHttpMessageOptions(
                        new HttpMessageOptions
                        {
                            HttpResponseMessage = SomeFakeResponse
                        })
                };

                yield return new object[]
                {
                    // All wildcards.
                    GetSomeFakeHttpMessageOptions(
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Get,
                            HttpResponseMessage = SomeFakeResponse
                        })
                };

                yield return new object[]
                {
                    // All wildcards.
                    GetSomeFakeHttpMessageOptions(
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Get,
                            RequestUri = RequestUri,
                            HttpResponseMessage = SomeFakeResponse
                        })
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidHttpMessageOptions))]
        public async Task GivenAnHttpMessageOptions_GetAsync_ReturnsAFakeResponse(HttpMessageOptions option)
        {
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(option);

            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
        }

        [Theory]
        [MemberData(nameof(ValidSomeHttpMessageOptions))]
        public async Task GivenSomeHttpMessageOptions_GetAsync_ReturnsAFakeResponse(IEnumerable<HttpMessageOptions> lotsOfOption)
        {
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(lotsOfOption);

            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
        }

        private static async Task DoGetAsync(string requestUri,
                                             string expectedResponseContent,
                                             FakeHttpMessageHandler fakeHttpMessageHandler)
        {
            requestUri.ShouldNotBeNullOrWhiteSpace();
            expectedResponseContent.ShouldNotBeNullOrWhiteSpace();
            fakeHttpMessageHandler.ShouldNotBeNull();

            HttpResponseMessage message;
            string content;
            using (var httpClient = new System.Net.Http.HttpClient(fakeHttpMessageHandler))
            {
                // Act.
                message = await httpClient.GetAsync(requestUri);
                content = await message.Content.ReadAsStringAsync();
            }

            // Assert.
            message.StatusCode.ShouldBe(HttpStatusCode.OK);
            content.ShouldBe(expectedResponseContent);
        }

        [Fact]
        public async Task GivenAnHttpResponseMessage_GetAsync_ReturnsAFakeResponse()
        {
            var httpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(ExpectedContent);
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);

            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
        }

        [Fact]
        public async Task GivenSomeHttpResponseMessages_GetAsync_ReturnsAFakeResponse()
        {
            const string requestUrl1 = RequestUri;
            const string responseData1 = ExpectedContent;
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

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(messageResponses);

            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
        }
    }
}