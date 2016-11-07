using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;
// ReSharper disable ConsiderUsingConfigureAwait

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class IntegrationFacts
    {
        public class AddMessageHandlerFacts
        {
            [Fact]
            public async Task GivenAGetRequest_GetStringAsync_ReturnsSomeData()
            {
                // Arrange.
                string html;

                var messageHandler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential("api", "hi there!")
                };

                var key = Guid.NewGuid().ToString();
                HttpClientFactory.AddMessageHandler(messageHandler, key);

                using (var httpClient = HttpClientFactory.GetHttpClient(key))
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

                var messageHandler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential("api", "hi there!")
                };

                var key = Guid.NewGuid().ToString();
                HttpClientFactory.AddMessageHandler(messageHandler, key, false);

                using (var httpClient = HttpClientFactory.GetHttpClient(key))
                {
                    html1 = await httpClient.GetStringAsync("http://www.google.com.au");
                }

                using (var httpClient = HttpClientFactory.GetHttpClient(key))
                {
                    html2 = await httpClient.GetStringAsync("http://www.google.com.au");
                }

                // Assert.
                html1.ShouldNotBeNullOrEmpty();
                html2.ShouldNotBeNullOrEmpty();
            }

            [Fact]
            public async Task GivenTwoGetRequestsButFailedToReuseHandler_GetStringAsync_ShouldThrowAnException()
            {
                // Arrange.
                string html1;

                var messageHandler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential("api", "hi there!")
                };

                var key = Guid.NewGuid().ToString();
                HttpClientFactory.AddMessageHandler(messageHandler, key); // Handler is disposed after first used.

                using (var httpClient = HttpClientFactory.GetHttpClient(key))
                {
                    html1 = await httpClient.GetStringAsync("http://www.google.com.au");
                }

                Exception exception;
                using (var httpClient = HttpClientFactory.GetHttpClient(key))
                {
                    var client = httpClient;
                    exception = Should.Throw<Exception>(
                        async () => await client.GetStringAsync("http://www.google.com.au"));
                }

                // Assert.
                html1.ShouldNotBeNullOrEmpty();
                exception.Message.ShouldStartWith("Cannot access a disposed object.");
            }
        }

        public class RemoveMessageHanderFacts
        {
            [Fact]
            public void GivenAMessageHandlerExists_RemoveMessageHandler_Succeeds()
            {
                // Arrange.
                var messageHandler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential("api", "hi there!")
                };

                var key = Guid.NewGuid().ToString();
                HttpClientFactory.AddMessageHandler(messageHandler, key);

                // Act & Assert.
                HttpClientFactory.RemoveMessageHandler(key);
            }

            [Fact]
            public void GivenAMessageHandlerExistsByADifferentKeyIsProvided_RemoveMessageHandler_Succeeds()
            {
                // Arrange.
                var messageHandler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential("api", "hi there!")
                };

                HttpClientFactory.AddMessageHandler(messageHandler, Guid.NewGuid().ToString());

                // Act & Assert.
                HttpClientFactory.RemoveMessageHandler(Guid.NewGuid().ToString());
            }

            [Fact]
            public void GivenNoMessageHandlerExists_RemoveMessageHandler_Succeeds()
            {
                // Arrange.
                var key = Guid.NewGuid().ToString();

                // Act & Assert.
                HttpClientFactory.RemoveMessageHandler(key);
            }
        }

        public class HttpMessageParameterFacts
        {
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
}