using System.Net;

namespace Skymarlin.Network;

public class Connection : IConnection
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool Connected { get; }
    
    public EndPoint? EndPoint { get; }
    
    public async Task BeginReceiveAsync()
    {
        throw new NotImplementedException();
    }

    public async ValueTask DisconnectAsync()
    {
        throw new NotImplementedException();
    }
}