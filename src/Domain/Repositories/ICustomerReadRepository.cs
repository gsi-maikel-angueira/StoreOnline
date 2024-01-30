using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Domain.Repositories;

public interface ICustomerReadRepository : IReadRepository<int, Customer>;
