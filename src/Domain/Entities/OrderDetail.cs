using StoreOnline.Domain.Common;

namespace StoreOnline.Domain.Entities;

public class OrderDetail : BaseEntity
{
    public int? OrderId { get; set; }
    public int? ProductId { get; set; }
    public int Quantity { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
