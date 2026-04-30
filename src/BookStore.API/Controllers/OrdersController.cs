using BookStore.Application.Features.Orders.Commands.ApplyDiscount;
using BookStore.Application.Features.Orders.Commands.CancelOrder;
using BookStore.Application.Features.Orders.Commands.ConfirmOrderPayment;
using BookStore.Application.Features.Orders.Commands.CreateOrder;
using BookStore.Application.Features.Orders.Commands.DeliverOrder;
using BookStore.Application.Features.Orders.Commands.SetShipping;
using BookStore.Application.Features.Orders.Commands.ShipOrder;
using BookStore.Application.Features.Orders.Commands.StartOrderProcessing;
using BookStore.Application.Features.Orders.Queries.GetOrderById;
using BookStore.Application.Features.Orders.Queries.GetOrdersByCustomer;
using BookStore.Application.Features.Orders.Queries.GetOrdersPaged;
using BookStore.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

public sealed record CancelOrderRequest(string? Reason);


[Authorize]
public sealed class OrdersController : BaseController
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
        => _sender = sender;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] Guid? customerId,
        [FromQuery] OrderStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetOrdersPagedQuery(customerId, status, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCustomer(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOrdersByCustomerQuery(customerId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleCreated(result, nameof(GetById), new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    [HttpPatch("{id:guid}/payment/confirm")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmPayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ConfirmOrderPaymentCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/processing/start")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartProcessing(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new StartOrderProcessingCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/ship")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ship(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ShipOrderCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/deliver")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deliver(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeliverOrderCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
    Guid id,
    [FromBody] CancelOrderRequest? request,
    CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand(id, request?.Reason);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/discount")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApplyDiscount(
        Guid id,
        [FromBody] ApplyDiscountCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.OrderId)
            return BadRequest(new ProblemDetails
            {
                Title = "Id mismatch.",
                Detail = "The id in the route must match the id in the body.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/shipping")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetShipping(
        Guid id,
        [FromBody] SetShippingCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.OrderId)
            return BadRequest(new ProblemDetails
            {
                Title = "Id mismatch.",
                Detail = "The id in the route must match the id in the body.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
