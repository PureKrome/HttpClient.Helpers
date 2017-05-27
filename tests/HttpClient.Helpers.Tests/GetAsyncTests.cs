using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Uri RequestUri = new Uri("http://www.something.com/some/website");
        private const string ExpectedContent = "pew pew";

        private static List<HttpMessageOptions> GetSomeFakeHttpMessageOptions(HttpMessageOptions option)
        {
            return new List<HttpMessageOptions>
            {
                new HttpMessageOptions
                {
                    HttpMethod = HttpMethod.Get,
                    RequestUri = new Uri("http://some/url"),
                    HttpResponseMessage = SomeFakeResponse
                },
                new HttpMessageOptions
                {
                    HttpMethod = HttpMethod.Get,
                    RequestUri = new Uri("http://another/url"),
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

                // Has to match GET + URI + Header
                yield return new object[]
                {
                    new HttpMessageOptions
                    {
                        HttpMethod = HttpMethod.Get,
                        RequestUri = RequestUri,
                        Headers = new Dictionary<string, IEnumerable<string>>
                        {
                            {"Bearer", new[]
                                {
                                    "pewpew"
                                }
                            }
                        },
                        HttpResponseMessage = SomeFakeResponse
                    }
                };

                // Has to match GET + URI + Header (but with a different case)
                yield return new object[]
                {
                    new HttpMessageOptions
                    {
                        HttpMethod = HttpMethod.Get,
                        RequestUri = RequestUri,
                        Headers = new Dictionary<string, IEnumerable<string>>
                        {
                            {"Bearer", new[]
                                {
                                    "PEWPEW"
                                }
                            }
                        },
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
                    // Any Uri but has to be a GET.
                    GetSomeFakeHttpMessageOptions(
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Get,
                            HttpResponseMessage = SomeFakeResponse
                        })
                };

                yield return new object[]
                {
                    // Has to match GET + URI
                    GetSomeFakeHttpMessageOptions(
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Get,
                            RequestUri = RequestUri,
                            HttpResponseMessage = SomeFakeResponse
                        })
                };

                yield return new object[]
                {
                    // Has to match GET + URI (case sensitive)
                    GetSomeFakeHttpMessageOptions(
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Get,
                            RequestUri = new Uri(RequestUri.AbsoluteUri.ToUpper()),
                            HttpResponseMessage = SomeFakeResponse
                        })
                };
            }
        }

        public static IEnumerable<object[]> DifferentHttpMessageOptions
        {
            get
            {
                yield return new object[]
                {
                    // Different uri.
                    new HttpMessageOptions
                    {
                        RequestUri = new Uri("http://this.is.a.different.website")
                    }
                };

                yield return new object[]
                {
                    // Different Method.
                    new HttpMessageOptions
                    {
                        HttpMethod = HttpMethod.Head
                     }
                };

                yield return new object[]
                {
                    // Different header (different key).
                    new HttpMessageOptions
                    {
                        Headers = new Dictionary<string, IEnumerable<string>>
                        {
                            {
                                "xxxx", new[]
                                {
                                    "pewpew"
                                }
                            }
                        }
                    }
                };

                yield return new object[]
                {
                    // Different header (found key, different content).
                    new HttpMessageOptions
                    {
                        Headers = new Dictionary<string, IEnumerable<string>>
                        {
                            {
                                "Bearer", new[]
                                {
                                    "pewpew"
                                }
                            }
                        }
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidHttpMessageOptions))]
        public async Task GivenAnHttpMessageOptions_GetAsync_ReturnsAFakeResponse(HttpMessageOptions options)
        {
            // Arrange.
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(options);

            // Act.
            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler,
                             options.Headers);

            // Assert.
            options.NumberOfTimesCalled.ShouldBe(1);
        }

        [Theory]
        [MemberData(nameof(ValidSomeHttpMessageOptions))]
        public async Task GivenSomeHttpMessageOptions_GetAsync_ReturnsAFakeResponse(IList<HttpMessageOptions> lotsOfOptions)
        {
            // Arrange.
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(lotsOfOptions);

            // Act & Assert.
            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
            lotsOfOptions.Sum(x => x.NumberOfTimesCalled).ShouldBe(1);
        }

        [Fact]
        public async Task GivenAnHttpResponseMessage_GetAsync_ReturnsAFakeResponse()
        {
            // Arrange.
            var httpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(ExpectedContent);
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = httpResponseMessage
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(options);

            // Act & Assert.
            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
            options.NumberOfTimesCalled.ShouldBe(1);
        }

        [Fact]
        public async Task GivenSomeHttpResponseMessages_GetAsync_ReturnsAFakeResponse()
        {
            // Arrange.
            var messageResponse1 = FakeHttpMessageHandler.GetStringHttpResponseMessage(ExpectedContent);

            const string responseData2 = "Html, I am not.";
            var messageResponse2 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData2);

            const string responseData3 = "<html><head><body>pew pew</body></head>";
            var messageResponse3 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData3);

            var options = new List<HttpMessageOptions>
            {
                new HttpMessageOptions
                {
                    RequestUri = RequestUri,
                    HttpResponseMessage = messageResponse1
                },
                new HttpMessageOptions
                {
                    RequestUri = new Uri("http://www.something.com/another/site"),
                    HttpResponseMessage = messageResponse2
                },
                new HttpMessageOptions
                {
                    RequestUri = new Uri("http://www.whatever.com/"),
                    HttpResponseMessage = messageResponse3
                },
            };

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(options);

            // Act & Assert.
            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
            options[0].NumberOfTimesCalled.ShouldBe(1);
            options[1].NumberOfTimesCalled.ShouldBe(0);
            options[2].NumberOfTimesCalled.ShouldBe(0);
        }

        [Fact]
        public async Task GivenAnUnauthorisedStatusCodeResponse_GetAsync_ReturnsAFakeResponseWithAnUnauthorisedStatusCode()
        {
            // Arrange.
            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage("pew pew", HttpStatusCode.Unauthorized);
            var options = new HttpMessageOptions
            {
                RequestUri = RequestUri,
                HttpResponseMessage = messageResponse
            };
            var messageHandler = new FakeHttpMessageHandler(options);

            HttpResponseMessage message;
            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // Act.
                message = await httpClient.GetAsync(RequestUri);
            }

            // Assert.
            message.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            options.NumberOfTimesCalled.ShouldBe(1);
        }

        [Fact]
        public async Task GivenAValidHttpRequest_GetSomeDataAsync_ReturnsAFoo()
        {
            // Arrange.
            const string errorMessage = "Oh man - something bad happened.";
            var expectedException = new HttpRequestException(errorMessage);
            var messageHandler = new FakeHttpMessageHandler(expectedException);

            Exception exception;
            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // Act.
                // NOTE: network traffic will not leave your computer because you've faked the response, above.
                exception = await Should.ThrowAsync<HttpRequestException>(async () => await httpClient.GetAsync(RequestUri));
            }

            // Assert.
            exception.Message.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task GivenAFewCallsToAnHttpRequest_GetSomeDataAsync_ReturnsAFakeResponse()
        {
            // Arrange.
            var httpResponseMessage = FakeHttpMessageHandler.GetStringHttpResponseMessage(ExpectedContent);
            var options = new HttpMessageOptions
            {
                HttpResponseMessage = httpResponseMessage
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(options);

            // Act & Assert
            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
            await DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler);
            options.NumberOfTimesCalled.ShouldBe(3);
        }

        [Theory]
        [MemberData(nameof(DifferentHttpMessageOptions))]
        public async Task GivenSomeDifferentHttpMessageOptions_GetAsync_ShouldThrowAnException(HttpMessageOptions options)
        {
            // Arrange.
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(options);
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                {
                    "hi", new[]
                    {
                        "there"
                    }
                }
            };

            // Act.
            var exception = await Should.ThrowAsync<Exception>(() => DoGetAsync(RequestUri,
                             ExpectedContent,
                             fakeHttpMessageHandler,
                             headers));

            // Assert.
            exception.Message.ShouldStartWith("No HttpResponseMessage found for the Request => ");
            options.NumberOfTimesCalled.ShouldBe(0);
        }

        private static async Task DoGetAsync(Uri requestUri,
                                             string expectedResponseContent,
                                             FakeHttpMessageHandler fakeHttpMessageHandler,
                                             IDictionary<string, IEnumerable<string>> optionalHeaders =null)
        {
            requestUri.ShouldNotBeNull();
            expectedResponseContent.ShouldNotBeNullOrWhiteSpace();
            fakeHttpMessageHandler.ShouldNotBeNull();

            HttpResponseMessage message;
            string content;
            using (var httpClient = new System.Net.Http.HttpClient(fakeHttpMessageHandler))
            {
                // Do we have any Headers?
                if (optionalHeaders != null &&
                    optionalHeaders.Any())
                {
                    foreach (var keyValue in optionalHeaders)
                    {
                        httpClient.DefaultRequestHeaders.Add(keyValue.Key, keyValue.Value);
                    }
                }

                // Act.
                message = await httpClient.GetAsync(requestUri);
                content = await message.Content.ReadAsStringAsync();
            }

            // Assert.
            message.StatusCode.ShouldBe(HttpStatusCode.OK);
            content.ShouldBe(expectedResponseContent);
        }
    }
}