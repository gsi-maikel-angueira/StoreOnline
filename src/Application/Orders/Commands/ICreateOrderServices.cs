using StoreOnline.Application.Common.Models;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.Orders.Commands;

interface ICreateOrderServices<in T> where T:IOrderCommand
{
    Task<Order> CreateOrUpdateAsync(T request);
}

public interface IOrderCommand
{
    int CustomerId { get; }
    List<ProductDto> Products { get; }
};
