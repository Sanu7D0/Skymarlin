#nullable enable
using System;
using System.Threading;
using Skymarlin.Network;

namespace Skymarlin.ChatServer.Packets;
public static class ConnectionExtensions {

    public static async ValueTask SendChatMessageAsync(this IConnection? connection, byte@senderId, string@message)
 {
        if (connection is null)
            return;
        
        int WritePacket()
        {
            var length = ChatMessage.Length;;
            var packet = new ChatMessage(connection.Output.GetSpan(length)[..length]);
            packet.SenderId = @senderId;
            packet.Message = @message;

            return packet.Header.Length;
        }
        
        await connection.SendAsync(WritePacket).ConfigureWait(false);
    }

    public static async ValueTask SendChatClientAsync(this IConnection? connection, uint@id)
 {
        if (connection is null)
            return;
        
        int WritePacket()
        {
            var length = ChatClient.Length;;
            var packet = new ChatClient(connection.Output.GetSpan(length)[..length]);
            packet.Id = @id;

            return packet.Header.Length;
        }
        
        await connection.SendAsync(WritePacket).ConfigureWait(false);
    }}