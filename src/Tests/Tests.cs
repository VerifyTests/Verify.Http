using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VerifyNUnit;
using NUnit.Framework;
using VerifyTests;
using VerifyTests.Web;

[TestFixture]
public class Tests
{
    #region ServiceThatDoesHttp

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

    #endregion

    #if(NET5_0_OR_GREATER)
    [Test]
    public async Task HttpClientRecordingGlobal()
    {
        #region HttpClientRecordingGlobal

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

        #endregion
    }
    #endif
    [Test]
    public async Task HttpClientRecording()
    {
        #region HttpClientRecording

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

        #endregion
    }

    [Test]
    public async Task PauseResume()
    {
        #region HttpClientPauseResume
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
        #endregion
    }

    [Test]
    public async Task RecordingFullControl()
    {
        #region HttpClientRecordingExplicit
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
        #endregion
    }
}
