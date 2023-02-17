using System.ComponentModel;
using System.Net.Sockets;

namespace Skymarlin.Network;

public class ClientAcceptingEventArgs : CancelEventArgs
{
    public ClientAcceptingEventArgs(Socket socket)
    {
        AcceptingSocket = socket;
    }
    
    public Socket AcceptingSocket { get; }
}