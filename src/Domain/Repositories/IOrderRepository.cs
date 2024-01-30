using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Domain.Repositories;

public interface IOrderRepository : IRepository<Order>, IReadRepository<int, Order>;
