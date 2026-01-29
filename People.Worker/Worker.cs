using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using People.Infrastructure.ReadDb;
using People.Infrastructure.ReadDb.Models;
using People.Infrastructure.WriteDb;
using People.Infrastructure.WriteDb.Outbox;

namespace People.Worker.Outbox;

public sealed class OutboxSyncWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxSyncWorker(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SyncOnce(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }

    private async Task SyncOnce(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var writeDb = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        var readDb = scope.ServiceProvider.GetRequiredService<ReadDbContext>();

        var batch = await writeDb.Outbox
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(50)
            .ToListAsync(ct);

        if (batch.Count == 0) return;

        foreach (var msg in batch)
        {
            if (msg.Type == "PersonCreated")
            {
                var dto = JsonSerializer.Deserialize<PersonCreatedDto>(msg.PayloadJson);
                if (dto is null) continue;

                var existing = await readDb.People.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

                if (existing is null)
                {
                    await readDb.People.AddAsync(new PersonReadModel
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Age = dto.Age,
                        Sex = dto.Sex,
                        Rg = dto.Rg,
                        Cpf = dto.Cpf,
                        CreatedAtUtc = dto.CreatedAtUtc
                    }, ct);
                }
                else
                {
                    existing.Name = dto.Name;
                    existing.Age = dto.Age;
                    existing.Sex = dto.Sex;
                    existing.Rg = dto.Rg;
                    existing.Cpf = dto.Cpf;
                    existing.CreatedAtUtc = dto.CreatedAtUtc;
                }
            }

            msg.ProcessedAtUtc = DateTime.UtcNow;
        }

        await readDb.SaveChangesAsync(ct);
        await writeDb.SaveChangesAsync(ct);
    }

    private sealed record PersonCreatedDto(
        Guid Id,
        string Name,
        int Age,
        string Sex,
        string Rg,
        string Cpf,
        DateTime CreatedAtUtc
    );
}
