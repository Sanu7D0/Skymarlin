using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Skymarlin.Utils;

namespace Skymarlin.Network;
public class Listener
{
    private readonly int _port;
    private TcpListener? _clientListener;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    public Listener(int port, ILoggerFactory loggerFactory)
    {
        _port = port;
        _loggerFactory = loggerFactory;

        _logger = _loggerFactory.CreateLogger<Listener>();
    }

    public event AsyncEventHandler<ClientAcceptingEventArgs>? ClientAccepting;
    
    public event AsyncEventHandler<ClientAcceptedEventArgs>? ClientAccepted;

    public bool IsBound => _clientListener?.Server.IsBound ?? false;

    public void Start(int backlog = (int)SocketOptionName.MaxConnections)
    {
        _clientListener = new TcpListener(IPAddress.Any, _port);
        _clientListener.Start(backlog);
        _clientListener.BeginAcceptSocket(OnAccept, null);
    }

    public void Stop()
    {
        _clientListener?.Stop();
    }

    private async void OnAccept(IAsyncResult result)
    {
        try
        {
            Socket socket;

            try
            {
                if (_clientListener is null)
                    return;

                socket = _clientListener.EndAcceptSocket(result);
            }
            catch (ObjectDisposedException)
            {
                // Ignore
                return;
            }
            catch (SocketException e) when (e.ErrorCode == (int)SocketError.OperationAborted)
            {
                _logger.LogDebug(e, "The listener was stopped.");
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error accepting the client socket.");
                throw;
            }
            
            // Accept the next client
            if (IsBound)
                _clientListener.BeginAcceptSocket(OnAccept, null);

            ClientAcceptingEventArgs? cancel = null;
            if (ClientAccepting is { } clientAccepting)
            {
                cancel = new ClientAcceptingEventArgs(socket);
                await clientAccepting.Invoke(cancel).ConfigureAwait(false);
            }

            if (cancel is null || !cancel.Cancel)
            {
                socket.NoDelay = true;
                IConnection connection = CreateConnection(socket);

                if (ClientAccepted is { } clientAccepted)
                {
                    await ClientAccepted.Invoke(new ClientAcceptedEventArgs(connection)).ConfigureAwait(false);
                }
            }
            else
            {
                socket.Dispose();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error.");
        }
    }

    private IConnection CreateConnection(Socket clientSocket)
    {
        var connection = new Connection(clientSocket, _loggerFactory.CreateLogger<Connection>());
        return connection;
    }
}