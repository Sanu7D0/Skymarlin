namespace Skymarlin.Utils;

/// <summary>
/// Event handler which is awaitable
/// </summary>
public delegate ValueTask AsyncEventHandler<in T>(T eventArgs);

/// <summary>
/// Event handler without arguments which is awaitable
/// </summary>
public delegate ValueTask AsyncEventHandler();

public static class AsyncEventExtensions
{
    /// <summary>
    /// Invokes an event if any event handler is registered.
    /// </summary>
    public static ValueTask SafeInvokeAsync<T>(this AsyncEventHandler<T>? handler, T args)
    {
        return handler?.Invoke(args) ?? ValueTask.CompletedTask;
    }
    
    /// <summary>
    /// Invokes an event if any event handler is registered.
    /// </summary>
    public static ValueTask SafeInvokeAsync(this AsyncEventHandler? handler)
    {
        return handler?.Invoke() ?? ValueTask.CompletedTask;
    }
}