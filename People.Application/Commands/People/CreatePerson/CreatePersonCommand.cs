using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace People.Application.Commands.People.CreatePerson
{
    public sealed record CreatePersonCommand(
        string Name,
        int Age,
        string Sex,
        string Rg,
        string Cpf
    );
}
