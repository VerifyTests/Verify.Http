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
    * [HttpClient recording](#httpclient-recording)
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
<sup><a href='/src/Tests/ModuleInitializer.cs#L9-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
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
            new("Value1"),
            new("Value2")
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
      Value: Value1
    },
    {
      Value: Value2
    }
  ],
  context: {
    Headers: {
      headerKey: headerValue,
      receivedInput: inputValue
    },
    Cookies: {
      cookieKey: cookieValue
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
      headerKey: headerValue
    }
  },
  nextCalled: true
}
```
<sup><a href='/src/Tests/Snippets/MyMiddlewareTests.Test.verified.txt#L1-L8' title='Snippet source file'>snippet source</a> | <a href='#snippet-MyMiddlewareTests.Test.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### HttpClient recording

For code that does web calls via HttpClient, these calls can be recorded and verified.


#### Service that does http

Given a class that does some Http calls:

<!-- snippet: ServiceThatDoesHttp -->
<a id='snippet-servicethatdoeshttp'></a>
```cs
public class MyService
{
    HttpClient client;

    // Resolve a HttpClient. All http calls done at any
    // resolved client will be added to `recording.Sends`
    public MyService(HttpClient client)
    {
        this.client = client;
    }

    public Task MethodThatDoesHttp()
    {
        // Some code that does some http calls
        return client.GetAsync("https://httpbin.org/status/undefined");
    }
}
```
<sup><a href='/src/Tests/Tests.cs#L79-L99' title='Snippet source file'>snippet source</a> | <a href='#snippet-servicethatdoeshttp' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Add to IHttpClientBuilder

Http recording can be added to a `IHttpClientBuilder`:

<!-- snippet: HttpClientRecording -->
<a id='snippet-httpclientrecording'></a>
```cs
var collection = new ServiceCollection();
collection.AddScoped<MyService>();
var httpBuilder = collection.AddHttpClient<MyService>();

// Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
var recording = httpBuilder.AddRecording();

await using var provider = collection.BuildServiceProvider();

var myService = provider.GetRequiredService<MyService>();

await myService.MethodThatDoesHttp();

await Verifier.Verify(recording.Sends)
    // Ignore some headers that change per request
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L129-L148' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientrecording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Add globally

Http can also be added globally `IHttpClientBuilder`:

Note: This only seems to work in net5 and up.

<!-- snippet: HttpClientRecordingGlobal -->
<a id='snippet-httpclientrecordingglobal'></a>
```cs
var collection = new ServiceCollection();
collection.AddScoped<MyService>();

// Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
var (builder, recording) = collection.AddRecordingHttpClient();

await using var provider = collection.BuildServiceProvider();

var myService = provider.GetRequiredService<MyService>();

await myService.MethodThatDoesHttp();

await Verifier.Verify(recording.Sends)
    // Ignore some headers that change per request
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L105-L123' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientrecordingglobal' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Result

Will result in the following verified file:

<!-- snippet: Tests.HttpClientRecording.verified.txt -->
<a id='snippet-Tests.HttpClientRecording.verified.txt'></a>
```txt
[
  {
    RequestUri: https://httpbin.org/status/undefined,
    RequestMethod: GET,
    ResponseStatus: BadRequest,
    ResponseHeaders: {
      Connection: keep-alive,
      Server: gunicorn/19.9.0,
      Access-Control-Allow-Origin: *,
      Access-Control-Allow-Credentials: true
    },
    ResponseContent: Invalid status code
  }
]
```
<sup><a href='/src/Tests/Tests.HttpClientRecording.verified.txt#L1-L14' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.HttpClientRecording.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There a Pause/Resume semantics:

<!-- snippet: HttpClientPauseResume -->
<a id='snippet-httpclientpauseresume'></a>
```cs
var collection = new ServiceCollection();
collection.AddScoped<MyService>();
var httpBuilder = collection.AddHttpClient<MyService>();

// Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
var recording = httpBuilder.AddRecording();

await using var provider = collection.BuildServiceProvider();

var myService = provider.GetRequiredService<MyService>();

// Recording is enabled by default. So Pause to stop recording
recording.Pause();
await myService.MethodThatDoesHttp();

// Resume recording
recording.Resume();
await myService.MethodThatDoesHttp();

await Verifier.Verify(recording.Sends)
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L154-L176' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientpauseresume' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

If the `AddRecordingHttpClient` helper method does not meet requirements, the `RecordingHandler` can be explicitly added:

<!-- snippet: HttpClientRecordingExplicit -->
<a id='snippet-httpclientrecordingexplicit'></a>
```cs
var collection = new ServiceCollection();

var builder = collection.AddHttpClient("name");

// Change to not recording at startup
var recording = new RecordingHandler(recording: false);

builder.AddHttpMessageHandler(() => recording);

await using var provider = collection.BuildServiceProvider();

var factory = provider.GetRequiredService<IHttpClientFactory>();

var client = factory.CreateClient("name");

await client.GetAsync("https://www.google.com/");

recording.Resume();
await client.GetAsync("https://httpbin.org/status/undefined");

await Verifier.Verify(recording.Sends)
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L182-L205' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientrecordingexplicit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Spider](https://thenounproject.com/term/spider/904683/) designed by [marialuisa iborra](https://thenounproject.com/marialuisa.iborra/) from [The Noun Project](https://thenounproject.com/creativepriyanka).
