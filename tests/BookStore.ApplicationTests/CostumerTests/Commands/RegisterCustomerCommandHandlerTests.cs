using BookStore.Application.Features.Customers.Commands.RegisterCustomer;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CostumerTests.Commands;

public class RegisterCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly RegisterCustomerCommandHandler _handler;

    public RegisterCustomerCommandHandlerTests()
    {
        _customerRepository = new Mock<ICustomerRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new RegisterCustomerCommandHandler(
            _customerRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var command = new RegisterCustomerCommand(
            "João",
            "Silva",
            "joao@email.com",
            null,
            "12345678909",
            null
        );

        _customerRepository
            .Setup(x => x.EmailExistsAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _customerRepository.Verify(x =>
            x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWork.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldFail()
    {
        var command = new RegisterCustomerCommand(
            "João",
            "Silva",
            "email-invalido",
            null,
            "12345678909",
            null
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _customerRepository.Verify(x =>
            x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldFail()
    {
        var command = new RegisterCustomerCommand(
            "João",
            "Silva",
            "email@email.com",
            null,
            "12345678909",
            null
        );

        _customerRepository
            .Setup(x => x.EmailExistsAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _customerRepository.Verify(x =>
            x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidCpf_ShouldFail()
    {
        var command = new RegisterCustomerCommand(
            "João",
            "Silva",
            "email@email.com",
            null,
            "11111111111", // CPF inválido
            null
        );

        _customerRepository
            .Setup(x => x.EmailExistsAsync(It.IsAny<Email>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _customerRepository.Verify(x =>
            x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}