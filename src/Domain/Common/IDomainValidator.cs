namespace StoreOnline.Domain.Common;

public interface IDomainValidator<in T> where T : notnull
{
    Task<bool> Validate(T data);
}
