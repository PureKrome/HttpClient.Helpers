using System.Threading.Tasks;
using Shouldly;
using WorldDomination.Net.Http;
using Xunit;

namespace WorldDomination.HttpClient.Helpers.Tests.Wiki_Examples
{
    public class MultipleEndPointTests
    {
        [Fact]
        public async Task GivenSomeValidHttpRequests_GetSomeDataAsync_ReturnsAFoo()
        {
            // Arrange.

            // 1. First fake response.
            const string responseData1 = "{ \"Id\":69, \"Name\":\"Jane\" }";
            var messageResponse1 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData1);

            // 2. Second fake response.
            const string responseData2 = "{ \"FavGame\":\"Star Wars\", \"FavMovie\":\"Star Wars - all of em\" }";
            var messageResponse2 = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData2);

            // Prepare our 'options' with all of the above fake stuff.
            var options = new[]
            {
                new HttpMessageOptions
                {
                    RequestUri = MyService.GetFooEndPoint,
                    HttpResponseMessage = messageResponse1
                },
                new HttpMessageOptions
                {
                    RequestUri = MyService.GetBaaEndPoint,
                    HttpResponseMessage = messageResponse2
                }
            };

            // 3. Use the fake responses if those urls are attempted.
            var messageHandler = new FakeHttpMessageHandler(options);

            var myService = new MyService(messageHandler);

            // Act.
            // NOTE: network traffic will not leave your computer because you've faked the response, above.
            var result = await myService.GetAllDataAsync();

            // Assert.
            result.Id.ShouldBe(69); // Returned from GetSomeFooDataAsync.
            result.Baa.FavMovie.ShouldBe("Star Wars - all of em"); // Returned from GetSomeBaaDataAsync.
            options[0].NumberOfTimesCalled.ShouldBe(1);
            options[1].NumberOfTimesCalled.ShouldBe(1);
        }
    }
}
