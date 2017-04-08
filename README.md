# HttpClient.Helpers

| Stage       | CI | NuGet |
|-------------|----|-------|
| Production  | [![Build status](https://ci.appveyor.com/api/projects/status/siwilxb8t3enyus2/branch/master?svg=true)](https://ci.appveyor.com/project/PureKrome/httpclient-helpers) |  [![NuGet Badge](https://buildstats.info/nuget/WorldDomination.HttpClient.Helpers)](https://www.nuget.org/packages/WorldDomination.HttpClient.Helpers/) |
| Development | [![Build status](https://ci.appveyor.com/api/projects/status/siwilxb8t3enyus2/branch/dev?svg=true)](https://ci.appveyor.com/project/PureKrome/httpclient-helpers) | [![MyGet Badge](https://buildstats.info/myget/pk-development/WorldDomination.HttpClient.Helpers)](https://www.myget.org/feed/pk-development/package/nuget/WorldDomination.HttpClient.Helpers) |

---

Code that uses `System.Net.Http.HttpClient` will attempt to actually call/hit that Http endpoint.

To prevent this from happening in a *unit* test, some simple helpers are provided in this code library.

## Key Points
- Use Dependency Injection to hijack your request (this library makes it _supa dupa easy_ to do this)
- Works with GET/POST/etc.
- Can provide wildcards (i.e. I don't care about the Request endpoint or the request HTTP Method, etc)
- Can provide multiple endpoints and see handle what is returned based on the particular request.
- Can be used to test network errors during transmission. i.e. can test when the HttpClient throws an exception because of .. well ... :boom:
-----

## Installation

[![](http://i.imgur.com/oLtAwq9.png)](https://www.nuget.org/packages/WorldDomination.HttpClient.Helpers/)

Package Name: `WorldDomination.HttpClient.Helpers`  
CLI: `install-package WorldDomination.HttpClient.Helpers`  


## Sample Code

There's [plenty more examples](https://github.com/PureKrome/HttpClient.Helpers/wiki) about to wire up:
- [A really simple example](https://github.com/PureKrome/HttpClient.Helpers/wiki/A-single-endpoint)
- [Multiple endpoints](https://github.com/PureKrome/HttpClient.Helpers/wiki/Multiple-endpoints) at once
- [Wildcard endpoints](https://github.com/PureKrome/HttpClient.Helpers/wiki/Wildcard-endpoints)
- [Throwing exceptions](https://github.com/PureKrome/HttpClient.Helpers/wiki/Faking-an-Exception) and handling it

For all the samples, please [check out the Wiki page: Helper Examples](https://github.com/PureKrome/HttpClient.Helpers/wiki)

-----

Special Ackowledgements

A special and sincere *thank you* to David Fowler ([@davidfowl](http://www.twitter.com/davidfowl)) who explained how I should be unit testing the `HttpClient` class and gave me the guidence to make this library. I was soooo on the wrong path - and he guided me back on track.

Thank you David! :ok_hand: :cocktail: :space_invader:

-----

### Summary

Finally, unit testing `HttpClient` is now awesome and simple!

![Wohoo](https://31.media.tumblr.com/43e63461d1e3f22a49b18dbf15227a1d/tumblr_inline_n3t10oQfIh1solpjm.gif)

---
[![I'm happy to accept tips](http://img.shields.io/gittip/purekrome.svg?style=flat-square)](https://gratipay.com/PureKrome/)  
![Lic: MIT](http://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)
