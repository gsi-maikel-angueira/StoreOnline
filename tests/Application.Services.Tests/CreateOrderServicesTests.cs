using FluentAssertions;
using Moq;
using NUnit.Framework;
using StoreOnline.Application.Common.Models;
using StoreOnline.Application.Orders.Commands.CreateOrder;
using StoreOnline.Application.Services;
using StoreOnline.Domain.Entities;
using StoreOnline.Domain.Exceptions;
using StoreOnline.Domain.Repositories;
using static StoreOnline.Application.UnitTests.Common.Behaviours.TestStoreDataInitializer;

namespace StoreOnline.Application.UnitTests.Common.Behaviours;

[TestFixture]
public class CreateOrderServicesTests
{
    private Mock<IOrderRepository> _mockOrderRepository = null!;
    private Mock<IProductReadRepository> _mockProductReadRepository = null!;
    private Mock<IOrderDetailRepository> _mockOrderDetailRepository = null!;

    [SetUp]
    public void Setup()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockProductReadRepository = new Mock<IProductReadRepository>();
        _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();
    }

    [Test]
    public void ShouldThrowProductNotFoundException()
    {
        _mockOrderRepository.Setup(m => m.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(CreatedOrder);
        _mockProductReadRepository.Setup(m => m.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);
        CreateOrderServices createOrderServices =
            new(
                _mockOrderRepository.Object,
                _mockProductReadRepository.Object,
                _mockOrderDetailRepository.Object);

        CreateOrderCommand createCommandRequest = new()
        {
            CustomerId = 1, Products = new List<ProductDto> { ProductDtoA, ProductDtoB }
        };
        Assert.ThrowsAsync<ProductNotFoundException>(
            () => createOrderServices.CreateOrUpdateAsync(createCommandRequest));
        _mockProductReadRepository.Verify(repository => repository.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }
    
    [Test]
    public async Task ShouldCreateNewOrderNotEmptyProducts()
    {
        _mockOrderRepository.Setup(m => m.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(CreatedOrder);
        _mockProductReadRepository.Setup(m => m.FindByIdAsync(ProductA.Id)).ReturnsAsync(() =>ProductA);
        _mockProductReadRepository.Setup(m => m.FindByIdAsync(ProductB.Id)).ReturnsAsync(() =>ProductB);
        _mockOrderDetailRepository.Setup(m => m.AddAsync(It.Is<OrderDetail>(o => o.Product!.Id == ProductA.Id)))
            .ReturnsAsync(() => OrderDetailA);
        _mockOrderDetailRepository.Setup(m => m.AddAsync(It.Is<OrderDetail>(o => o.Product!.Id == ProductB.Id)))
            .ReturnsAsync(() => OrderDetailB);
        CreateOrderServices createOrderServices =
            new(
                _mockOrderRepository.Object,
                _mockProductReadRepository.Object,
                _mockOrderDetailRepository.Object);

        CreateOrderCommand createCommandRequest = new()
        {
            CustomerId = 1, Products = new List<ProductDto> { ProductDtoA, ProductDtoB }
        };

        var newOrder = await createOrderServices.CreateOrUpdateAsync(createCommandRequest);
        _mockProductReadRepository.Verify(repository => repository.FindByIdAsync(It.IsAny<int>()), Times.Exactly(2));
        newOrder.Should().NotBeNull();
        newOrder.OrderDetails.Should().NotBeEmpty();
        newOrder.OrderDetails.Count.Should().Be(2);
    }
}
