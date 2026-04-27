using BookStore.Application.Features.Carts.Commands.AddItemToCart;
using BookStore.Application.Features.Carts.Commands.CheckoutCart;
using BookStore.Application.Features.Carts.Commands.ClearCart;
using BookStore.Application.Features.Carts.Commands.RemoveItemFromCart;
using BookStore.Application.Features.Carts.Commands.UpdateItemQuantity;
using BookStore.Application.Features.Carts.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

public sealed class CartsController : BaseController
{
    private readonly ISender _sender;

    public CartsController(ISender sender)
        => _sender = sender;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart(
        [FromQuery] Guid? customerId,
        [FromQuery] string? sessionId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCartQuery(customerId, sessionId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(
        [FromBody] AddItemToCartCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{cartId:guid}/items/{bookId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(
        Guid cartId,
        Guid bookId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RemoveItemFromCartCommand(cartId, bookId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{cartId:guid}/items/{bookId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItemQuantity(
        Guid cartId,
        Guid bookId,
        [FromBody] UpdateItemQuantityCommand command,
        CancellationToken cancellationToken)
    {
        if (cartId != command.CartId || bookId != command.BookId)
            return BadRequest(new ProblemDetails
            {
                Title = "Id mismatch.",
                Detail = "The ids in the route must match the ids in the body.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{cartId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Clear(
        Guid cartId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ClearCartCommand(cartId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{cartId:guid}/checkout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Checkout(
        Guid cartId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CheckoutCartCommand(cartId), cancellationToken);
        return HandleResult(result);
    }
}