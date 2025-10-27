# <img src="/src/icon.png" height="30px"> Verify.Http

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/rfmvbst3od5vpl7p?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-http)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.Http.svg)](https://www.nuget.org/packages/Verify.Http/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of Http bits.<!-- singleLineInclude: intro. path: /docs/intro.include.md -->

**See [Milestones](../../milestones?state=closed) for release notes.**


## Sponsors


### Entity Framework Extensions<!-- include: zzz. path: /docs/zzz.include.md -->

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=simoncropp&utm_medium=Verify.Http) is a major sponsor and is proud to contribute to the development this project.

[![Entity Framework Extensions](https://raw.githubusercontent.com/VerifyTests/Verify.Http/refs/heads/main/docs/zzz.png)](https://entityframework-extensions.net/?utm_source=simoncropp&utm_medium=Verify.Http)<!-- endInclude -->


## NuGet

 * https://nuget.org/packages/Verify.Http


## Initialize

Call `VerifierSettings.InitializePlugins()` in a `[ModuleInitializer]`. 

<!-- snippet: ModuleInitializer.cs -->
<a id='snippet-ModuleInitializer.cs'></a>
```cs
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.InitializePlugins();
}
```
<sup><a href='/src/Tests/ModuleInitializer.cs#L1-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-ModuleInitializer.cs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Or, if order of plugins is important, use `VerifyHttp.Initialize()` in a `[ModuleInitializer]`.


## ScrubHttpTextResponse

<!-- snippet: ScrubHttpTextResponse -->
<a id='snippet-ScrubHttpTextResponse'></a>
```cs
[Test]
public async Task ScrubHttpTextResponse()
{
    using var client = new HttpClient();

    using var result = await client.GetAsync("https://httpcan.org/html");

    await Verify(result)
        .ScrubHttpTextResponse(_ => _.Replace("Herman Melville - Moby-Dick", "New title"));
}
```
<sup><a href='/src/Tests/Tests.cs#L24-L37' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScrubHttpTextResponse' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Enable Recording

Enable at any point in a test using `VerifyTests.Recording.Start()`.


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
<a id='snippet-HttpResponse'></a>
```cs
[Test]
public async Task HttpResponse()
{
    using var client = new HttpClient();

    var result = await client.GetAsync("https://httpcan.org/json");

    await Verify(result);
}
```
<sup><a href='/src/Tests/Tests.cs#L260-L272' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpResponse' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: Tests.HttpResponse.verified.txt -->
<a id='snippet-Tests.HttpResponse.verified.txt'></a>
```txt
{
  Status: 200 OK,
  Headers: {
    Access-Control-Allow-Credentials: true,
    Alt-Svc: h3=":443",
    cf-cache-status: DYNAMIC,
    Connection: keep-alive,
    Date: DateTime_1,
    Nel: {"report_to":"cf-nel","success_fraction":0.0,"max_age":604800},
    Server: cloudflare,
    Vary: Origin,Access-Control-Request-Method,Access-Control-Request-Headers
  },
  Content: {
    Headers: {
      Content-Length: 274,
      Content-Type: application/json
    },
    Value: {
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
}
```
<sup><a href='/src/Tests/Tests.HttpResponse.verified.txt#L1-L40' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.HttpResponse.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Ignoring Headers

Headers are treated as properties, and hence can be ignored using `IgnoreMember`:

<!-- snippet: IgnoreHeader -->
<a id='snippet-IgnoreHeader'></a>
```cs
[Test]
public async Task IgnoreHeader()
{
    using var client = new HttpClient();

    using var result = await client.GetAsync("https://httpcan.org/get");

    await Verify(result)
        .IgnoreMembers(
            "Server",
            "Access-Control-Allow-Credentials");
}
```
<sup><a href='/src/Tests/Tests.cs#L7-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-IgnoreHeader' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## HttpClient recording via Service

For code that does web calls via HttpClient, these calls can be recorded and verified.


### Service that does http

Given a class that does some Http calls:

<!-- snippet: ServiceThatDoesHttp -->
<a id='snippet-ServiceThatDoesHttp'></a>
```cs
// Resolve a HttpClient. All http calls done at any
// resolved client will be added to `recording.Sends`
public class MyService(HttpClient client)
{
    public Task MethodThatDoesHttp() =>
        // Some code that does some http calls
        client.GetAsync("https://httpcan.org/get");
}
```
<sup><a href='/src/Tests/Tests.cs#L78-L89' title='Snippet source file'>snippet source</a> | <a href='#snippet-ServiceThatDoesHttp' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Add to IHttpClientBuilder

Http recording can be added to a `IHttpClientBuilder`:

<!-- snippet: HttpClientRecording -->
<a id='snippet-HttpClientRecording'></a>
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
    .IgnoreMember("Date");
```
<sup><a href='/src/Tests/Tests.cs#L212-L230' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientRecording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Add globally

Http can also be added globally `IHttpClientBuilder`:

<!-- snippet: HttpClientRecordingGlobal -->
<a id='snippet-HttpClientRecordingGlobal'></a>
```cs
var collection = new ServiceCollection();
collection.AddScoped<MyService>();

// Adds a AddHttpClient and adds a RecordingHandler using AddHttpMessageHandler
var (builder, recording) = collection.AddRecordingHttpClient();

await using var provider = collection.BuildServiceProvider();

var myService = provider.GetRequiredService<MyService>();

await myService.MethodThatDoesHttp();

await Verify(recording.Sends)
    .IgnoreMember("Date");
```
<sup><a href='/src/Tests/Tests.cs#L189-L206' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientRecordingGlobal' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: Tests.HttpClientRecording.verified.txt -->
<a id='snippet-Tests.HttpClientRecording.verified.txt'></a>
```txt
[
  {
    RequestUri: https://httpcan.org/get,
    RequestMethod: GET,
    ResponseStatus: OK 200,
    ResponseHeaders: {
      Access-Control-Allow-Credentials: true,
      Alt-Svc: h3=":443",
      cf-cache-status: DYNAMIC,
      Connection: keep-alive,
      Nel: {"report_to":"cf-nel","success_fraction":0.0,"max_age":604800},
      Server: cloudflare,
      Vary: Origin|Access-Control-Request-Method|Access-Control-Request-Headers
    },
    ResponseContent: {"args":{},"headers":{"accept-encoding":"gzip, br","connection":"close","host":"httpcan.org"},"origin":"194.193.45.211","url":"http://httpcan.org/get"}
  }
]
```
<sup><a href='/src/Tests/Tests.HttpClientRecording.verified.txt#L1-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.HttpClientRecording.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There a Pause/Resume semantics:

<!-- snippet: HttpClientPauseResume -->
<a id='snippet-HttpClientPauseResume'></a>
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
    .ScrubInlineDateTimes("R");
```
<sup><a href='/src/Tests/Tests.cs#L282-L306' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientPauseResume' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

If the `AddRecordingHttpClient` helper method does not meet requirements, the `RecordingHandler` can be explicitly added:

<!-- snippet: HttpClientRecordingExplicit -->
<a id='snippet-HttpClientRecordingExplicit'></a>
```cs
var collection = new ServiceCollection();

var builder = collection.AddHttpClient("name");

// Change to not recording at startup
var recording = new RecordingHandler(recording: false);

builder.AddHttpMessageHandler(() => recording);

await using var provider = collection.BuildServiceProvider();

var factory = provider.GetRequiredService<IHttpClientFactory>();

var client = factory.CreateClient("name");

await client.GetAsync("https://httpcan.org/html");

recording.Resume();
await client.GetAsync("https://httpcan.org/json");

await Verify(recording.Sends)
    .ScrubInlineDateTimes("R");
```
<sup><a href='/src/Tests/Tests.cs#L312-L337' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientRecordingExplicit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Http Recording via listener

Http Recording allows, when a method is being tested, for any http requests made as part of that method call to be recorded and verified.


### Usage

Call `HttpRecording.StartRecording();` before the method being tested is called.

The perform the verification as usual:

<!-- snippet: HttpRecording -->
<a id='snippet-HttpRecording'></a>
```cs
[Test]
public async Task TestHttpRecording()
{
    Recording.Start();

    var sizeOfResponse = await MethodThatDoesHttpCalls();

    await Verify(
            new
            {
                sizeOfResponse
            })
        .IgnoreMembers("Expires", "Date")
        .ScrubLinesContaining("\"version\"");
}

static async Task<int> MethodThatDoesHttpCalls()
{
    using var client = new HttpClient();

    var jsonResult = await client.GetStringAsync("https://httpcan.org/json");
    var ymlResult = await client.GetStringAsync("https://httpcan.org/xml");
    return jsonResult.Length + ymlResult.Length;
}
```
<sup><a href='/src/Tests/Tests.cs#L123-L150' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpRecording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

The requests/response pairs will be appended to the verified file.

<!-- snippet: Tests.TestHttpRecording.verified.txt -->
<a id='snippet-Tests.TestHttpRecording.verified.txt'></a>
```txt
{
  target: {
    sizeOfResponse: 792
  },
  httpCall: [
    {
      Status: Created,
      Request: {
        Uri: https://httpcan.org/json,
        Headers: {}
      },
      Response: {
        Status: 200 OK,
        Headers: {
          Access-Control-Allow-Credentials: true,
          Alt-Svc: h3=":443",
          cf-cache-status: DYNAMIC,
          Connection: keep-alive,
          Nel: {"report_to":"cf-nel","success_fraction":0.0,"max_age":604800},
          Server: cloudflare,
          Vary: Origin,Access-Control-Request-Method,Access-Control-Request-Headers
        },
        ContentHeaders: {
          Content-Length: 274,
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
      Status: Created,
      Request: {
        Uri: https://httpcan.org/xml,
        Headers: {}
      },
      Response: {
        Status: 200 OK,
        Headers: {
          Access-Control-Allow-Credentials: true,
          Alt-Svc: h3=":443",
          cf-cache-status: DYNAMIC,
          Connection: keep-alive,
          Nel: {"report_to":"cf-nel","success_fraction":0.0,"max_age":604800},
          Server: cloudflare,
          Vary: Origin,Access-Control-Request-Method,Access-Control-Request-Headers
        },
        ContentHeaders: {
          Content-Length: 518,
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
<sup><a href='/src/Tests/Tests.TestHttpRecording.verified.txt#L1-L113' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.TestHttpRecording.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Explicit Usage

The above usage results in the http calls being automatically added snapshot file. Calls can also be explicitly read and recorded using `HttpRecording.FinishRecording()`. This enables:

 * Filtering what http calls are included in the snapshot.
 * Only verifying a subset of information for each http call.
 * Performing additional asserts on http calls.

For example:

<!-- snippet: HttpRecordingExplicit -->
<a id='snippet-HttpRecordingExplicit'></a>
```cs
[Test]
public async Task TestHttpRecordingExplicit()
{
    Recording.Start();

    var responseSize = await MethodThatDoesHttpCalls();

    var httpCalls = Recording.Stop()
        .Select(_ => _.Data)
        .OfType<HttpCall>()
        .ToList();

    // Ensure all calls finished in under 5 seconds
    var threshold = TimeSpan.FromSeconds(5);
    foreach (var call in httpCalls)
    {
        IsTrue(call.Duration < threshold);
    }

    await Verify(
        new
        {
            responseSize,
            // Only use the Uri in the snapshot
            httpCalls = httpCalls.Select(_ => _.Request.Uri)
        });
}
```
<sup><a href='/src/Tests/Tests.cs#L152-L182' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpRecordingExplicit' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: Tests.TestHttpRecordingExplicit.verified.txt -->
<a id='snippet-Tests.TestHttpRecordingExplicit.verified.txt'></a>
```txt
{
  responseSize: 792,
  httpCalls: [
    https://httpcan.org/json,
    https://httpcan.org/xml
  ]
}
```
<sup><a href='/src/Tests/Tests.TestHttpRecordingExplicit.verified.txt#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.TestHttpRecordingExplicit.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Mocking

`MockHttpClient` allows mocking of http responses and recording of http requests.


### Default Response

The default behavior is to return a `HttpResponseMessage` with a status code of `200 OK`.

<!-- snippet: DefaultContent -->
<a id='snippet-DefaultContent'></a>
```cs
[Test]
public async Task DefaultContent()
{
    using var client = new MockHttpClient();

    var result = await client.GetAsync("https://fake/get");

    await Verify(result);
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L38-L50' title='Snippet source file'>snippet source</a> | <a href='#snippet-DefaultContent' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.DefaultContent.verified.txt -->
<a id='snippet-MockHttpClientTests.DefaultContent.verified.txt'></a>
```txt
{
  Status: 200 OK
}
```
<sup><a href='/src/Tests/MockHttpClientTests.DefaultContent.verified.txt#L1-L3' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.DefaultContent.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Verifying Calls

Request-Response pairs can be verified using `MockHttpClient.Calls`

<!-- snippet: ExplicitContent -->
<a id='snippet-ExplicitContent'></a>
```cs
[Test]
public async Task ExplicitContent()
{
    using var client = new MockHttpClient(
        content: """{ "a": "b" }""",
        mediaType: "application/json");

    var result = await client.GetAsync("https://fake/get");

    await Verify(result);
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L22-L36' title='Snippet source file'>snippet source</a> | <a href='#snippet-ExplicitContent' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.TrackedCalls.verified.txt -->
<a id='snippet-MockHttpClientTests.TrackedCalls.verified.txt'></a>
```txt
[
  {
    Request: https://fake/get1,
    Response: 200 Ok
  },
  {
    Request: https://fake/get2,
    Response: 200 Ok
  }
]
```
<sup><a href='/src/Tests/MockHttpClientTests.TrackedCalls.verified.txt#L1-L10' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.TrackedCalls.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Explicit Content Response

Always return an explicit `StringContent` and media-type:

<!-- snippet: ExplicitContent -->
<a id='snippet-ExplicitContent'></a>
```cs
[Test]
public async Task ExplicitContent()
{
    using var client = new MockHttpClient(
        content: """{ "a": "b" }""",
        mediaType: "application/json");

    var result = await client.GetAsync("https://fake/get");

    await Verify(result);
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L22-L36' title='Snippet source file'>snippet source</a> | <a href='#snippet-ExplicitContent' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.ExplicitContent.verified.txt -->
<a id='snippet-MockHttpClientTests.ExplicitContent.verified.txt'></a>
```txt
{
  Status: 200 OK,
  Content: {
    Headers: {
      Content-Length: 12,
      Content-Type: application/json; charset=utf-8
    },
    Value: {
      a: b
    }
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ExplicitContent.verified.txt#L1-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ExplicitContent.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Explicit HttpStatusCode Response

Always return an explicit `HttpStatusCode`:

<!-- snippet: ExplicitStatusCode -->
<a id='snippet-ExplicitStatusCode'></a>
```cs
[Test]
public async Task ExplicitStatusCode()
{
    using var client = new MockHttpClient(HttpStatusCode.Ambiguous);

    var result = await client.GetAsync("https://fake/get");

    await Verify(result);
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L67-L79' title='Snippet source file'>snippet source</a> | <a href='#snippet-ExplicitStatusCode' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.ExplicitStatusCode.verified.txt -->
<a id='snippet-MockHttpClientTests.ExplicitStatusCode.verified.txt'></a>
```txt
{
  Status: 300 Multiple Choices
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ExplicitStatusCode.verified.txt#L1-L3' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ExplicitStatusCode.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Explicit HttpResponseMessage

Alwars return an explicit `HttpResponseMessage`:

<!-- snippet: ExplicitResponse -->
<a id='snippet-ExplicitResponse'></a>
```cs
[Test]
public async Task ExplicitResponse()
{
    var response = new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent("Hello")
    };
    using var client = new MockHttpClient(response);

    var result = await client.GetAsync("https://fake/get");

    await Verify(result);
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L106-L122' title='Snippet source file'>snippet source</a> | <a href='#snippet-ExplicitResponse' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.ExplicitResponse.verified.txt -->
<a id='snippet-MockHttpClientTests.ExplicitResponse.verified.txt'></a>
```txt
{
  Status: 200 OK,
  Content: {
    Headers: {
      Content-Length: 5,
      Content-Type: text/plain; charset=utf-8
    },
    Value: Hello
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ExplicitResponse.verified.txt#L1-L10' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ExplicitResponse.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### HttpResponseMessage builder

Use custom code to create a `HttpResponseMessage` base on a `HttpRequestMessage`:

<!-- snippet: ResponseBuilder -->
<a id='snippet-ResponseBuilder'></a>
```cs
[Test]
public async Task ResponseBuilder()
{
    using var client = new MockHttpClient(request =>
    {
        var content = $"Hello to {request.RequestUri}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(content),
        };
        return response;
    });

    var result1 = await client.GetAsync("https://fake/get1");
    var result2 = await client.GetAsync("https://fake/get2");

    await Verify(new
    {
        result1,
        result2
    });
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L151-L176' title='Snippet source file'>snippet source</a> | <a href='#snippet-ResponseBuilder' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.ResponseBuilder.verified.txt -->
<a id='snippet-MockHttpClientTests.ResponseBuilder.verified.txt'></a>
```txt
{
  result1: {
    Status: 200 OK,
    Content: {
      Headers: {
        Content-Length: 26,
        Content-Type: text/plain; charset=utf-8
      },
      Value: Hello to https://fake/get1
    }
  },
  result2: {
    Status: 200 OK,
    Content: {
      Headers: {
        Content-Length: 26,
        Content-Type: text/plain; charset=utf-8
      },
      Value: Hello to https://fake/get2
    }
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ResponseBuilder.verified.txt#L1-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ResponseBuilder.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Enumeration of HttpResponseMessage

Use a sequence of `HttpResponseMessage` to return a sequence of requests:

<!-- snippet: EnumerableResponses -->
<a id='snippet-EnumerableResponses'></a>
```cs
[Test]
public async Task EnumerableResponses()
{
    using var client = new MockHttpClient(
        new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Hello")
        },
        new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("World")
        });

    var result1 = await client.GetAsync("https://fake/get1");
    var result2 = await client.GetAsync("https://fake/get2");

    await Verify(new
    {
        result1,
        result2
    });
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L124-L149' title='Snippet source file'>snippet source</a> | <a href='#snippet-EnumerableResponses' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.EnumerableResponses.verified.txt -->
<a id='snippet-MockHttpClientTests.EnumerableResponses.verified.txt'></a>
```txt
{
  result1: {
    Status: 200 OK,
    Content: {
      Headers: {
        Content-Length: 5,
        Content-Type: text/plain; charset=utf-8
      },
      Value: Hello
    }
  },
  result2: {
    Status: 200 OK,
    Content: {
      Headers: {
        Content-Length: 5,
        Content-Type: text/plain; charset=utf-8
      },
      Value: World
    }
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.EnumerableResponses.verified.txt#L1-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.EnumerableResponses.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->



### Recording Mock Interactions

Interactions with MockHttpClient (in the form of Request and repsponse pairs) can optionally be included in [Recording](https://github.com/VerifyTests/Verify/blob/main/docs/recording.md). 

<!-- snippet: RecordingMockInteractions -->
<a id='snippet-RecordingMockInteractions'></a>
```cs
[Test]
public async Task RecordingMockInteractions()
{
    using var client = new MockHttpClient(recording: true);

    Recording.Start();
    await client.GetStringAsync("https://fake/getOne");
    await client.GetStringAsync("https://fake/getTwo");

    await Verify();
}
```
<sup><a href='/src/Tests/MockHttpClientTests.cs#L307-L321' title='Snippet source file'>snippet source</a> | <a href='#snippet-RecordingMockInteractions' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: MockHttpClientTests.RecordingMockInteractions.verified.txt -->
<a id='snippet-MockHttpClientTests.RecordingMockInteractions.verified.txt'></a>
```txt
{
  httpCall: [
    {
      Request: https://fake/getOne,
      Response: 200 Ok
    },
    {
      Request: https://fake/getTwo,
      Response: 200 Ok
    }
  ]
}
```
<sup><a href='/src/Tests/MockHttpClientTests.RecordingMockInteractions.verified.txt#L1-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.RecordingMockInteractions.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Spider](https://thenounproject.com/term/spider/904683/) designed by [marialuisa iborra](https://thenounproject.com/marialuisa.iborra/) from [The Noun Project](https://thenounproject.com).
