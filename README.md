#System.Net.Http.HttpClient.Helpers

![](https://ci.appveyor.com/api/projects/status/siwilxb8t3enyus2)

Code that uses `System.Net.Http.HttpClient` will attempt to actually call/hit that Http endpoint.

To prevent this from happening in a *unit* test, some simple helpers are provided in this code library.

-----

## Installation

[![](http://i.imgur.com/oLtAwq9.png)](https://www.nuget.org/packages/WorldDomination.HttpClient.Helpers/)

## Examples

### HttpClient only calls one endpoint / request Url
<code>
    const string requestUrl = "http://www.something.com/some/website";<br/>
    const string responseData = "I am not some Html.";

    // 1. Fake response.
    var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

    // 2. Fake handler that says: for this Url, return this (fake) response.
    var messageHandler = new FakeHttpMessageHandler(requestUrl, messageResponse);

    // 3. Go forth and win!
    var httpClient = new System.Net.Http.HttpClient(messageHandler);
    var result = await httpClient.GetStringAsync(requestUrl);
</code>

### HttpClient calls multiple endpoints / request Url's
<code>
    // 1. Fake response #1.<br/>
    const string requestUrl1 = "http://www.something.com/some/website";<br/>
    const string responseData1 = "Fake response #1";<br/>
    var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData1);<br/>

    // 2. Fake response #2.
    const string requestUrl2 = "http://www.pewpew.com/some/website";
    const string responseData2 = "Fake response #2";
    var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData2);

    // 3. Fake handler that says: for any one of these 2 Url's, return their appropriate (fake) response.<br/>
    var messageResponses = new Dictionary<string, HttpResponseMessage>
    {
        {requestUrl1, messageResponse1},
        {requestUrl2, messageResponse2}
    };
    var messageHandler = new FakeHttpMessageHandler(messageResponses);

    // 4. Go forth and win!
    var httpClient = new System.Net.Http.HttpClient(messageHandler);
    var result1 = await httpClient.GetStringAsync(requestUrl1);
    var result2 = await httpClient.GetStringAsync(requestUrl2);
</code>

For more a few more samples, please check out the Wiki page: Helper Examples

-----
