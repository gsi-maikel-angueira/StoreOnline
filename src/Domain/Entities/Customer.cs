using StoreOnline.Domain.Common;

namespace StoreOnline.Domain.Entities;

public class Customer : BaseEntity
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string CardNumber { get; init; }

    public List<Order> Orders { get; set; } = new();
}
