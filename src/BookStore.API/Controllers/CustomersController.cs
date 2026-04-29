using BookStore.Application.Features.Customers.Commands.DeleteCustomer;
using BookStore.Application.Features.Customers.Commands.RegisterCustomer;
using BookStore.Application.Features.Customers.Commands.UpdateCustomerProfile;
using BookStore.Application.Features.Customers.Queries.GetCustomerByEmail;
using BookStore.Application.Features.Customers.Queries.GetCustomerById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[Authorize]
public sealed class CustomersController : BaseController
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
        => _sender = sender;

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomerByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("email/{email}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail(
        string email,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomerByEmailQuery(email), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return HandleCreated(result, nameof(GetById), new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        Guid id,
        [FromBody] UpdateCustomerProfileCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.CustomerId)
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
        var result = await _sender.Send(new DeleteCustomerCommand(id), cancellationToken);
        return HandleResult(result);
    }
}