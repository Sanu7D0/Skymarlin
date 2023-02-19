using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using Skymarlin.Utils;

namespace Skymarlin.Network;

public interface IConnection : IDisposable
{
    event AsyncEventHandler<ReadOnlySequence<byte>>? PacketReceived;

    event AsyncEventHandler? Disconnected;
    
    // bool Connected { get; }
    
    EndPoint? EndPoint { get; }

    PipeWriter Output { get; }

    Task BeginReceiveAsync();

    ValueTask DisconnectAsync();
}