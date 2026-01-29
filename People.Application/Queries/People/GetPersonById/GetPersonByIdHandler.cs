using People.Application.Abstractions;

namespace People.Application.Queries.People.GetPersonById;

public sealed class GetPersonByIdHandler
{
    private readonly IPersonReadRepository _repo;

    public GetPersonByIdHandler(IPersonReadRepository repo) => _repo = repo;

    public Task<GetPersonByIdResult?> Handle(GetPersonByIdQuery query, CancellationToken ct)
        => _repo.GetByIdAsync(query.Id, ct);
}
