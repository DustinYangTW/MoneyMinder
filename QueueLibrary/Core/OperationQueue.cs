using System.Collections.Concurrent;
using System.Threading.Channels;

namespace QueueLibrary.Core;

/// <summary>
/// Provides a queued execution mechanism so read/write operations are executed sequentially.
/// </summary>
public class OperationQueue : IAsyncDisposable
{
    private readonly Channel<Func<Task>> _channel;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _processingTask;

    public OperationQueue()
    {
        _channel = Channel.CreateUnbounded<Func<Task>>();
        _processingTask = Task.Run(ProcessQueueAsync);
    }

    /// <summary>
    /// Enqueue an asynchronous operation to run sequentially.
    /// </summary>
    /// <param name="operation">Operation to run.</param>
    public Task EnqueueAsync(Func<Task> operation)
    {
        if (operation is null) throw new ArgumentNullException(nameof(operation));
        var tcs = new TaskCompletionSource();
        _channel.Writer.TryWrite(async () =>
        {
            try
            {
                await operation();
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return tcs.Task;
    }

    /// <summary>
    /// Enqueue an asynchronous operation returning a value to run sequentially.
    /// </summary>
    /// <typeparam name="T">Type of result.</typeparam>
    /// <param name="operation">Operation to run.</param>
    public Task<T> EnqueueAsync<T>(Func<Task<T>> operation)
    {
        if (operation is null) throw new ArgumentNullException(nameof(operation));
        var tcs = new TaskCompletionSource<T>();
        _channel.Writer.TryWrite(async () =>
        {
            try
            {
                var result = await operation();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return tcs.Task;
    }

    private async Task ProcessQueueAsync()
    {
        var reader = _channel.Reader;
        while (await reader.WaitToReadAsync(_cts.Token))
        {
            while (reader.TryRead(out var operation))
            {
                try
                {
                    await operation();
                }
                catch
                {
                    // Exceptions are captured in individual task completion sources.
                }
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _channel.Writer.Complete();
        try
        {
            await _processingTask;
        }
        catch
        {
            // Ignore errors during shutdown
        }
        _cts.Dispose();
    }
}
