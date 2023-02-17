using System.Buffers;
using System.Net;
using Skymarlin.Utils;

namespace Skymarlin.Network;

public interface IConnection : IDisposable
{
    event AsyncEventHandler<ReadOnlySequence<byte>>? PacketReceived;

    event AsyncEventHandler? Disconnected;
    
    // bool Connected { get; }
    
    EndPoint? EndPoint { get; }

    Task BeginReceiveAsync();

    ValueTask DisconnectAsync();
}