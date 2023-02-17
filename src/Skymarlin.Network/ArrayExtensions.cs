namespace Skymarlin.Network;

public static class ArrayExtensions
{
    public static string AsString(this byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace('-', ' ');
    }
    
    /// <summary>
    /// Gets the size of a packet from its header.
    /// C1 and C3 packets have a maximum length of 255, and the length defined in the second byte.
    /// C2 and C4 packets have a maximum length of 65535, and the length defined in the second and third byte.
    /// </summary>
    public static int GetPacketSize(this Span<byte> packet)
    {
        switch (packet[0])
        {
            case 0xC1:
            case 0xC3:
                return packet[1];
            case 0xC2:
            case 0xC4:
                return (packet[1] << 8) | packet[2];
            default:
                return 0;
        }
    }
}