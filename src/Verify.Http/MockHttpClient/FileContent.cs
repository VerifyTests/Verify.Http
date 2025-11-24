class FileContent : HttpContent
{
    string path;
    bool simulateNetworkStream;

    public FileContent(string path, bool simulateNetworkStream)
    {
        this.path = path;
        this.simulateNetworkStream = simulateNetworkStream;

        var extension = Path.GetExtension(path);
        if (ContentTypes.TryGetMediaType(extension, out var mediaType))
        {
            Headers.ContentType = new(mediaType);
        }
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        await using var read = File.OpenRead(path);

        if (simulateNetworkStream)
        {
            await using var wrapper = new NonSeekableStreamWrapper(read);
            await wrapper.CopyToAsync(stream);
        }
        else
        {
            await read.CopyToAsync(stream);
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        if (simulateNetworkStream)
        {
            length = 0;
            return false;
        }

        length = new FileInfo(path).Length;
        return true;
    }
}