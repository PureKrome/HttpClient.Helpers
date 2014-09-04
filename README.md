#Http.HttpClient.Helpers

![](https://ci.appveyor.com/api/projects/status/siwilxb8t3enyus2)
![](http://img.shields.io/nuget/v/WorldDomination.HttpClient.Helpers.svg?style=flat-square)
![](http://img.shields.io/nuget/dt/WorldDomination.HttpClient.Helpers.svg?style-flat-square)
---

Code that uses `System.Net.Http.HttpClient` will attempt to actually call/hit that Http endpoint.

To prevent this from happening in a *unit* test, some simple helpers are provided in this code library.

-----

## Installation

![](https://ci.appveyor.com/api/projects/status/siwilxb8t3enyus2)
![](http://img.shields.io/nuget/v/WorldDomination.HttpClient.Helpers.svg?style=flat-square)
![](http://img.shields.io/nuget/dt/WorldDomination.HttpClient.Helpers.svg?style=flat-square)


### HttpClient only calls one endpoint / request Url

```C#
[Fact]
public async Task GivenAValidHttpRequest_GetSomeDataAsync_ReturnsAFoo()
{
    // Arrange.
    const string requestUrl = "http://www.something.com/some/website";  
    const string responseData = "I am not some Html.";
    var myService = new MyService();

    // 1. Fake response.  
    var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);  

    // 2. Fake handler that says: for this Url, return this (fake) response.  
    HttpClientFactory.MessageHandler = new FakeHttpMessageHandler(requestUrl, messageResponse);
    
    // Act.
    // NOTE: network traffic will not leave your computer because you've faked the response, above.
    var result = myService.GetSomeDataAsync();
    
    // Assert.
    result.Id.ShouldBe(69);
}
```

There's [plenty more examples](https://github.com/PureKrome/HttpClient.Helpers/wiki) about to wire up:

 - [Multiple endpoints](https://github.com/PureKrome/HttpClient.Helpers/wiki/Multiple-endpoints) at once
 - [Wildcard endpoints](https://github.com/PureKrome/HttpClient.Helpers/wiki/Wildcard-endpoints)
 - [Throwing exceptions](https://github.com/PureKrome/HttpClient.Helpers/wiki/Faking-an-Exception) and handling it

For more a few more samples, please check out the Wiki page: Helper Examples

-----

###Summary

Finally, unit testing `HttpClient` is now awesome and simple!

![Wohoo](https://31.media.tumblr.com/43e63461d1e3f22a49b18dbf15227a1d/tumblr_inline_n3t10oQfIh1solpjm.gif)

---
![I'm happy to accept tips](http://img.shields.io/gittip/purekrome.svg?style=flat-square)  
![Lic: MIT](http://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)