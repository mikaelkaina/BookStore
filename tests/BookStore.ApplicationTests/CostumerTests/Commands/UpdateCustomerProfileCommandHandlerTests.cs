using BookStore.Application.Features.Customers.Commands.UpdateCustomerProfile;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CostumerTests.Commands;

public class UpdateCustomerProfileCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateCustomerProfileCommandHandler _handler;

    public UpdateCustomerProfileCommandHandlerTests()
    {
        _customerRepository = new Mock<ICustomerRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new UpdateCustomerProfileCommandHandler(
            _customerRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var customer = Customer.Create(
            "João",
            "Silva",
            "email@email.com",
            document: "12345678909"
        ).Value;

        var command = new UpdateCustomerProfileCommand(
            customer.Id,
            "Carlos",
            "Mendes",
            "(11) 99999-9999"
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Carlos");

        _customerRepository.Verify(
            x => x.UpdateAsync(customer, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCustomerNotFound_ShouldFail()
    {
        var command = new UpdateCustomerProfileCommand(
            Guid.NewGuid(),
            "Carlos",
            "Mendes",
            null
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _customerRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDomainValidationFails_ShouldFail()
    {
        var customer = Customer.Create(
            "João",
            "Silva",
            "email@email.com",
            document: "12345678909"
        ).Value;

        var command = new UpdateCustomerProfileCommand(
            customer.Id,
            "", // inválido
            "Mendes",
            null
        );

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _customerRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
