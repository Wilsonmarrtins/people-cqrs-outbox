using System.Text.Json;
using People.Application.Abstractions;
namespace People.Infrastructure.WriteDb.Outbox;

public sealed class EfOutboxWriter : IOutboxWriter
{
    private readonly WriteDbContext _db;

    public EfOutboxWriter(WriteDbContext db) => _db = db;

    public async Task AddAsync(string type, object payload, CancellationToken ct)
    {
        var msg = new OutboxMessage
        {
            Type = type,
            PayloadJson = JsonSerializer.Serialize(payload),
            OccurredAtUtc = DateTime.UtcNow
        };

        await _db.Outbox.AddAsync(msg, ct);
    }
}
