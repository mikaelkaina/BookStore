using BookStore.Application.Features.Customers.Queries.GetCustomerById;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;


namespace BookStore.ApplicationTests.CostumerTests.Queries;

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly GetCustomerByIdQueryHandler _handler;

    public GetCustomerByIdQueryHandlerTests()
    {
        _customerRepository = new Mock<ICustomerRepository>();

        _handler = new GetCustomerByIdQueryHandler(
            _customerRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingCustomer_ShouldReturnSuccess()
    {
        var customerId = Guid.NewGuid();

        var customer = Customer.Create(
            "João",
            "Silva",
            "email@email.com",
            document: "12345678909"
        ).Value;

        _customerRepository
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var query = new GetCustomerByIdQuery(customerId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(customer.Id);
        result.Value.Email.Should().Be("email@email.com");
    }

    [Fact]
    public async Task Handle_WithNonExistingCustomer_ShouldReturnFailure()
    {
        var customerId = Guid.NewGuid();

        _customerRepository
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var query = new GetCustomerByIdQuery(customerId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallRepository()
    {
        var customerId = Guid.NewGuid();

        _customerRepository
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var query = new GetCustomerByIdQuery(customerId);

        await _handler.Handle(query, CancellationToken.None);

        _customerRepository.Verify(
            x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
