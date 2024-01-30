namespace StoreOnline.Domain.Common;

public interface IValidator<in T>
{
    Task<bool> Validate(T data);
}
