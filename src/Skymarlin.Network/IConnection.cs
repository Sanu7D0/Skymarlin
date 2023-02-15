using System.Net;

namespace Skymarlin.Network;

public interface IConnection : IDisposable
{
    bool Connected { get; }
    
    EndPoint? EndPoint { get; }

    Task BeginReceiveAsync();

    ValueTask DisconnectAsync();
}