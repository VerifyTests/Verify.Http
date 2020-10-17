# <img src="/src/icon.png" height="30px"> Verify.Web

[![Build status](https://ci.appveyor.com/api/projects/status/eedjhmx5o3082tyq?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-web)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Web.svg)](https://www.nuget.org/packages/Verify.Web/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of web bits.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify?utm_source=nuget-verify&utm_medium=referral&utm_campaign=enterprise).

<a href='https://dotnetfoundation.org' alt='Part of the .NET Foundation'><img src='https://raw.githubusercontent.com/VerifyTests/Verify/master/docs/dotNetFoundation.svg' height='30px'></a><br>
Part of the <a href='https://dotnetfoundation.org' alt=''>.NET Foundation</a>

<!-- toc -->
## Contents

  * [Usage](#usage)
    * [Controller](#controller)
    * [Middleware](#middleware)
  * [Security contact information](#security-contact-information)<!-- endToc -->


## NuGet package

https://nuget.org/packages/Verify.Web/


## Usage

Enable VerifyWeb once at assembly load time:

<!-- snippet: Enable -->
<a id='snippet-enable'></a>
```cs
VerifyWeb.Enable();
```
<sup><a href='/src/Tests/ModuleInitializer.cs#L7-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Controller

Given the following controller:

<!-- snippet: MyController.cs -->
<a id='snippet-MyController.cs'></a>
```cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

public class MyController :
    Controller
{
    public ActionResult<List<DataItem>> Method(string input)
    {
        var headers = HttpContext.Response.Headers;
        headers.Add("headerKey", "headerValue");
        headers.Add("receivedInput", input);

        var cookies = HttpContext.Response.Cookies;
        cookies.Append("cookieKey", "cookieValue");

        var items = new List<DataItem>
        {
            new DataItem("Value1"),
            new DataItem("Value2")
        };
        return new ActionResult<List<DataItem>>(items);
    }

    public class DataItem
    {
        public string Value { get; }

        public DataItem(string value)
        {
            Value = value;
        }
    }
}
```
<sup><a href='/src/Tests/Snippets/MyController.cs#L1-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-MyController.cs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This test:

<!-- snippet: MyControllerTest -->
<a id='snippet-mycontrollertest'></a>
```cs
[Test]
public Task Test()
{
    var context = new ControllerContext
    {
        HttpContext = new DefaultHttpContext()
    };
    var controller = new MyController
    {
        ControllerContext = context
    };

    var result = controller.Method("inputValue");
    return Verifier.Verify(
        new
        {
            result,
            context
        });
}
```
<sup><a href='/src/Tests/Snippets/MyControllerTests.cs#L10-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-mycontrollertest' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

<!-- snippet: MyControllerTests.Test.verified.txt -->
<a id='snippet-MyControllerTests.Test.verified.txt'></a>
```txt
{
  result: [
    {
      Value: 'Value1'
    },
    {
      Value: 'Value2'
    }
  ],
  context: {
    Headers: {
      headerKey: 'headerValue',
      receivedInput: 'inputValue'
    },
    Cookies: {
      cookieKey: 'cookieValue'
    }
  }
}
```
<sup><a href='/src/Tests/Snippets/MyControllerTests.Test.verified.txt#L1-L19' title='Snippet source file'>snippet source</a> | <a href='#snippet-MyControllerTests.Test.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->



### Middleware

Given the following middleware:

<!-- snippet: MyMiddleware.cs -->
<a id='snippet-MyMiddleware.cs'></a>
```cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class MyMiddleware
{
    RequestDelegate next;

    public MyMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("headerKey", "headerValue");
        await next(context);
    }
}
```
<sup><a href='/src/Tests/Snippets/MyMiddleware.cs#L1-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-MyMiddleware.cs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This test:

<!-- snippet: MyMiddlewareTest -->
<a id='snippet-mymiddlewaretest'></a>
```cs
[Test]
public async Task Test()
{
    var nextCalled = false;
    var middleware = new MyMiddleware(
        _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

    var context = new DefaultHttpContext();
    await middleware.Invoke(context);

    await Verifier.Verify(
        new
        {
            context.Response,
            nextCalled
        });
}
```
<sup><a href='/src/Tests/Snippets/MyMiddlewareTests.cs#L9-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-mymiddlewaretest' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

<!-- snippet: MyMiddlewareTests.Test.verified.txt -->
<a id='snippet-MyMiddlewareTests.Test.verified.txt'></a>
```txt
{
  Response: {
    Headers: {
      headerKey: 'headerValue'
    }
  },
  nextCalled: true
}
```
<sup><a href='/src/Tests/Snippets/MyMiddlewareTests.Test.verified.txt#L1-L8' title='Snippet source file'>snippet source</a> | <a href='#snippet-MyMiddlewareTests.Test.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Spider](https://thenounproject.com/term/spider/904683/) designed by [marialuisa iborra](https://thenounproject.com/marialuisa.iborra/) from [The Noun Project](https://thenounproject.com/creativepriyanka).
