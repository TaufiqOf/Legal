namespace Legal.Service.Infrastructure.Exceptions;

public class UserDisabledException : Exception
{
    public UserDisabledException()
    { }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a meaningful message.
    /// </summary>
    /// <param name="message">User friendly message.</param>
    public UserDisabledException(string? message) : base(message)
    {
    }
}