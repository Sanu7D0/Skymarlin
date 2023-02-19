namespace Skymarlin.ChatServer;

public sealed class ChatServer : IChatServer, IDisposable
{
    public ValueTask RegisterClientAsync(ushort roomId, string clientName)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ushort> CreateChatRoomAsync()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}