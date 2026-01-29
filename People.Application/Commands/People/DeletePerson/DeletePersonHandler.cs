using People.Application.Abstractions;

namespace People.Application.Commands.People.DeletePerson;

public sealed class DeletePersonHandler
{
    private readonly IPersonWriteRepository _repo;
    private readonly IOutboxWriter _outbox;

    public DeletePersonHandler(
        IPersonWriteRepository repo,
        IOutboxWriter outbox)
    {
        _repo = repo;
        _outbox = outbox;
    }

    public async Task<DeletePersonResult> Handle(DeletePersonCommand cmd, CancellationToken ct)
    {
        await using var tx = await _repo.BeginTransactionAsync(ct);

        var person = await _repo.GetByIdAsync(cmd.Id, ct);
        if (person is null)
            return new DeletePersonResult(true);

        _repo.Remove(person);

        await _outbox.AddAsync("PersonDeleted", new
        {
            person.Id
        }, ct);

        await _repo.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new DeletePersonResult(false);
    }
}
