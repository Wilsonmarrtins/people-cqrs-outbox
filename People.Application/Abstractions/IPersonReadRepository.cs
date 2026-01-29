using People.Application.Queries.People.GetPersonById;

namespace People.Application.Abstractions;

public interface IPersonReadRepository
{
    Task<GetPersonByIdResult?> GetByIdAsync(Guid id, CancellationToken ct);
}
