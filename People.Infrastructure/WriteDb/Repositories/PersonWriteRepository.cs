using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using People.Application.Abstractions;
using People.Domain.Entities;

namespace People.Infrastructure.WriteDb.Repositories;

public sealed class PersonWriteRepository : IPersonWriteRepository
{
    private readonly WriteDbContext _db;

    public PersonWriteRepository(WriteDbContext db) => _db = db;

    public Task<bool> ExistsByRgOrCpfAsync(string rg, string cpf, CancellationToken ct)
        => _db.People.AnyAsync(x => x.Rg == rg || x.Cpf == cpf, ct);

    public Task AddAsync(Person person, CancellationToken ct)
        => _db.People.AddAsync(person, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct)
           => _db.Database.BeginTransactionAsync(ct);

    public Task<Person?> GetByIdAsync(Guid id, CancellationToken ct)
    => _db.People.FirstOrDefaultAsync(x => x.Id == id, ct);

    public void Remove(Person person)
        => _db.People.Remove(person);
}
