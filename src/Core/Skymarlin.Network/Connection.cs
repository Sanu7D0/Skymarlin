using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Skymarlin.Utils;

namespace Skymarlin.Network;

public sealed class Connection : PacketPipeReader, IConnection
{
    private readonly Socket _socket;
    private readonly Stream _stream;
    private readonly AsyncLock _outputLock;
    
    private readonly ILogger<Connection> _logger;
    
    // TODO: lock?
    private bool _disconnected;

    public Connection(Socket socket, ILogger<Connection> logger)
    {
        _socket = socket;
        _logger = logger;

        _stream = new NetworkStream(_socket);
        _outputLock = new AsyncLock();
        Input = PipeReader.Create(_stream);
        Output = PipeWriter.Create(_stream); // TODO: Is stream sharing legal?

        EndPoint = _socket.RemoteEndPoint;
    }
    
    public event AsyncEventHandler<ReadOnlySequence<byte>>? PacketReceived;
    public event AsyncEventHandler? Disconnected;
    public EndPoint? EndPoint { get; }
    public PipeWriter Output { get; }

    public async Task BeginReceiveAsync()
    {
        try
        {
            await ReadAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception e)
        {
            await OnCompleteAsync(e).ConfigureAwait(false);
            return;
        }

        await OnCompleteAsync(null).ConfigureAwait(false);
    }

    public async ValueTask DisconnectAsync()
    {
        using IDisposable? scope = _logger.BeginScope(EndPoint!);
        
        if (_disconnected)
        {
            _logger.LogDebug("Connection already disconnected.");
            return;
        }
        
        _logger.LogDebug("Disconnecting...");

        // TODO: complete read/write
        
        _logger.LogDebug("Disconnected");
        _disconnected = true;

        await Disconnected.SafeInvokeAsync().ConfigureAwait(false);
    }
    
    public void Dispose()
    {
        ValueTask disconnectTask =  DisconnectAsync();
        disconnectTask.AsTask().GetAwaiter().GetResult();
        
        PacketReceived = null;
        Disconnected = null;
    }

    public async ValueTask SendAsync(Func<int> packetBuilder)
    {
        using (await _outputLock.LockAsync())
        {
            int length = packetBuilder();
            Output.Advance(length);
            await Output.FlushAsync().ConfigureAwait(false);
        }
    }

    protected override async ValueTask ReadPacketAsync(ReadOnlySequence<byte> packet)
    {
        await PacketReceived.SafeInvokeAsync(packet).ConfigureAwait(false);
    }

    protected override async ValueTask OnCompleteAsync(Exception? exception)
    {
        using IDisposable? scope = _logger.BeginScope(EndPoint!);

        if (exception != null)
        {
            _logger.LogError(exception, "Connection will be disconnected due to exception.");
        }

        await Output.CompleteAsync(exception).ConfigureAwait(false);
        await DisconnectAsync().ConfigureAwait(false);
    }
}