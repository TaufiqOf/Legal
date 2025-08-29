namespace Legal.Service.Infrastructure.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException()
    { }

    public ForbiddenException(string? message) : base(message)
    {
    }

    public static void Throw(string? message = null) => throw new ForbiddenException(message);

    public static void ThrowIf(Func<bool> predicate, string? message = null)
    {
        if (predicate?.Invoke() is true)
        {
            throw new ForbiddenException(message);
        }
    }
}