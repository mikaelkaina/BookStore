using BookStore.Application.Features.Books.Commands.CreateBook;
using BookStore.Application.Features.Books.Commands.DeleteBook;
using BookStore.Application.Features.Books.Commands.ManageStock;
using BookStore.Application.Features.Books.Commands.UpdateBook;
using BookStore.Application.Features.Books.Commands.UpdateBookPrice;
using BookStore.Application.Features.Books.Queries.GetBookById;
using BookStore.Application.Features.Books.Queries.GetBooksByCategory;
using BookStore.Application.Features.Books.Queries.GetBooksPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

public sealed class BooksController : BaseController
{
    private readonly ISender _sender;

    public BooksController(ISender sender)
        => _sender = sender;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? sortByPrice,
        [FromQuery] bool ascending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBooksPagedQuery(
            searchTerm, categoryId, minPrice, maxPrice,
            sortByPrice, ascending, page, pageSize);

        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("category/{categoryId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCategory(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetBooksByCategoryQuery(categoryId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleCreated(result, nameof(GetById), new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateBookCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.BookId)
            return BadRequest(new ProblemDetails
            {
                Title = "Id mismatch.",
                Detail = "The id in the route must match the id in the body.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/price")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePrice(
        Guid id,
        [FromBody] UpdateBookPriceCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.BookId)
            return BadRequest(new ProblemDetails
            {
                Title = "Id mismatch.",
                Detail = "The id in the route must match the id in the body.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/stock/add")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddStock(
        Guid id,
        [FromBody] AddStockCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.BookId)
            return BadRequest(new ProblemDetails
            {
                Title = "Id mismatch.",
                Detail = "The id in the route must match the id in the body.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/stock/decrement")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DecrementStock(
        Guid id,
        [FromBody] DecrementStockCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.BookId)
            return BadRequest(new ProblemDetails
            {
                Title = "Id mismatch.",
                Detail = "The id in the route must match the id in the body.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteBookCommand(id), cancellationToken);
        return HandleResult(result);
    }
}