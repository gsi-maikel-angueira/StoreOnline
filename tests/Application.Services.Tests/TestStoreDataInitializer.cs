using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Services;
using StoreOnline.Domain.Entities;

namespace StoreOnline.Application.UnitTests.Common.Behaviours;

public static class TestStoreDataInitializer
{
    static TestStoreDataInitializer()
    {
        Initialize();
    }

    private static void Initialize()
    {
        CreatedOrder = new Order
        {
            Id = 1, CreatedDate = DateTime.Today, CustomerId = 1, OrderNumber = RandomGenerator.NewKey()
        };
        ProductDtoA = new ProductDto { ProductId = 1, Quantity = 5 };
        ProductDtoB = new ProductDto { ProductId = 2, Quantity = 10 };
        ProductA = new Product() { Id = 1, Name = "Memory Usb Flash", Price = 25.0, Stock = 50 };
        ProductB = new Product() { Id = 2, Name = "Adapter Hdmi", Price = 12.0, Stock = 20 };
        OrderDetailA = new OrderDetail
        {
            Id = 1,
            OrderId = 1,
            Order = CreatedOrder,
            ProductId = 1,
            Product = ProductA,
            Quantity = 5
        };
        OrderDetailB = new OrderDetail
        {
            Id = 2,
            OrderId = 1,
            Order = CreatedOrder,
            ProductId = 2,
            Product = ProductB,
            Quantity = 10
        };
        CreatedOrder.OrderDetails = new List<OrderDetail> { OrderDetailA, OrderDetailB };
    }

    public static Product ProductB { get; set; } = null!;

    public static Product ProductA { get; set; } = null!;

    public static Order CreatedOrder { get; set; } = null!;

    public static OrderDetail OrderDetailB { get; set; } = null!;

    public static OrderDetail OrderDetailA { get; set; } = null!;

    public static ProductDto ProductDtoB { get; set; } = null!;

    public static ProductDto ProductDtoA { get; set; } = null!;
}
