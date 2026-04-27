using BookStore.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return HandleError(result.Error);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return HandleError(result.Error);
    }

    protected IActionResult HandleCreated<T>(Result<T> result, string actionName, object routeValues)
    {
        if (result.IsSuccess)
            return CreatedAtAction(actionName, routeValues, result.Value);

        return HandleError(result.Error);
    }

    private IActionResult HandleError(Error error)
    {
        if (error.Code.Contains("NotFound"))
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found.",
                Detail = error.Description,
                Status = StatusCodes.Status404NotFound
            });

        if (error.Code.Contains("Validation"))
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error.",
                Detail = error.Description,
                Status = StatusCodes.Status400BadRequest
            });

        if (error.Code.Contains("AlreadyExists") || error.Code.Contains("SlugExists"))
            return Conflict(new ProblemDetails
            {
                Title = "Conflict.",
                Detail = error.Description,
                Status = StatusCodes.Status409Conflict
            });

        return BadRequest(new ProblemDetails
        {
            Title = "Bad request.",
            Detail = error.Description,
            Status = StatusCodes.Status400BadRequest
        });
    }
}