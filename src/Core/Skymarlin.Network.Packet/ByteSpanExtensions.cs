using System.Text;

namespace Skymarlin.Network.Packet;

public static class SpanExtensions
{
    public static int GetPacketSize(this Span<byte> packet)
    {
        switch (packet[0])
        {
            case 0xA0:
            case 0xE0:
                return packet[1];
            case 0xA1:
            case 0xE1:
                return (packet[1] << 8) | packet[2];
            default:
                return 0;
        }
    }

    public static int GetPacketSize(this byte[] packet)
    {
        return packet.AsSpan().GetPacketSize();
    }
    
    public static bool ReadBoolean(this Span<byte> span)
    {
        return BitConverter.ToBoolean(span);
    }
    
    public static ushort ReadUInt16(this Span<byte> span)
    {
        return BitConverter.ToUInt16(span);
    }
        
    public static uint ReadUInt32(this Span<byte> span)
    {
        return BitConverter.ToUInt32(span);
    }
        
    public static float ReadSingle(this Span<byte> span)
    {
        return BitConverter.ToSingle(span);
    }
    
    public static double ReadDouble(this Span<byte> span)
    {
        return BitConverter.ToDouble(span);
    }
        
    public static string ReadString(this Span<byte> span)
    {
        ushort length = span.ReadUInt16();
        string str = Encoding.UTF8.GetString(span[sizeof(ushort)..(sizeof(ushort) + length)]);
        return str;
    }
    
    public static bool WriteBoolean(this Span<byte> span, bool value)
    {
        return BitConverter.TryWriteBytes(span, value);
    }
    
    public static bool WriteUInt16(this Span<byte> span, ushort value)
    {
        return BitConverter.TryWriteBytes(span, value);
    }
        
    public static bool WriteUInt32(this Span<byte> span, uint value)
    {
        return BitConverter.TryWriteBytes(span, value);
    }
        
    public static bool WriteSingle(this Span<byte> span, float value)
    {
        return BitConverter.TryWriteBytes(span, value);
    }
    
    public static bool WriteDouble(this Span<byte> span, double value)
    {
        return BitConverter.TryWriteBytes(span, value);
    }
        
    public static bool WriteString(this Span<byte> span, string str)
    {
        ushort length = (ushort)Encoding.UTF8.GetByteCount(str);
        span.WriteUInt16(length);

        return length == (ushort)Encoding.UTF8.GetBytes(str, span[sizeof(ushort)..]);
    }
}