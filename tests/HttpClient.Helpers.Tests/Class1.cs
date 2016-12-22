using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorldDomination.Net.Http;
using Xunit;

namespace WorldDomination.HttpClient.Helpers.Tests
{
    public class Asdsds
    { 
        private const string MebourneWeatherApiEndpoint =
            "http://api.openweathermap.org/data/2.5/weather?q=Melbourne,AU&APPID=e73e7e23d05a82823d68fbe5069766aa";
        private const string DefaultKey = "pewpew";

    [Fact]
        public void Main()
        {
            Console.WriteLine("Starting sample console application.");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("About to retrieve weather for Melbourne, Australia...");
            var json = GetCurrentWeather().Result;
            Console.WriteLine("Retrieved some result form the weather service ...");
            Console.WriteLine("");
            Console.WriteLine(json);
            Console.WriteLine("");

            Console.WriteLine("About to retrieve some fake weather result ...");
            var fakeResult = GetFakeWeather().Result;
            Console.WriteLine("Retrieved some fake result ...");
            Console.WriteLine("");
            Console.WriteLine(fakeResult);
            Console.WriteLine("");

            Console.WriteLine("End of sample console application. Thank you for trying this out :)");
            Console.WriteLine("");
            
        }

        private static async Task<string> GetCurrentWeather(HttpMessageHandler fakeHttpMessageHandler = null)
        {
            var requestUri = new Uri(MebourneWeatherApiEndpoint);

            string content;
            using (var httpClient = fakeHttpMessageHandler == null
                ? new System.Net.Http.HttpClient()
                : new System.Net.Http.HttpClient(fakeHttpMessageHandler))
            {
                content = await httpClient.GetStringAsync(requestUri);
            }
            
            return content;

        }

        private static Task<string> GetFakeWeather()
        {
            // Set up the fake response for the Weather API Endpoint.
            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage("I am some fake weather result");
    var options = new HttpMessageOptions
    {
        HttpResponseMessage = messageResponse,
        RequestUri = MebourneWeatherApiEndpoint
    };
    var messageHandler = new FakeHttpMessageHandler(options);

            // Now attempt to get the result. Only this time, the fake data will be returned and no network
            // traffic will leave your computer.
            return GetCurrentWeather(messageHandler);
        }
    }
}