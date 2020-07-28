# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) and this project adheres to [Semantic Versioning](http://semver.org/).

## [7.1.0] - 2019-07-08
### Fixed
- NumberOfTimesCount is now thread-safe.

## [7.1.0] - 2019-07-08
### Added
- Added GitHub sourcelink integration.

## [7.0.0] - 2019-05-28
### Changed
- Updated to .NET Standard 2.0.
- Better error message when failed to match the setup Options.

## [6.1.0] - 2017-06-07
### Fixed
- The new .NET Standard packaging (in AppVeyor) was creating an incorrect Assembly name. :blush:

## [6.0.0] - 2017-05-27
### Changed
- Supports .NET Standard 1.4! Woot!

### Fixed
- #27: Uri fails to compare/equals when Uri contains characters that get encoded.

## [5.1.0] - 2017-02-06
### Added
- Support for `Headers` in `HttpMessageOptions`.


## [1.0.0 -> 5.0.0] - 2014-08-07 -> 2017-02-06
### Added
- Inital and subsequent releases in supporting faking an `HttpClient` request/response.
