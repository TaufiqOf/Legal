using System.Diagnostics.CodeAnalysis;

namespace Legal.Service.Infrastructure.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    { }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a meaningful message.
    /// </summary>
    /// <param name="message">User friendly message.</param>
    public NotFoundException(string? message) : base(message)
    {
    }

    public static void ThrowIfNull([NotNull] object? obj, string? message = null)
    {
        if (obj is null)
        {
            throw new NotFoundException(message);
        }
    }

    public static void ThrowIfNullOrEmpty([NotNull] string? argument, string? message = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new NotFoundException(message);
        }
    }

    public static void ThrowIf(Func<bool> predicate, string? message = null)
    {
        if (predicate?.Invoke() is true)
        {
            throw new NotFoundException(message);
        }
    }
}