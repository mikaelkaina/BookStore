using BookStore.Application.Features.Customers.Commands.DeleteCustomer;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CostumerTests.Commands;

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteCustomerCommandHandler _handler;

    public DeleteCustomerCommandHandlerTests()
    {
        _customerRepository = new Mock<ICustomerRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new DeleteCustomerCommandHandler(
            _customerRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingCustomer_ShouldDeactivateAndSucceed()
    {
        var customer = Customer.Create(
            "João",
            "Silva",
            "email@email.com",
            document: "12345678909"
        ).Value;

        var command = new DeleteCustomerCommand(customer.Id);

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        customer.IsActive.Should().BeFalse();

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
        var command = new DeleteCustomerCommand(Guid.NewGuid());

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
    public async Task Handle_ShouldCallRepository()
    {
        var customer = Customer.Create(
            "João",
            "Silva",
            "email@email.com",
            document: "12345678909"
        ).Value;

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var command = new DeleteCustomerCommand(customer.Id);

        await _handler.Handle(command, CancellationToken.None);

        _customerRepository.Verify(
            x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}