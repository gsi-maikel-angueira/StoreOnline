using StoreOnline.Domain.Common;

namespace StoreOnline.Domain.Entities;

public class Order : BaseEntity
{
    public required string OrderNumber { get; init; }
    public required DateTime CreatedDate { get; init; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = new();
}
