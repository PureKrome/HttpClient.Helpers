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
using Xunit;

// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class PutAsyncTests
    {
        public static IEnumerable<object[]> ValidPutHttpContent
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
                    new StringContent(JsonConvert.SerializeObject(DateTime.UtcNow.ToString("o")) , Encoding.UTF8)
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

        public static IEnumerable<object[]> VariousOptions
        {
            get
            {
                yield return new object[]
                {
                    new []
                    {
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Put,
                            RequestUri = new Uri("http://www.something.com/some/website"),
                            HttpContent = new StringContent(JsonConvert.SerializeObject(DateTime.UtcNow)),
                            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent)
                        }
                    }
                };

                // Two options setup with two different Request Uri's.
                yield return new object[]
                {
                    new []
                    {
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Put,
                            RequestUri = new Uri("http://www.something.com/some/website"),
                            HttpContent = new StringContent(JsonConvert.SerializeObject(DateTime.UtcNow)),
                            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent)
                        },
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Put,
                            RequestUri = new Uri("http://www.1.2.3.4/a/b"),
                            HttpContent = new StringContent(JsonConvert.SerializeObject(DateTime.UtcNow)),
                            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent)
                        }
                    }
                };

                // Two options setup with two different Request Uri's.
                yield return new object[]
                {
                    new []
                    {
                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Put,
                            RequestUri = new Uri("http://www.something.com/some/website"),
                            HttpContent = new StringContent(JsonConvert.SerializeObject(DateTime.UtcNow)),
                            Headers = new Dictionary<string, IEnumerable<string>>
                            {
                                {
                                    "Bearer", new[]
                                    {
                                        "pewpew",
                                        "1234"
                                    }
                                }
                            },
                            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent)
                        },

                        new HttpMessageOptions
                        {
                            HttpMethod = HttpMethod.Put,
                            RequestUri = new Uri("http://www.1.2.3.4/a/b"),
                            HttpContent = new StringContent(JsonConvert.SerializeObject(DateTime.UtcNow)),
                            Headers = new Dictionary<string, IEnumerable<string>>
                            {
                                {
                                    "Bearer", new[]
                                    {
                                        "pewpew",
                                        "1234"
                                    }
                                }
                            },
                            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent)
                        }
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidPutHttpContent))]
        public async Task GivenAPutRequest_PutAsync_ReturnsAFakeResponse(HttpContent content)
        {
            // Arrange.
            var requestUri = new Uri("http://www.something.com/some/website");
            var options = new HttpMessageOptions
            {
                HttpMethod = HttpMethod.Put,
                RequestUri = requestUri,
                HttpContent = content,
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent)
            };

            var messageHandler = new FakeHttpMessageHandler(options);

            HttpResponseMessage message;
            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // Act.
                message = await httpClient.PutAsync(requestUri, content);
            }

            // Assert.
            message.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// The actual httpClient call fails to match anything setup with the 'options'.
        /// </summary>
        [Theory]
        [MemberData(nameof(VariousOptions))]
        public async Task GivenADifferentPutRequestAndExpectedOutcome_PutAsync_ThrowsAnException(IEnumerable<HttpMessageOptions> options)
        {
            // Arrange.
            var content = new StringContent("hi");
            var messageHandler = new FakeHttpMessageHandler(options);

            InvalidOperationException exception;
            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // Act.
                exception = await Should.ThrowAsync<InvalidOperationException>( async () => await httpClient.PutAsync("http://a.b.c.d./abcde", content));
            }

            // Act.
            exception.ShouldNotBeNull();
            exception.Message.ShouldStartWith("No HttpResponseMessage found for the Request => What was called:");
            exception.Message.ShouldContain($"{options.Count()}) "); // e.g. 1)  or   2)  .. etc.
        }
    }
}
