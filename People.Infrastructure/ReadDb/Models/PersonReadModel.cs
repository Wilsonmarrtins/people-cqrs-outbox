namespace People.Infrastructure.ReadDb.Models;

public sealed class PersonReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string Sex { get; set; } = default!;
    public string Rg { get; set; } = default!;
    public string Cpf { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }
}
