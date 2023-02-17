namespace Skymarlin.Network;

public class ClientAcceptedEventArgs : EventArgs
{
    public ClientAcceptedEventArgs(IConnection acceptedConnection)
    {
        AcceptedConnection = acceptedConnection;
    }
    
    public IConnection AcceptedConnection { get; }
}
    
