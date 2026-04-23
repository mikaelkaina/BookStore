using BookStore.Application.Features.Books.Commands.UpdateBookPrice;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.BookTests;

public class UpdateBookPriceCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateBookPriceCommandHandler _handler;

    public UpdateBookPriceCommandHandlerTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new UpdateBookPriceCommandHandler(
            _unitOfWork.Object,
            _bookRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var command = new UpdateBookPriceCommand(Guid.NewGuid(), 100);

        var book = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            10,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        ).Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _bookRepository.Verify(x => x.UpdateAsync(book, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookNotFound_ShouldFail()
    {
        var command = new UpdateBookPriceCommand(Guid.NewGuid(), 100);

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _bookRepository.Verify(x => x.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidPrice_ShouldFail()
    {
        var command = new UpdateBookPriceCommand(Guid.NewGuid(), -10);

        var book = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            10,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        ).Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _bookRepository.Verify(x => x.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
