namespace StoreOnline.Domain.Common;

public interface IDomainValidator<in T>
{
    Task<bool> Validate(T data);
}
