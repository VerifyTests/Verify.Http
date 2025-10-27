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

    using var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify.Http/refs/heads/main/src/sample.html");

    await Verify(result)
        .ScrubHttpTextResponse(_ => _.Replace("This is the title of the webpage", "New title"));
}
```
<sup><a href='/src/Tests/Tests.cs#L22-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScrubHttpTextResponse' title='Start of snippet'>anchor</a></sup>
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

    var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

    await Verify(result);
}
```
<sup><a href='/src/Tests/Tests.cs#L272-L284' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpResponse' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: Tests.HttpResponse.verified.txt -->
<a id='snippet-Tests.HttpResponse.verified.txt'></a>
```txt
{
  Status: 200 OK,
  Headers: {
    Accept-Ranges: bytes,
    Access-Control-Allow-Origin: *,
    Cache-Control: max-age=300,
    Connection: keep-alive,
    Cross-Origin-Resource-Policy: cross-origin,
    Strict-Transport-Security: max-age=31536000,
    Vary: Authorization,Accept-Encoding,
    Via: 1.1 varnish,
    X-Content-Type-Options: nosniff,
    X-Frame-Options: deny,
    X-XSS-Protection: 1; mode=block
  },
  Content: {
    Headers: {
      Content-Type: text/plain; charset=utf-8,
      Expires: DateTime_1
    },
    Value:
MIT License

Copyright (c) .NET Foundation and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

  }
}
```
<sup><a href='/src/Tests/Tests.HttpResponse.verified.txt#L1-L45' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.HttpResponse.verified.txt' title='Start of snippet'>anchor</a></sup>
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

    using var result = await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

    await Verify(result)
        .IgnoreMembers("Server");
}
```
<sup><a href='/src/Tests/Tests.cs#L7-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-IgnoreHeader' title='Start of snippet'>anchor</a></sup>
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
        client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");
}
```
<sup><a href='/src/Tests/Tests.cs#L77-L88' title='Snippet source file'>snippet source</a> | <a href='#snippet-ServiceThatDoesHttp' title='Start of snippet'>anchor</a></sup>
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

await Verify(recording.Sends);
```
<sup><a href='/src/Tests/Tests.cs#L211-L228' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientRecording' title='Start of snippet'>anchor</a></sup>
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

await Verify(recording.Sends);
```
<sup><a href='/src/Tests/Tests.cs#L189-L205' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientRecordingGlobal' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

<!-- snippet: Tests.HttpClientRecording.verified.txt -->
<a id='snippet-Tests.HttpClientRecording.verified.txt'></a>
```txt
[
  {
    RequestUri: https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt,
    RequestMethod: GET,
    ResponseStatus: OK 200,
    ResponseHeaders: {
      Accept-Ranges: bytes,
      Access-Control-Allow-Origin: *,
      Cache-Control: max-age=300,
      Connection: keep-alive,
      Cross-Origin-Resource-Policy: cross-origin,
      Strict-Transport-Security: max-age=31536000,
      Vary: Authorization|Accept-Encoding,
      Via: 1.1 varnish,
      X-Content-Type-Options: nosniff,
      X-Frame-Options: deny,
      X-XSS-Protection: 1; mode=block
    },
    ResponseContent:
MIT License

Copyright (c) .NET Foundation and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

  }
]
```
<sup><a href='/src/Tests/Tests.HttpClientRecording.verified.txt#L1-L43' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.HttpClientRecording.verified.txt' title='Start of snippet'>anchor</a></sup>
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

await Verify(recording.Sends);
```
<sup><a href='/src/Tests/Tests.cs#L294-L317' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientPauseResume' title='Start of snippet'>anchor</a></sup>
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

await client.GetAsync("https://www.google.com/");

recording.Resume();
await client.GetAsync("https://raw.githubusercontent.com/VerifyTests/Verify/main/license.txt");

await Verify(recording.Sends);
```
<sup><a href='/src/Tests/Tests.cs#L323-L347' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpClientRecordingExplicit' title='Start of snippet'>anchor</a></sup>
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
        .IgnoreMember("Expires")
        .ScrubLinesContaining("\"version\"");
}

static async Task<int> MethodThatDoesHttpCalls()
{
    using var client = new HttpClient();

    var jsonResult = await client.GetStringAsync("https://github.com/VerifyTests/Verify.Http/raw/main/src/global.json");
    var ymlResult = await client.GetStringAsync("https://github.com/VerifyTests/Verify.Http/raw/main/src/appveyor.yml");
    return jsonResult.Length + ymlResult.Length;
}
```
<sup><a href='/src/Tests/Tests.cs#L122-L149' title='Snippet source file'>snippet source</a> | <a href='#snippet-HttpRecording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Resulting verified file

The requests/response pairs will be appended to the verified file.

<!-- snippet: Tests.TestHttpRecording.verified.txt -->
<a id='snippet-Tests.TestHttpRecording.verified.txt'></a>
```txt
{
  target: {
    sizeOfResponse: 771
  },
  httpCall: [
    {
      Status: Created,
      Request: {
        Uri: https://github.com/VerifyTests/Verify.Http/raw/main/src/global.json,
        Headers: {}
      },
      Response: {
        Status: 302 Found,
        Headers: {
          Access-Control-Allow-Origin: ,
          Cache-Control: no-cache,
          Location: https://raw.githubusercontent.com/VerifyTests/Verify.Http/main/src/global.json,
          Referrer-Policy: no-referrer-when-downgrade,
          Strict-Transport-Security: max-age=31536000; includeSubdomains; preload,
          Vary: X-PJAX,X-PJAX-Container,Turbo-Visit,Turbo-Frame,X-Requested-With,Accept-Encoding,Accept,X-Requested-With,
          X-Content-Type-Options: nosniff,
          X-Frame-Options: deny,
          X-XSS-Protection: 0
        },
        ContentHeaders: {
          Content-Type: text/html; charset=utf-8
        },
        ContentString: 
      }
    },
    {
      Status: Created,
      Request: {
        Uri: https://raw.githubusercontent.com/VerifyTests/Verify.Http/main/src/global.json,
        Headers: {}
      },
      Response: {
        Status: 200 OK,
        Headers: {
          Accept-Ranges: bytes,
          Access-Control-Allow-Origin: *,
          Cache-Control: max-age=300,
          Connection: keep-alive,
          Cross-Origin-Resource-Policy: cross-origin,
          Strict-Transport-Security: max-age=31536000,
          Vary: Authorization,Accept-Encoding,
          Via: 1.1 varnish,
          X-Content-Type-Options: nosniff,
          X-Frame-Options: deny,
          X-XSS-Protection: 1; mode=block
        },
        ContentHeaders: {
          Content-Type: text/plain; charset=utf-8
        },
        ContentString:
{
  "sdk": {
    "allowPrerelease": true,
    "rollForward": "latestFeature"
  }
}
      }
    },
    {
      Status: Created,
      Request: {
        Uri: https://github.com/VerifyTests/Verify.Http/raw/main/src/appveyor.yml,
        Headers: {}
      },
      Response: {
        Status: 302 Found,
        Headers: {
          Access-Control-Allow-Origin: ,
          Cache-Control: no-cache,
          Location: https://raw.githubusercontent.com/VerifyTests/Verify.Http/main/src/appveyor.yml,
          Referrer-Policy: no-referrer-when-downgrade,
          Strict-Transport-Security: max-age=31536000; includeSubdomains; preload,
          Vary: X-PJAX,X-PJAX-Container,Turbo-Visit,Turbo-Frame,X-Requested-With,Accept-Encoding,Accept,X-Requested-With,
          X-Content-Type-Options: nosniff,
          X-Frame-Options: deny,
          X-XSS-Protection: 0
        },
        ContentHeaders: {
          Content-Type: text/html; charset=utf-8
        },
        ContentString: 
      }
    },
    {
      Status: Created,
      Request: {
        Uri: https://raw.githubusercontent.com/VerifyTests/Verify.Http/main/src/appveyor.yml,
        Headers: {}
      },
      Response: {
        Status: 200 OK,
        Headers: {
          Accept-Ranges: bytes,
          Access-Control-Allow-Origin: *,
          Cache-Control: max-age=300,
          Connection: keep-alive,
          Cross-Origin-Resource-Policy: cross-origin,
          Strict-Transport-Security: max-age=31536000,
          Vary: Authorization,Accept-Encoding,
          Via: 1.1 varnish,
          X-Content-Type-Options: nosniff,
          X-Frame-Options: deny,
          X-XSS-Protection: 1; mode=block
        },
        ContentHeaders: {
          Content-Type: text/plain; charset=utf-8
        },
        ContentString:
image: Visual Studio 2022
environment:
  DOTNET_NOLOGO: true
  DOTNET_CLI_TELEMETRY_OPTOUT: true
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
build_script:
- pwsh: |
    Invoke-WebRequest "https://dot.net/v1/dotnet-install.ps1" -OutFile "./dotnet-install.ps1"
    ./dotnet-install.ps1 -JSonFile src/global.json -Architecture x64 -InstallDir 'C:\Program Files\dotnet'
- dotnet build src --configuration Release
- dotnet test src --configuration Release --no-build --no-restore --filter Category!=Integration
test: off
on_failure:
  - ps: Get-ChildItem *.received.* -recurse | % { Push-AppveyorArtifact $_.FullName -FileName $_.Name }
artifacts:
- path: nugets\*.nupkg
      }
    }
  ]
}
```
<sup><a href='/src/Tests/Tests.TestHttpRecording.verified.txt#L1-L133' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.TestHttpRecording.verified.txt' title='Start of snippet'>anchor</a></sup>
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
  responseSize: 771,
  httpCalls: [
    https://github.com/VerifyTests/Verify.Http/raw/main/src/global.json,
    https://raw.githubusercontent.com/VerifyTests/Verify.Http/main/src/global.json,
    https://github.com/VerifyTests/Verify.Http/raw/main/src/appveyor.yml,
    https://raw.githubusercontent.com/VerifyTests/Verify.Http/main/src/appveyor.yml
  ]
}
```
<sup><a href='/src/Tests/Tests.TestHttpRecordingExplicit.verified.txt#L1-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.TestHttpRecordingExplicit.verified.txt' title='Start of snippet'>anchor</a></sup>
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
  Version: 1.1,
  Status: 200 OK
}
```
<sup><a href='/src/Tests/MockHttpClientTests.DefaultContent.verified.txt#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.DefaultContent.verified.txt' title='Start of snippet'>anchor</a></sup>
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
      Content-Type: application/json; charset=utf-8
    },
    Value: {
      a: b
    }
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ExplicitContent.verified.txt#L1-L11' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ExplicitContent.verified.txt' title='Start of snippet'>anchor</a></sup>
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
  Version: 1.1,
  Status: 300 Multiple Choices
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ExplicitStatusCode.verified.txt#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ExplicitStatusCode.verified.txt' title='Start of snippet'>anchor</a></sup>
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
      Content-Type: text/plain; charset=utf-8
    },
    Value: Hello
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ExplicitResponse.verified.txt#L1-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ExplicitResponse.verified.txt' title='Start of snippet'>anchor</a></sup>
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
        Content-Type: text/plain; charset=utf-8
      },
      Value: Hello to https://fake/get1
    }
  },
  result2: {
    Status: 200 OK,
    Content: {
      Headers: {
        Content-Type: text/plain; charset=utf-8
      },
      Value: Hello to https://fake/get2
    }
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.ResponseBuilder.verified.txt#L1-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.ResponseBuilder.verified.txt' title='Start of snippet'>anchor</a></sup>
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
        Content-Type: text/plain; charset=utf-8
      },
      Value: Hello
    }
  },
  result2: {
    Status: 200 OK,
    Content: {
      Headers: {
        Content-Type: text/plain; charset=utf-8
      },
      Value: World
    }
  }
}
```
<sup><a href='/src/Tests/MockHttpClientTests.EnumerableResponses.verified.txt#L1-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-MockHttpClientTests.EnumerableResponses.verified.txt' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/src/Tests/MockHttpClientTests.cs#L314-L328' title='Snippet source file'>snippet source</a> | <a href='#snippet-RecordingMockInteractions' title='Start of snippet'>anchor</a></sup>
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
