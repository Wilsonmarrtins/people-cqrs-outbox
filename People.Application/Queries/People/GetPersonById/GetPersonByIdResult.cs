namespace People.Application.Queries.People.GetPersonById;

public sealed record GetPersonByIdResult(
    Guid Id,
    string Name,
    int Age,
    string Sex,
    string Rg,
    string Cpf,
    DateTime CreatedAtUtc
);
