using System;
using Skymarlin.Network.Packet;

namespace Skymarlin.ChatServer.Packets;
    public readonly struct ChatMessage
{
    private readonly Memory<byte> _data;

    public ChatMessage(Memory<byte> data)
    : this(data, true)
    {
    }

    private ChatMessage(Memory<byte> data, bool initialize)
    {
        _data = data;
        if (initialize)
        {
            var header = Header;
            header.Type = HeaderType;
            header.Code = Code;
            header.Length = (ushort)data.Length;
        }
    }

