using People.Application.Abstractions;
using People.Domain.Entities;

namespace People.Application.Commands.People.CreatePerson;

public sealed class CreatePersonHandler
{
    private readonly IPersonWriteRepository _repo;
    private readonly IOutboxWriter _outbox;

    public CreatePersonHandler(IPersonWriteRepository repo, IOutboxWriter outbox)
    {
        _repo = repo;
        _outbox = outbox;
    }

    public async Task<CreatePersonResult> Handle(CreatePersonCommand cmd, CancellationToken ct)
    {
        //a transação está no WriteDbContext(SQL Server)
        await using var tx = await _repo.BeginTransactionAsync(ct);

        var rg = cmd.Rg.Trim();
        var cpf = cmd.Cpf.Trim();

        if (await _repo.ExistsByRgOrCpfAsync(rg, cpf, ct))
            throw new InvalidOperationException("RG or CPF already exists.");

        var person = new Person(cmd.Name, cmd.Age, cmd.Sex, rg, cpf);

        await _repo.AddAsync(person, ct);

        // O registro da Outbox deve ser inserido na MESMA transação do WriteDbContext
        await _outbox.AddAsync("PersonCreated", new
        {
            person.Id,
            person.Name,
            person.Age,
            person.Sex,
            person.Rg,
            person.Cpf,
            person.CreatedAtUtc
        }, ct);

        // Um único commit para ambos os inserts
        await _repo.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new CreatePersonResult(person.Id);
    }
}
