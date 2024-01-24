using StoreOnline.Domain.Common;

namespace StoreOnline.Domain.Entities;

public class Product : BaseEntity
{
    public required string Name { get; init; }
    public required double Price { get; init; }
    public int Stock { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = new();
}
