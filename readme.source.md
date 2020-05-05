# <img src="/src/icon.png" height="30px"> Verify.Web

[![Build status](https://ci.appveyor.com/api/projects/status/eedjhmx5o3082tyq?svg=true)](https://ci.appveyor.com/project/VerifyTests/verify-web)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Web.svg)](https://www.nuget.org/packages/Verify.Web/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of web bits.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify.web?utm_source=nuget-verify.web&utm_medium=referral&utm_campaign=enterprise).

toc


## NuGet package

https://nuget.org/packages/Verify.Web/


## Usage

Enable VerifyWeb once at assembly load time:

snippet: Enable


### Controller

Given the following controller:

snippet: MyController.cs

This test:

snippet: MyControllerTest

Will result in the following verified file:

snippet: MyControllerTests.Test.verified.txt



### Middleware

Given the following middleware:

snippet: MyMiddleware.cs

This test:

snippet: MyMiddlewareTest

Will result in the following verified file:

snippet: MyMiddlewareTests.Test.verified.txt


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Spider](https://thenounproject.com/term/spider/904683/) designed by [marialuisa iborra](https://thenounproject.com/marialuisa.iborra/) from [The Noun Project](https://thenounproject.com/creativepriyanka).