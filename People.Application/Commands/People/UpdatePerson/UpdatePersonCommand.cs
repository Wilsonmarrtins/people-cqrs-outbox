namespace People.Application.Commands.People.UpdatePerson;

public sealed record UpdatePersonCommand(
    Guid Id,
    string Name,
    int Age,
    string Sex,
    string Rg,
    string Cpf
);
