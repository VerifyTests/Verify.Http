# <img src="/src/icon.png" height="30px"> Verify.Http

[![Build status](https://ci.appveyor.com/api/projects/status/rfmvbst3od5vpl7p?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-http)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Http.svg)](https://www.nuget.org/packages/Verify.Http/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of Http bits.


## NuGet package

https://nuget.org/packages/Verify.Http/


## Enable

Enable VerifyHttp once at assembly load time:

<!-- snippet: Enable -->
<a id='snippet-enable'></a>
```cs
VerifyHttp.Enable();
```
<sup><a href='/src/Tests/ModuleInitializer.cs#L6-L8' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Converters

Includes converters for the following

  * `HttpMethod`
  * `Uri`
  * `HttpHeaders`
  * `HttpContent`
  * `HttpRequestMessage`
  * `HttpResponseMessage`

For example:

<!-- snippet: HttpResponse -->
<a id='snippet-httpresponse'></a>
```cs
[Fact]
public async Task HttpResponse()
{
    using var client = new HttpClient();

    var result = await client.GetAsync("https://httpbin.org/get");

    await Verify(result);
}
```
<sup><a href='/src/Tests/Tests.cs#L233-L243' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpresponse' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in:

<!-- snippet: Tests.HttpResponse.verified.txt -->
<a id='snippet-Tests.HttpResponse.verified.txt'></a>
```txt
{
  Version: 1.1,
  Status: 200 OK,
  Headers: {
    Access-Control-Allow-Credentials: true,
    Connection: keep-alive,
    Date: DateTime_1,
    Server: gunicorn/19.9.0
  },
  Content: {
    Headers: {
      Content-Type: application/json
    },
    Value: {
      args: {},
      headers: {
        Host: httpbin.org,
      },
      url: https://httpbin.org/get
    }
  },
  Request: https://httpbin.org/get
}
```
<sup><a href='/src/Tests/Tests.HttpResponse.verified.txt#L1-L23' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.HttpResponse.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## HttpClient recording via Service

For code that does web calls via HttpClient, these calls can be recorded and verified.


### Service that does http

Given a class that does some Http calls:

<!-- snippet: ServiceThatDoesHttp -->
<a id='snippet-servicethatdoeshttp'></a>
```cs
public class MyService
{
    HttpClient client;

    // Resolve a HttpClient. All http calls done at any
    // resolved client will be added to `recording.Sends`
    public MyService(HttpClient client) =>
        this.client = client;

    public Task MethodThatDoesHttp() =>
        // Some code that does some http calls
        client.GetAsync("https://httpbin.org/status/undefined");
}
```
<sup><a href='/src/Tests/Tests.cs#L33-L49' title='Snippet source file'>snippet source</a> | <a href='#snippet-servicethatdoeshttp' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Add to IHttpClientBuilder

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

await Verify(recording.Sends)
    // Ignore some headers that change per request
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L171-L190' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientrecording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Add globally

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

await Verify(recording.Sends)
    // Ignore some headers that change per request
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L145-L163' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientrecordingglobal' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Result

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
      Access-Control-Allow-Credentials: true,
      Connection: keep-alive,
      Server: gunicorn/19.9.0
    },
    ResponseContent: Invalid status code
  }
]
```
<sup><a href='/src/Tests/Tests.HttpClientRecording.verified.txt#L1-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.HttpClientRecording.verified.txt' title='Start of snippet'>anchor</a></sup>
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

await Verify(recording.Sends)
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L248-L272' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientpauseresume' title='Start of snippet'>anchor</a></sup>
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

await Verify(recording.Sends)
    .ModifySerialization(x => x.IgnoreMembers("Date"));
```
<sup><a href='/src/Tests/Tests.cs#L278-L303' title='Snippet source file'>snippet source</a> | <a href='#snippet-httpclientrecordingexplicit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Http Recording via listener

Http Recording allows, when a method is being tested, for any http requests made as part of that method call to be recorded and verified.

**Supported in net5 and up**


### Usage

Call `HttpRecording.StartRecording();` before the method being tested is called.

The perform the verification as usual:

<!-- snippet: HttpRecording -->
<a id='snippet-httprecording'></a>
```cs
[Fact]
public async Task TestHttpRecording()
{
    HttpRecording.StartRecording();

    var sizeOfResponse = await MethodThatDoesHttpCalls();

    await Verify(
            new
            {
                sizeOfResponse
            })
        .ModifySerialization(settings =>
        {
            //scrub some headers that are not consistent between test runs
            settings.IgnoreMembers("traceparent", "Date");
        });
}

static async Task<int> MethodThatDoesHttpCalls()
{
    using var client = new HttpClient();

    var jsonResult = await client.GetStringAsync("https://httpbin.org/json");
    var xmlResult = await client.GetStringAsync("https://httpbin.org/xml");
    return jsonResult.Length + xmlResult.Length;
}
```
<sup><a href='/src/Tests/Tests.cs#L81-L111' title='Snippet source file'>snippet source</a> | <a href='#snippet-httprecording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The requests/response pairs will be appended to the verified file.

<!-- snippet: Tests.TestHttpRecording.verified.txt -->
<a id='snippet-Tests.TestHttpRecording.verified.txt'></a>
```txt
{
  target: {
    sizeOfResponse: 951
  },
  httpCalls: [
    {
      Request: {
        Uri: https://httpbin.org/json,
        Headers: {}
      },
      Response: {
        Status: 200 OK,
        Headers: {
          Access-Control-Allow-Credentials: true,
          Connection: keep-alive,
          Server: gunicorn/19.9.0
        },
        ContentHeaders: {
          Content-Type: application/json
        },
        ContentStringParsed: {
          slideshow: {
            author: Yours Truly,
            date: date of publication,
            slides: [
              {
                title: Wake up to WonderWidgets!,
                type: all
              },
              {
                items: [
                  Why <em>WonderWidgets</em> are great,
                  Who <em>buys</em> WonderWidgets
                ],
                title: Overview,
                type: all
              }
            ],
            title: Sample Slide Show
          }
        }
      }
    },
    {
      Request: {
        Uri: https://httpbin.org/xml,
        Headers: {}
      },
      Response: {
        Status: 200 OK,
        Headers: {
          Access-Control-Allow-Credentials: true,
          Connection: keep-alive,
          Server: gunicorn/19.9.0
        },
        ContentHeaders: {
          Content-Type: application/xml
        },
        ContentStringParsed: {
          ?xml: {
            @version: 1.0,
            @encoding: us-ascii
          }/*  A SAMPLE set of slides  */,
          slideshow: {
            @title: Sample Slide Show,
            @date: Date of publication,
            @author: Yours Truly,
            #comment: [],
            slide: [
              {
                @type: all,
                title: Wake up to WonderWidgets!
              },
              {
                @type: all,
                title: Overview,
                item: [
                  {
                    #text: [
                      Why ,
                       are great
                    ],
                    em: WonderWidgets
                  },
                  null,
                  {
                    #text: [
                      Who ,
                       WonderWidgets
                    ],
                    em: buys
                  }
                ]
              }
            ]
          }
        }
      }
    }
  ]
}
```
<sup><a href='/src/Tests/Tests.TestHttpRecording.verified.txt#L1-L101' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.TestHttpRecording.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Explicit Usage

The above usage results in the http calls being automatically added snapshot file. Calls can also be explicitly read and recorded using `HttpRecording.FinishRecording()`. This enables:

 * Filtering what http calls are included in the snapshot.
 * Only verifying a subset of information for each http call.
 * Performing additional asserts on http calls.

For example:

<!-- snippet: HttpRecordingExplicit -->
<a id='snippet-httprecordingexplicit'></a>
```cs
[Fact]
public async Task TestHttpRecordingExplicit()
{
    HttpRecording.StartRecording();

    var sizeOfResponse = await MethodThatDoesHttpCalls();

    var httpCalls = HttpRecording.FinishRecording().ToList();

    // Ensure all calls finished in under 5 seconds
    var threshold = TimeSpan.FromSeconds(5);
    foreach (var call in httpCalls)
    {
        Assert.True(call.Duration < threshold);
    }

    await Verify(
        new
        {
            sizeOfResponse,
            // Only use the Uri in the snapshot
            httpCalls = httpCalls.Select(_ => _.Request.Uri)
        });
}
```
<sup><a href='/src/Tests/Tests.cs#L113-L140' title='Snippet source file'>snippet source</a> | <a href='#snippet-httprecordingexplicit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Results in the following:

<!-- snippet: Tests.TestHttpRecordingExplicit.verified.txt -->
<a id='snippet-Tests.TestHttpRecordingExplicit.verified.txt'></a>
```txt
{
  sizeOfResponse: 951,
  httpCalls: [
    https://httpbin.org/json,
    https://httpbin.org/xml
  ]
}
```
<sup><a href='/src/Tests/Tests.TestHttpRecordingExplicit.verified.txt#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.TestHttpRecordingExplicit.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Spider](https://thenounproject.com/term/spider/904683/) designed by [marialuisa iborra](https://thenounproject.com/marialuisa.iborra/) from [The Noun Project](https://thenounproject.com).
