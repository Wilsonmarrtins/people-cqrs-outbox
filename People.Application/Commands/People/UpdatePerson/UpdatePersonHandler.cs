using People.Application.Abstractions;
using People.Domain.Entities;

namespace People.Application.Commands.People.UpdatePerson;

public sealed class UpdatePersonHandler
{
    private readonly IPersonWriteRepository _repo;
    private readonly IOutboxWriter _outbox;

    public UpdatePersonHandler(
        IPersonWriteRepository repo,
        IOutboxWriter outbox)
    {
        _repo = repo;
        _outbox = outbox;
    }

    public async Task<UpdatePersonResult> Handle(UpdatePersonCommand cmd, CancellationToken ct)
    {
        await using var tx = await _repo.BeginTransactionAsync(ct);

        var person = await _repo.GetByIdAsync(cmd.Id, ct);
        if (person is null)
            return new UpdatePersonResult(true);

        person.Update(cmd.Name, cmd.Age, cmd.Sex, cmd.Rg, cmd.Cpf);

        await _outbox.AddAsync("PersonUpdated", new
        {
            person.Id,
            person.Name,
            person.Age,
            person.Sex,
            person.Rg,
            person.Cpf,
            person.CreatedAtUtc
        }, ct);

        await _repo.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new UpdatePersonResult(false);
    }
}
