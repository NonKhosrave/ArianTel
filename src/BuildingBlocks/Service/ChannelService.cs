using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BuildingBlocks.Service;
public sealed class ChannelService<TMessage> where TMessage : class, new()
{
    private readonly Channel<TMessage> _serviceChannel;

    public ChannelService()
    {
        _serviceChannel = Channel.CreateBounded<TMessage>(new BoundedChannelOptions(10000)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async Task Add(TMessage model, CancellationToken cancellationToken)
    {
        await _serviceChannel.Writer.WriteAsync(model, cancellationToken).ConfigureAwait(false);
    }

    public IAsyncEnumerable<TMessage> Get(CancellationToken cancellationToken)
    {
        return _serviceChannel.Reader.ReadAllAsync(cancellationToken);
    }
}
