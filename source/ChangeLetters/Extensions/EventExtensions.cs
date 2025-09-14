using System.Diagnostics;

namespace ChangeLetters.Extensions;

/// <summary>
/// The <see cref="EventExtensions"/> class.
/// </summary>
public static class EventExtensions
{
    /// <summary>Raises the specified event.</summary>
    /// <param name="action">The action.</param>
    /// <returns>See description.</returns>
    [DebuggerHidden]
    public static void Raise(this Action? action)
        => action?.Invoke();

    /// <summary>Raises the specified event.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action">The action.</param>
    /// <param name="value">The value.</param>
    /// <returns>See description.</returns>
    [DebuggerHidden]
    public static void Raise<T>(this Action<T>? action, T value)
        => action?.Invoke(value);

    /// <summary>Raises the event as an asynchronous operation.</summary>
    /// <param name="func">The function.</param>
    /// <returns>The completed Task.</returns>
    [DebuggerHidden]
    public static Task RaiseAsync(this Func<Task>? func)
    {
        if (func != null)
        {
            return Task.WhenAll(func.GetInvocationList()
                .OfType<Func<Task>>()
                .Select(e => e()));
        }
        return Task.CompletedTask;
    }

    /// <summary>Raises the event as an asynchronous operation.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func">The function.</param>
    /// <param name="value">The value.</param>
    /// <returns>The completed Task.</returns>
    [DebuggerHidden]
    public static Task RaiseAsync<T>(this Func<T, Task>? func, T value)
    {
        if (func != null)
        {
            return Task.WhenAll(func.GetInvocationList()
                .OfType<Func<T,Task>>()
                .Select(e => e(value)));
        }
        return Task.CompletedTask;
    }
}
