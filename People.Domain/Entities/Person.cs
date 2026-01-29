namespace People.Domain.Entities;

public sealed class Person
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public int Age { get; private set; }
    public string Sex { get; private set; } = default!;
    public string Rg { get; private set; } = default!;
    public string Cpf { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private Person() { } // EF Core

    public Person(string name, int age, string sex, string rg, string cpf)
    {
        Update(name, age, sex, rg, cpf);
    }

    public void Update(string name, int age, string sex, string rg, string cpf)
    {
        name = name.Trim();
        sex = sex.Trim();
        rg = rg.Trim();
        cpf = cpf.Trim();

        if (name.Length < 2) throw new InvalidOperationException("Name is invalid.");
        if (age < 0 || age > 130) throw new InvalidOperationException("Age is invalid.");
        if (sex.Length < 1) throw new InvalidOperationException("Sex is invalid.");
        if (rg.Length < 5) throw new InvalidOperationException("RG is invalid.");
        if (cpf.Length < 11) throw new InvalidOperationException("CPF is invalid.");

        Name = name;
        Age = age;
        Sex = sex;
        Rg = rg;
        Cpf = cpf;
    }
}
