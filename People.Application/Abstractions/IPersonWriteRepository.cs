using Microsoft.EntityFrameworkCore.Storage;
using People.Domain.Entities;

namespace People.Application.Abstractions;

public interface IPersonWriteRepository
{
    Task<bool> ExistsByRgOrCpfAsync(string rg, string cpf, CancellationToken ct);
    Task AddAsync(Person person, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct);
}
