using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Orders.Commands.CreateOrder;

interface ICreateOrderServices
{
    Order CreateOrUpdate(CreateOrderCommand request);
}
