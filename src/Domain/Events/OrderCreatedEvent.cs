using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Domain.Events;

public class OrderCreatedEvent(Order item) : BaseEvent
{
    public Order Item { get; } = item;
}
