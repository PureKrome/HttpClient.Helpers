using System;
using System.Net;
using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;

namespace WorldDomination.HttpClient.Helpers.Tests.Wiki_Examples
{
    public class SingleEndpointTests
    {
        [Theory]
        [InlineData(MyService.GetFooEndPoint)] // Specific url they are hitting.
        [InlineData("*")] // Don't care what url they are hitting.
        public async Task GivenSomeValidHttpRequest_GetSomeFooDataAsync_ReturnsAFoo(string endPoint)
        {
            // Arrange.
            const string responseData = "{ \"Id\":69, \"Name\":\"Jane\" }";
            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var options = new HttpMessageOptions
            {
                RequestUri = endPoint,
                HttpResponseMessage = messageResponse
            };

            var messageHandler = new FakeHttpMessageHandler(options);

            var myService = new MyService(messageHandler);

            // Act.
            // NOTE: network traffic will not leave your computer because you've faked the response, above.
            var result = await myService.GetSomeFooDataAsync();

            // Assert.
            result.Id.ShouldBe(69); // Returned from GetSomeFooDataAsync.
            result.Baa.ShouldBeNull();
            options.NumberOfTimesCalled.ShouldBe(1);
        }

        [Fact]
        public async Task GivenADifferentFakeUrlEndpoint_GetSomeFooDataAsync_ThrowsAnException()
        {
            // Arrange.
            const string responseData = "{ \"Id\":69, \"Name\":\"Jane\" }";
            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var options = new HttpMessageOptions
            {
                RequestUri = "http://this.is.not.the.correct.endpoint",
                HttpResponseMessage = messageResponse
            };

            var messageHandler = new FakeHttpMessageHandler(options);

            var myService = new MyService(messageHandler);

            // Act.
            // NOTE: network traffic will not leave your computer because you've faked the response, above.
            var result = await Should.ThrowAsync<InvalidOperationException>(myService.GetSomeFooDataAsync());

            // Assert.
            result.Message.ShouldStartWith("No HttpResponseMessage found");
        }

        [Fact]
        public async Task GivenAServerError_GetSomeFooDataAsync_ThrowsAnException()
        {
            // Arrange.
            const string responseData = "Something Blew Up";
            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData,
                                                                                      HttpStatusCode.InternalServerError);

            var options = new HttpMessageOptions
            {
                HttpResponseMessage = messageResponse
            };
            var messageHandler = new FakeHttpMessageHandler(options);

            var myService = new MyService(messageHandler);

            // Act.
            // NOTE: network traffic will not leave your computer because you've faked the response, above.
            var result = await Should.ThrowAsync<InvalidOperationException>(myService.GetSomeFooDataAsync());

            // Assert.
            result.Message.ShouldStartWith(responseData);
        }
    }
}