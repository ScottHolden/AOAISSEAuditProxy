namespace AOAISSEProxy;

// This is a naive implementation, but fine for a proof of concept
public class ReadStreamInterceptor : Stream
{
    private readonly Stream _stream;
    private readonly Stream _outputStream;
    public ReadStreamInterceptor(Stream stream, Stream outputStream)
    {
        _stream = stream;
        _outputStream = outputStream;
    }

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _stream.Length;
    public override long Position { get => _stream.Position; set => _stream.Position = value; }
    public override void Flush() => _stream.Flush();
    public override int Read(byte[] buffer, int offset, int count)
    {
        int countRead = _stream.Read(buffer, offset, count);
        _outputStream.Write(buffer, offset, countRead);
        return countRead;
    }
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        int countRead = await _stream.ReadAsync(buffer, offset, count, cancellationToken);
        _outputStream.Write(buffer, offset, countRead);
        return countRead;
    }
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        int countRead = await _stream.ReadAsync(buffer, cancellationToken);
        _outputStream.Write(buffer[..countRead].Span);
        return countRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
        => _stream.Seek(offset, origin);

    public override void SetLength(long value)
        => _stream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
        => _stream.Write(buffer, offset, count);
}
