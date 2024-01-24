namespace StoreOnline.Domain.Exceptions;

public class ProductExceedLimitOnStockException : Exception
{
    public ProductExceedLimitOnStockException()
    {
    }

    public ProductExceedLimitOnStockException(string? message) : base(message)
    {
    }
}
