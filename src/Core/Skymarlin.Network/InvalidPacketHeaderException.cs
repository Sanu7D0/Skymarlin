using System.Buffers;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Skymarlin.Network;

public class InvalidPacketHeaderException : Exception
{
    private string? _message;

    public InvalidPacketHeaderException(byte[] header, ReadOnlySequence<byte> bufferContent, SequencePosition position)
    {
        Header = header.ToArray();
        BufferContent = bufferContent.ToArray();
        Position = position.GetInteger();
    }
    
    public byte[] Header { get; }
    
    public byte[] BufferContent { get; }
    
    public int Position { get; }

    public override string Message => _message ?? BuildMessage();

    private string BuildMessage()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder
            .Append("The packet header is invalid: ").AppendLine(BitConverter.ToString(Header))
            .Append("Buffer position: ").AppendLine(Position.ToString())
            .Append("Buffer content: ").AppendLine(BitConverter.ToString(BufferContent));
        return stringBuilder.ToString();
    }
}