namespace ChangeLetters.Domain;

/// <summary>
/// Represents the result of an operation, including status, result value, and error message.
/// </summary>
public class HandlerResult<T>
{
    /// <summary>
    /// The result value of the operation (if successful).
    /// </summary>
    public T? Result { get; set; }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Contains the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful HandlerResult with a result value.
    /// </summary>
    public static HandlerResult<T> Success(T? result = default) => new() { Succeeded = true, Result = result };

    /// <summary>
    /// Creates a failed HandlerResult with an error message.
    /// </summary>
    public static HandlerResult<T> Failure(string errorMessage) => new() { Succeeded = false, ErrorMessage = errorMessage };
}
