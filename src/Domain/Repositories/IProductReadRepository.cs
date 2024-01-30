using StoreOnline.Domain.Common;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Domain.Repositories;

public interface IProductReadRepository : IReadRepository<int, Product>;
