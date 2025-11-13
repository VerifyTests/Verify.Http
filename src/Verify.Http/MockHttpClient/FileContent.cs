class FileContent :
    HttpContent
{
    readonly string path;

    public FileContent(string path)
    {
        this.path = path;

        var extension = Path.GetExtension(path);
        if (ContentTypes.TryGetMediaType(extension, out var mediaType))
        {
            Headers.ContentType = new(mediaType);
        }
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        await using var read = File.OpenRead(path);
        await read.CopyToAsync(stream);
    }

    protected override bool TryComputeLength(out long length)
    {
        length = new FileInfo(path).Length;
        return true;
    }
}