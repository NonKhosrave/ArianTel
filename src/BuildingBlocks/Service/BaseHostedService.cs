using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Logger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace BuildingBlocks.Service;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public abstract class BaseHostedService : IHostedService
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly ILogger<BaseHostedService> _logger;
    private readonly TimeSpan _timeSpan;
    private Timer _timer;

    protected BaseHostedService(ILogger<BaseHostedService> logger, TimeSpan timeSpan)
    {
        _logger = logger;
        _timeSpan = timeSpan;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer
        {
            AutoReset = false,
            Interval = _timeSpan.TotalMilliseconds,
            Enabled = true
        };
        _timer.Elapsed += (s, e) => ExecuteCore();

        _timer.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        return Task.CompletedTask;
    }

    private void ExecuteCore()
    {
        try
        {
            Execute();
        }
        catch (Exception ex)
        {
            _logger.CompileLog(ex, LogLevel.Error, ex.Message);
        }
        finally
        {
            _timer.Start();
        }
    }

    protected abstract void Execute();
}
