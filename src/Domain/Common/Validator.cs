namespace StoreOnline.Domain.Common;

public interface IValidator<in T>
{
    bool Validate(T data);
}
