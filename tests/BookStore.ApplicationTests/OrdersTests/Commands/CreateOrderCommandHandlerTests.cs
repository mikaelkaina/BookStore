using BookStore.Application.Features.Orders.Commands.CreateOrder;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IBookRepository> _bookRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();

    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler(
            _unitOfWork.Object,
            _orderRepository.Object,
            _bookRepository.Object,
            _customerRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var customer = new CustomerBuilder().Build();

        var book = new BookBuilder()
            .WithStock(10)
            .Build();

        var command = new CreateOrderCommand(
            customer.Id,
            "Rua A",
            "123",
            null,
            "Centro",
            "São Paulo",
            "SP",
            "12345678",
            null,
            new List<OrderItemRequest>
            {
            new(book.Id, 2)
            }
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _bookRepository
            .Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _bookRepository.Verify(x => x.UpdateAsync(book, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCustomerNotFound_ShouldFail()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            "Rua A", "123", null, "Centro", "SP", "SP", "123", null,
            new List<OrderItemRequest>()
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenAddressInvalid_ShouldFail()
    {
        var customer = new CustomerBuilder().Build();

        var command = new CreateOrderCommand(
            customer.Id,
            "",
            "123",
            null,
            "Centro",
            "SP",
            "SP",
            "123",
            null,
            new List<OrderItemRequest>()
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenBookNotFound_ShouldFail()
    {
        var customer = new CustomerBuilder().Build();

        var command = new CreateOrderCommand(
            customer.Id,
            "Rua A", "123", null, "Centro", "SP", "SP", "123", null,
            new List<OrderItemRequest>
            {
            new(Guid.NewGuid(), 1)
            }
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenInsufficientStock_ShouldFail()
    {
        var customer = new CustomerBuilder().Build();

        var book = new BookBuilder()
            .WithStock(1)
            .Build();

        var command = new CreateOrderCommand(
            customer.Id,
            "Rua A", "123", null, "Centro", "SP", "SP", "123", null,
            new List<OrderItemRequest>
            {
            new(book.Id, 5)
            }
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _bookRepository
            .Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDecrementStockFails_ShouldFail()
    {
        var customer = new CustomerBuilder().Build();

        var book = new BookBuilder()
            .WithStock(1)
            .Build();

        var command = new CreateOrderCommand(
            customer.Id,
            "Rua A", "123", null, "Centro", "SP", "SP", "123", null,
            new List<OrderItemRequest>
            {
            new(book.Id, 1)
            }
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _bookRepository
            .Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        book.DecrementStock(1);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
