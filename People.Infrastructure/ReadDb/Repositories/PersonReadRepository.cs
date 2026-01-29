using Microsoft.EntityFrameworkCore;
using People.Application.Abstractions;
using People.Application.Queries.People.GetPersonById;

namespace People.Infrastructure.ReadDb.Repositories;

public sealed class PersonReadRepository : IPersonReadRepository
{
    private readonly ReadDbContext _db;

    public PersonReadRepository(ReadDbContext db) => _db = db;

    public async Task<GetPersonByIdResult?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var p = await _db.People.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p is null) return null;

        return new GetPersonByIdResult(p.Id, p.Name, p.Age, p.Sex, p.Rg, p.Cpf, p.CreatedAtUtc);
    }
}
