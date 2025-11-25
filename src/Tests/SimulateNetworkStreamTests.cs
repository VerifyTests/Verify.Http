// ReSharper disable UnusedVariable
[TestFixture]
public class SimulateNetworkStreamTests
{
    [Test]
    public async Task WithoutSimulateNetworkStream()
    {
        #region WithoutSimulateNetworkStream

        using var client = new MockHttpClient("sample.html");
        using var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        await using var stream = await result.Content.ReadAsStreamAsync();

        // Stream is seekable (MemoryStream-like behavior)
        IsTrue(stream.CanSeek);
        // Can reset
        stream.Position = 0;

        #endregion
    }

    [Test]
    public async Task WithSimulateNetworkStream()
    {
        #region WithSimulateNetworkStream

        using var client = new MockHttpClient("sample.html");
        client.SimulateNetworkStream = true;
        var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var stream = await result.Content.ReadAsStreamAsync();

        // Stream is non-seekable (real network stream behavior)
        IsFalse(stream.CanSeek);
        // stream.Position throws NotSupportedException
        // Cannot reset or re-read the stream

        #endregion
    }

    [Test]
    public async Task ResponseHeadersRead_StreamIsNotSeekable()
    {
        using var client = new MockHttpClient("sample.html");

        using var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        await using var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() => stream.Position = 1);
    }

    [Test]
    public async Task ResponseHeadersRead_LengthIsNotAvaliable()
    {
        using var client = new MockHttpClient("sample.html");

        using var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        await using var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() =>
        {
            // ReSharper disable once UnusedVariable
            var x = stream.Length;
        });
    }

    [Test]
    public async Task ResponseHeadersRead_WithFile_StreamIsNotSeekable()
    {
        using var client = new MockHttpClient("sample.html");
        client.SimulateNetworkStream = true;

        using var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        await using var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() => stream.Position = 1);
    }

    [Test]
    public async Task ResponseHeadersRead_WithFile_LengthIsNotAvaliable()
    {
        using var client = new MockHttpClient("sample.html");
        client.SimulateNetworkStream = true;

        using var result = await client.GetAsync("https://fake/get", HttpCompletionOption.ResponseHeadersRead);
        var content = result.Content;
        var stream = await content.ReadAsStreamAsync();

        Assert.Throws<NotSupportedException>(() =>
        {
            // ReSharper disable once UnusedVariable
            var x = stream.Length;
        });
    }

    #region VerifyWithSimulateNetworkStream

    [Test]
    public async Task VerifyWithSimulateNetworkStream()
    {
        using var client = new MockHttpClient("sample.html");
        client.SimulateNetworkStream = true;

        var response = await client.GetAsync(
            "https://fake/api/data",
            HttpCompletionOption.ResponseHeadersRead);

        await Verify(response);
    }

    #endregion

    [Test]
    public async Task ProgressiveReading()
    {
        #region ProgressiveReading

        using var client = new MockHttpClient("sample.html");
        client.SimulateNetworkStream = true;

        var response = await client.GetAsync(
            "https://fake/large-file",
            HttpCompletionOption.ResponseHeadersRead);

        // Read headers first without loading entire content
        response.EnsureSuccessStatusCode();
        var contentLength = response.Content.Headers.ContentLength;

        // Then read content progressively
        await using var stream = await response.Content.ReadAsStreamAsync();
        var buffer = new byte[8192];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            // Process chunks as they arrive
            ProcessChunk(buffer.AsSpan(0, bytesRead));
        }

        #endregion
    }

    // ReSharper disable once UnusedParameter.Local
    static void ProcessChunk(Span<byte> span)
    {
    }

    [Test]
    public async Task ReadOnceBehavior()
    {
        #region ReadOnceBehavior

        using var client = new MockHttpClient("sample.html");
        client.SimulateNetworkStream = true;

        using var response = await client.GetAsync(
            "https://fake/data",
            HttpCompletionOption.ResponseHeadersRead);

        await using var stream = await response.Content.ReadAsStreamAsync();

        // First read succeeds
        using (var reader = new StreamReader(stream))
        {
            var data1 = await reader.ReadToEndAsync();
        }

        // Second read returns empty (stream already consumed)
        // Throws NotSupportedException
        Assert.Throws<NotSupportedException>(() =>
        {
            stream.Position = 0;
        });
        // Returns empty
        var data2 = await response.Content.ReadAsStringAsync();

        #endregion
    }
}