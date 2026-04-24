using BookStore.Application.Features.Customers.Queries.GetCustomerByEmail;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CostumerTests.Queries;

public class GetCustomerByEmailQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly GetCustomerByEmailQueryHandler _handler;

    public GetCustomerByEmailQueryHandlerTests()
    {
        _customerRepository = new Mock<ICustomerRepository>();

        _handler = new GetCustomerByEmailQueryHandler(
            _customerRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidEmail_ShouldReturnCustomer()
    {
        var email = "email@email.com";

        var customer = Customer.Create(
            "João",
            "Silva",
            email,
            document: "12345678909"
        ).Value;

        _customerRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var query = new GetCustomerByEmailQuery(email);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(email);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldFail()
    {
        var query = new GetCustomerByEmailQuery("email-invalido");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _customerRepository.Verify(
            x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistingCustomer_ShouldFail()
    {
        var email = "email@email.com";

        _customerRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var query = new GetCustomerByEmailQuery(email);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallRepository()
    {
        var email = "email@email.com";

        _customerRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var query = new GetCustomerByEmailQuery(email);

        await _handler.Handle(query, CancellationToken.None);

        _customerRepository.Verify(
            x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
