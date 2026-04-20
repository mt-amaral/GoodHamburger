namespace GoodHamburger.Services.Exceptions;

public sealed class ApiRequestFailedException : Exception
{
    public ApiRequestFailedException(string message)
        : base(message)
    {
    }

    public ApiRequestFailedException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
