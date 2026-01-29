using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using People.Infrastructure.ReadDb;
using People.Infrastructure.ReadDb.Models;
using People.Infrastructure.WriteDb;

namespace People.Worker.Services;

public sealed class OutboxProcessor
{
    private readonly WriteDbContext _writeDb;
    private readonly ReadDbContext _readDb;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(WriteDbContext writeDb, ReadDbContext readDb, ILogger<OutboxProcessor> logger)
    {
        _writeDb = writeDb;
        _readDb = readDb;
        _logger = logger;
    }

    public async Task ProcessBatchAsync(int batchSize, CancellationToken ct)
    {
        var messages = await _writeDb.Outbox
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(batchSize)
            .ToListAsync(ct);

        if (messages.Count == 0) return;

        foreach (var msg in messages)
        {
            try
            {
                switch (msg.Type)
                {
                    case "PersonCreated":
                        await HandlePersonCreatedAsync(msg.PayloadJson, ct);
                        break;
                    case "PersonUpdated":
                        await HandlePersonCreatedAsync(msg.PayloadJson, ct);
                        break;

                    case "PersonDeleted":
                        await HandlePersonDeletedAsync(msg.PayloadJson, ct);
                        break;

                    default:
                        _logger.LogWarning("Unknown outbox message type: {Type}", msg.Type);
                        break;
                }

                msg.ProcessedAtUtc = DateTime.UtcNow;
                await _writeDb.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                // IMPORTANTE: não marca ProcessedAtUtc se falhar
                _logger.LogError(ex, "Failed processing outbox message {Id} ({Type})", msg.Id, msg.Type);
            }
        }
    }

    private async Task HandlePersonCreatedAsync(string payloadJson, CancellationToken ct)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<PersonCreatedDto>(payloadJson)
          ?? throw new InvalidOperationException("Invalid payload for PersonCreated.");

            var existing = await _readDb.People.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

            if (existing is null)
            {
                _readDb.People.Add(new PersonReadModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Age = dto.Age,
                    Sex = dto.Sex,
                    Rg = dto.Rg,
                    Cpf = dto.Cpf,
                    CreatedAtUtc = dto.CreatedAtUtc
                });
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

            await _readDb.SaveChangesAsync(ct);
        }
        catch (Exception Ex)
        {
            var error = Ex.Message;
        }
    }

    private async Task HandlePersonDeletedAsync(string payloadJson, CancellationToken ct)
    {
        var dto = JsonSerializer.Deserialize<PersonDeletedDto>(payloadJson)!;

        var existing = await _readDb.People.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
        if (existing is not null)
        {
            _readDb.People.Remove(existing);
            await _readDb.SaveChangesAsync(ct);
        }
    }

    private sealed record PersonDeletedDto(Guid Id);

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
