using System.Buffers;
using System.IO.Pipelines;
using Skymarlin.Network.Packet;

namespace Skymarlin.Network;

public abstract class PacketPipeReader
{
    private readonly byte[] _headerBuffer = new byte[3];

    protected PipeReader Input { get; init; } = null!;

    protected abstract ValueTask ReadPacketAsync(ReadOnlySequence<byte> packet);

    protected abstract ValueTask OnCompleteAsync(Exception? exception);

    protected async Task ReadAsync()
    {
        try
        {
            while (true)
            {
                bool completed = await ReadBufferAsync().ConfigureAwait(false);

                if (completed)
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
        catch (Exception e)
        {
            await OnCompleteAsync(e).ConfigureAwait(false);
            throw;
        }

        await OnCompleteAsync(null).ConfigureAwait(false);
    }

    private async Task<bool> ReadBufferAsync()
    {
        ReadResult result = await Input.ReadAsync().ConfigureAwait(false);
        ReadOnlySequence<byte> buffer = result.Buffer;
        int? length = null;

        do
        {
            if (buffer.Length > 2)
            {
                buffer.Slice(0, 3).CopyTo(_headerBuffer);
                length = _headerBuffer.GetPacketSize();

                if (length == 0)
                {
                    var exception = new InvalidPacketHeaderException(_headerBuffer, result.Buffer, buffer.Start);

                    await Input.CompleteAsync(exception).ConfigureAwait(false);
                    await OnCompleteAsync(exception).ConfigureAwait(false);

                    throw exception;
                }
            }

            if (length is > 0 && buffer.Length >= length)
            {
                ReadOnlySequence<byte> packet = buffer.Slice(0, length.Value);
                await ReadPacketAsync(packet).ConfigureAwait(false);

                buffer = buffer.Slice(buffer.GetPosition(length.Value), buffer.End);
                length = null;
            }
            else
            {
                // Read more
                break;
            }
        }
        while (buffer.Length > 2);

        if (result.IsCanceled || result.IsCompleted)
        {
            await OnCompleteAsync(null).ConfigureAwait(false);
        }
        else
        {
            Input.AdvanceTo(buffer.Start);
        }

        return result.IsCompleted;
    }
}