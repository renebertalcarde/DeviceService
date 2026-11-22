using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using zkemkeeper;

namespace DeviceService.Services;

public sealed class DeviceSession : IAsyncDisposable
{
    private readonly BlockingCollection<Action> _queue = new();

    private readonly Thread _thread;

    private readonly TaskCompletionSource _ready =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private CZKEM? _zk;

    private bool _disposed;

    public DeviceSession()
    {
        _thread = new Thread(ThreadMain)
        {
            IsBackground = true,
            Name = "ZKTeco COM Worker"
        };

        _thread.SetApartmentState(ApartmentState.STA);

        _thread.Start();
    }

    private void ThreadMain()
    {
        try
        {
            _zk = new CZKEM();

            _ready.TrySetResult();

            foreach (var action in _queue.GetConsumingEnumerable())
            {
                action();
            }
        }
        catch (Exception ex)
        {
            _ready.TrySetException(ex);
        }
        finally
        {
            if (_zk != null)
            {
                try
                {
                    _zk.Disconnect();
                }
                catch
                {
                    // Best-effort cleanup.
                }

                if (Marshal.IsComObject(_zk))
                {
                    Marshal.FinalReleaseComObject(_zk);
                }

                _zk = null;
            }
        }
    }

    public async Task<T> ExecuteAsync<T>(
        Func<CZKEM, T> operation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _ready.Task.ConfigureAwait(false);

        var completion = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _queue.Add(() =>
        {
            try
            {
                completion.TrySetResult(operation(_zk!));
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        });

        return await completion.Task.ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_ready.Task.IsCompletedSuccessfully)
        {
            _queue.CompleteAdding();

            await Task.Run(() => _thread.Join());
        }
        else
        {
            _queue.CompleteAdding();

            await Task.Run(() => _thread.Join());
        }

        _queue.Dispose();
    }
}