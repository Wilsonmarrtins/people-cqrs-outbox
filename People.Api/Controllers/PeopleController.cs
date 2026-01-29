using Microsoft.AspNetCore.Mvc;
using People.Application.Commands.People.CreatePerson;
using People.Application.Queries.People.GetPersonById;

namespace People.Api.Controllers;

[ApiController]
[Route("api/people")]
public sealed class PeopleController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromServices] CreatePersonHandler handler,[FromBody] CreatePersonCommand cmd, CancellationToken ct)
    {
        var result = await handler.Handle(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromServices] GetPersonByIdHandler handler,[FromRoute] Guid id, CancellationToken ct)
    {
        var result = await handler.Handle(new GetPersonByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }
}
