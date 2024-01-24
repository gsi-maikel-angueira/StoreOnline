namespace StoreOnline.Domain.Exceptions;

public class UnsupportedOrderException : Exception
{
    public UnsupportedOrderException()
    {
    }

    public UnsupportedOrderException(string? message) : base(message)
    {
    }
}
