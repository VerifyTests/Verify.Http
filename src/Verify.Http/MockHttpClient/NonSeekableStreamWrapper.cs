class NonSeekableStreamWrapper(Stream strean) :
    Stream
{
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;

    public override long Length =>
        throw new NotSupportedException("This stream does not support seeking.");

    public override long Position
    {
        get => throw new NotSupportedException("This stream does not support seeking.");
        set => throw new NotSupportedException("This stream does not support seeking.");
    }

    public override int Read(byte[] buffer, int offset, int count) =>
        strean.Read(buffer, offset, count);

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, Cancel cancel) =>
        strean.ReadAsync(buffer, offset, count, cancel);

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, Cancel cancel = default) =>
        strean.ReadAsync(buffer, cancel);

    public override long Seek(long offset, SeekOrigin origin) =>
        throw new NotSupportedException("This stream does not support seeking.");

    public override void SetLength(long value) =>
        throw new NotSupportedException("This stream does not support seeking.");

    public override void Flush() { }

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException("This stream does not support writing.");
}