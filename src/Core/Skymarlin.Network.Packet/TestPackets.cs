using System;
using Skymarlin.Network.Packet;

namespace Skymarlin.Packet;

public readonly struct TestPacket0
{
    private readonly Memory<byte> _data;
    
    public TestPacket0(Memory<byte> data)
       : (this, true)
    {
    }
    
    private TestPacket0(Memory<byte> data, bool initialize)
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
    
    public static byte HeaderType => 0xA1;
    
    public static byte Code => 0x01;
    
    public static A1Header Header => new (_data);
    
    public Byte SenderId
    {
        get => _data.Span[0..].ReadByte();
        set => _data.Span[0..].WriteByte(value);
    }

    public String Message
    {
        get => _data.Span[1..].ReadString();
        set => _data.Span[1..].WriteString(value);
    }
    
    public static implicit operator TestPacket0(Memory<byte> packet) => new (packet, false);
    
    public static implicit operator Memory<byte>(TestPacket0 packet) => packet._data;
}

public readonly struct TestPacket1
{
    private readonly Memory<byte> _data;
    
    public TestPacket1(Memory<byte> data)
       : (this, true)
    {
    }
    
    private TestPacket1(Memory<byte> data, bool initialize)
    {
        _data = data;
        if (initialize)
        {
            var header = Header;
            header.Type = HeaderType;
            header.Code = Code;
            header.Length = (byte)data.Length;
        }
    }
    
    public static byte HeaderType => 0xE0;
    
    public static byte Code => 0x02;
    
    public static E0Header Header => new (_data);
    
    public UInt32 Id
    {
        get => _data.Span[0..].ReadUInt32();
        set => _data.Span[0..].WriteUInt32(value);
    }

    public UInt32 aaa
    {
        get => _data.Span[4..].ReadUInt32();
        set => _data.Span[4..].WriteUInt32(value);
    }

    public UInt64 bbbHeee
    {
        get => _data.Span[8..].ReadUInt64();
        set => _data.Span[8..].WriteUInt64(value);
    }

    public String bbbAaa
    {
        get => _data.Span[16..].ReadString();
        set => _data.Span[16..].WriteString(value);
    }
    
    public static implicit operator TestPacket1(Memory<byte> packet) => new (packet, false);
    
    public static implicit operator Memory<byte>(TestPacket1 packet) => packet._data;
}