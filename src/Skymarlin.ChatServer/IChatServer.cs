namespace Skymarlin.ChatServer;

public interface IChatServer
{
    ValueTask RegisterClientAsync(ushort roomId, string clientName);

    ValueTask<ushort> CreateChatRoomAsync();
}